using System;
using MessagePipe;
using UnityEngine;
using UnityEngine.UI;
using VContainer.Unity;

namespace FishEvolution.Tutorial
{
    public sealed class HighlightUI : IStartable, IDisposable
    {
        private readonly ISubscriber<TutorialStepChangedEvent> _stepSubscriber;
        private readonly ISubscriber<TutorialCompletedEvent> _completedSubscriber;
        private IDisposable _stepSubscription;
        private IDisposable _completedSubscription;
        private GameObject _root;
        private RectTransform _highlight;
        private Text _messageText;

        public HighlightUI(
            ISubscriber<TutorialStepChangedEvent> stepSubscriber,
            ISubscriber<TutorialCompletedEvent> completedSubscriber)
        {
            _stepSubscriber = stepSubscriber;
            _completedSubscriber = completedSubscriber;
        }

        public void Start()
        {
            CreateView();
            _stepSubscription = _stepSubscriber.Subscribe(HandleStepChanged);
            _completedSubscription = _completedSubscriber.Subscribe(_ => Hide());
        }

        public void Dispose()
        {
            _stepSubscription?.Dispose();
            _completedSubscription?.Dispose();
            if (_root != null)
            {
                UnityEngine.Object.Destroy(_root);
            }
        }

        private void HandleStepChanged(TutorialStepChangedEvent stepEvent)
        {
            if (stepEvent.Step == null)
            {
                Hide();
                return;
            }

            _root.SetActive(true);
            _messageText.text = stepEvent.Step.Message;
            _highlight.anchoredPosition = stepEvent.Step.HighlightPosition;
            _highlight.sizeDelta = stepEvent.Step.HighlightSize;
        }

        private void Hide()
        {
            if (_root != null)
            {
                _root.SetActive(false);
            }
        }

        private void CreateView()
        {
            var canvas = CreateCanvas();
            var rootRect = CreateRoot(canvas.transform);
            _highlight = CreateHighlight(rootRect);
            _messageText = CreateMessage(rootRect);
            _root.SetActive(false);
        }

        private Canvas CreateCanvas()
        {
            _root = new GameObject("TutorialHighlightUI", typeof(Canvas));
            var canvas = _root.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 80;

            var scaler = _root.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;
            _root.AddComponent<GraphicRaycaster>();
            return canvas;
        }

        private RectTransform CreateRoot(Transform parent)
        {
            var panel = new GameObject("Tutorial_Mask", typeof(RectTransform), typeof(Image));
            var rect = panel.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            panel.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.45f);
            return rect;
        }

        private RectTransform CreateHighlight(Transform parent)
        {
            var item = new GameObject("Tutorial_Highlight", typeof(RectTransform), typeof(Image));
            var rect = item.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = rect.anchorMin;
            item.GetComponent<Image>().color = new Color(1f, 0.9f, 0.25f, 0.45f);
            return rect;
        }

        private Text CreateMessage(Transform parent)
        {
            var textObject = new GameObject("Tutorial_Message", typeof(RectTransform), typeof(Text));
            var rect = textObject.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = rect.anchorMin;
            rect.pivot = new Vector2(0.5f, 0f);
            rect.anchoredPosition = new Vector2(0f, 72f);
            rect.sizeDelta = new Vector2(760f, 96f);

            var text = textObject.GetComponent<Text>();
            text.font = Font.CreateDynamicFontFromOSFont("Noto Sans", 28);
            text.fontSize = 28;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
            return text;
        }
    }
}
