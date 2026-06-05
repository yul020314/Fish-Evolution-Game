using System;
using FishEvolution.Config;
using MessagePipe;
using UnityEngine;
using UnityEngine.UI;
using VContainer.Unity;

namespace FishEvolution.FishBook
{
    public sealed class FishPreviewUI : IStartable, IDisposable
    {
        private readonly ISubscriber<FishBookChangedEvent> _changedSubscriber;
        private readonly ISubscriber<FishBookSelectionChangedEvent> _selectionSubscriber;
        private readonly IPublisher<FishBookSelectionRequest> _selectionPublisher;
        private IDisposable _changedSubscription;
        private IDisposable _selectionSubscription;
        private RectTransform _listRoot;
        private Text _detailText;
        private Font _font;

        public FishPreviewUI(
            ISubscriber<FishBookChangedEvent> changedSubscriber,
            ISubscriber<FishBookSelectionChangedEvent> selectionSubscriber,
            IPublisher<FishBookSelectionRequest> selectionPublisher)
        {
            _changedSubscriber = changedSubscriber;
            _selectionSubscriber = selectionSubscriber;
            _selectionPublisher = selectionPublisher;
        }

        public void Start()
        {
            CreateView();
            _changedSubscription = _changedSubscriber.Subscribe(HandleBookChanged);
            _selectionSubscription = _selectionSubscriber.Subscribe(HandleSelectionChanged);
        }

        public void Dispose()
        {
            _changedSubscription?.Dispose();
            _selectionSubscription?.Dispose();
            _changedSubscription = null;
            _selectionSubscription = null;
        }

        private void HandleBookChanged(FishBookChangedEvent bookEvent)
        {
            ClearList();
            var entries = bookEvent.Entries ?? new FishBookEntry[0];
            for (var index = 0; index < entries.Length; index++)
            {
                CreateEntryText(entries[index], index);
            }
        }

        private void HandleSelectionChanged(FishBookSelectionChangedEvent selectionEvent)
        {
            _detailText.text = GetDetailText(selectionEvent.Entry);
        }

        private void CreateView()
        {
            _font = Font.CreateDynamicFontFromOSFont("Noto Sans", 18);
            var canvas = CreateCanvas();
            var root = CreatePanel(canvas.transform);
            _listRoot = CreateList(root);
            _detailText = CreateText(
                root,
                "FishBook_Detail",
                new Vector2(220f, -70f),
                new Vector2(360f, 260f),
                18,
                TextAnchor.UpperLeft);
        }

        private Canvas CreateCanvas()
        {
            var canvasObject = new GameObject("FishBookPreviewUI", typeof(Canvas));
            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 40;

            var scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;
            canvasObject.AddComponent<GraphicRaycaster>();
            return canvas;
        }

        private RectTransform CreatePanel(Transform parent)
        {
            var panel = new GameObject("FishBook_Panel", typeof(RectTransform), typeof(Image));
            var rect = panel.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = new Vector2(0f, 0.5f);
            rect.anchorMax = rect.anchorMin;
            rect.pivot = new Vector2(0f, 0.5f);
            rect.anchoredPosition = new Vector2(24f, 0f);
            rect.sizeDelta = new Vector2(620f, 360f);
            panel.GetComponent<Image>().color = new Color(0.04f, 0.22f, 0.28f, 0.72f);
            return rect;
        }

        private RectTransform CreateList(Transform parent)
        {
            var list = new GameObject("FishBook_List", typeof(RectTransform));
            var rect = list.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = rect.anchorMin;
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = new Vector2(18f, -24f);
            rect.sizeDelta = new Vector2(180f, 312f);
            return rect;
        }

        private void CreateEntryText(
            FishBookEntry entry,
            int index)
        {
            var text = CreateText(
                _listRoot,
                "FishBook_Entry",
                new Vector2(0f, -index * 28f),
                new Vector2(172f, 24f),
                16,
                TextAnchor.MiddleLeft);
            text.text = entry.IsUnlocked ? entry.FishName : "Locked";
            text.color = entry.IsUnlocked
                ? new Color(0.8f, 1f, 0.94f, 1f)
                : new Color(0.55f, 0.65f, 0.68f, 1f);
            AddSelectionButton(text, entry);
        }

        private void AddSelectionButton(
            Text text,
            FishBookEntry entry)
        {
            var button = text.gameObject.AddComponent<Button>();
            button.targetGraphic = text;
            button.onClick.AddListener(
                () => _selectionPublisher.Publish(
                    new FishBookSelectionRequest(entry.FishId)));
        }

        private Text CreateText(
            Transform parent,
            string name,
            Vector2 position,
            Vector2 size,
            int fontSize,
            TextAnchor alignment)
        {
            var textObject = new GameObject(name, typeof(RectTransform), typeof(Text));
            var rect = textObject.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = rect.anchorMin;
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;

            var text = textObject.GetComponent<Text>();
            text.font = _font;
            text.fontSize = fontSize;
            text.color = new Color(0.9f, 1f, 0.98f, 1f);
            text.alignment = alignment;
            return text;
        }

        private string GetDetailText(FishBookEntry entry)
        {
            if (!entry.IsUnlocked || entry.FishData == null)
            {
                return "Locked";
            }

            var fish = entry.FishData;
            return fish.FishName +
                "\nRarity: " + fish.Rarity +
                "\nHP: " + fish.HP +
                "\nAttack: " + fish.Attack +
                "\nSpeed: " + fish.Speed +
                "\nScale: " + fish.Scale +
                "\nSkill: " + fish.Skill;
        }

        private void ClearList()
        {
            for (var index = _listRoot.childCount - 1; index >= 0; index--)
            {
                UnityEngine.Object.Destroy(_listRoot.GetChild(index).gameObject);
            }
        }
    }
}
