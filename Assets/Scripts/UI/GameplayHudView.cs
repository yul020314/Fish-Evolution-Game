using UnityEngine;
using UnityEngine.UI;

namespace FishEvolution.UI
{
    public sealed class GameplayHudView
    {
        private const string HudRootName = "GameplayHUD";
        private const string BackgroundName = "HUD_Background";
        private const string BarName = "HUD_ExpBar";
        private const string MinimapName = "HUD_Minimap";

        private readonly string _fontName;
        private readonly Color _panelColor;
        private readonly Color _textColor;
        private readonly Color _barBackgroundColor;
        private readonly Color _barFillColor;
        private readonly Color _mapColor;
        private readonly Color _playerMarkerColor;

        public GameplayHudView(
            string fontName,
            Color panelColor,
            Color textColor,
            Color barBackgroundColor,
            Color barFillColor,
            Color mapColor,
            Color playerMarkerColor)
        {
            _fontName = fontName;
            _panelColor = panelColor;
            _textColor = textColor;
            _barBackgroundColor = barBackgroundColor;
            _barFillColor = barFillColor;
            _mapColor = mapColor;
            _playerMarkerColor = playerMarkerColor;
        }

        public GameplayHudBinding Create()
        {
            var canvas = CreateCanvas();
            var root = CreateRoot(canvas.transform);
            CreateInfoPanel(root);
            CreateMapPanel(root);
            var levelText = CreateText(root, "HUD_Level", new Vector2(24f, -22f), 22);
            var experienceText = CreateText(root, "HUD_Experience", new Vector2(24f, -54f), 18);
            var goldText = CreateText(root, "HUD_Gold", new Vector2(24f, -84f), 18);
            var skillText = CreateText(root, "HUD_Skill", new Vector2(24f, -114f), 18);
            var mapText = CreateText(
                root,
                "HUD_Map",
                new Vector2(-36f, -22f),
                new Vector2(144f, 32f),
                17);
            var progressBar = CreateProgressBar(root);
            var marker = CreateMinimap(root);
            return new GameplayHudBinding(
                canvas.gameObject,
                levelText,
                experienceText,
                goldText,
                skillText,
                mapText,
                progressBar,
                marker);
        }

        private Canvas CreateCanvas()
        {
            var canvasObject = new GameObject(HudRootName, typeof(Canvas));
            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 50;

            var scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;
            canvasObject.AddComponent<GraphicRaycaster>();
            return canvas;
        }

        private RectTransform CreateRoot(Transform parent)
        {
            var root = new GameObject(BackgroundName, typeof(RectTransform), typeof(Image));
            var rect = root.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            root.GetComponent<Image>().color = Color.clear;
            return rect;
        }

        private void CreateInfoPanel(Transform parent)
        {
            var panel = CreateImage(
                parent,
                "HUD_InfoPanel",
                new Vector2(16f, -90f),
                new Vector2(320f, 178f),
                _panelColor);
            panel.anchorMin = new Vector2(0f, 1f);
            panel.anchorMax = panel.anchorMin;
            panel.pivot = new Vector2(0f, 0.5f);
        }

        private void CreateMapPanel(Transform parent)
        {
            var panel = CreateImage(
                parent,
                "HUD_MapPanel",
                new Vector2(-104f, -92f),
                new Vector2(150f, 168f),
                _panelColor);
            panel.anchorMin = new Vector2(1f, 1f);
            panel.anchorMax = panel.anchorMin;
            panel.pivot = new Vector2(0.5f, 0.5f);
        }

        private Text CreateText(
            Transform parent,
            string name,
            Vector2 anchoredPosition,
            int fontSize)
        {
            return CreateText(
                parent,
                name,
                anchoredPosition,
                new Vector2(320f, 32f),
                fontSize);
        }

        private Text CreateText(
            Transform parent,
            string name,
            Vector2 anchoredPosition,
            Vector2 size,
            int fontSize)
        {
            var textObject = new GameObject(name, typeof(RectTransform), typeof(Text));
            var rect = textObject.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = GetAnchorFor(anchoredPosition);
            rect.anchorMax = rect.anchorMin;
            rect.pivot = GetPivotFor(anchoredPosition);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = size;

            var text = textObject.GetComponent<Text>();
            text.font = Font.CreateDynamicFontFromOSFont(_fontName, fontSize);
            text.fontSize = fontSize;
            text.color = _textColor;
            text.alignment = anchoredPosition.x < 0f
                ? TextAnchor.MiddleRight
                : TextAnchor.MiddleLeft;
            return text;
        }

        private HudProgressBar CreateProgressBar(Transform parent)
        {
            var background = CreateImage(
                parent,
                BarName,
                new Vector2(24f, -154f),
                new Vector2(260f, 16f),
                _barBackgroundColor);
            background.anchorMin = new Vector2(0f, 1f);
            background.anchorMax = background.anchorMin;
            background.pivot = new Vector2(0f, 0.5f);

            var fillRect = CreateImage(
                background,
                "HUD_ExpFill",
                Vector2.zero,
                new Vector2(260f, 16f),
                _barFillColor);
            var fill = fillRect.GetComponent<Image>();
            fill.type = Image.Type.Filled;
            fill.fillMethod = Image.FillMethod.Horizontal;
            fill.fillOrigin = 0;
            fill.fillAmount = 0f;
            return new HudProgressBar(fill);
        }

        private RectTransform CreateMinimap(Transform parent)
        {
            var map = CreateImage(
                parent,
                MinimapName,
                new Vector2(-104f, -92f),
                new Vector2(128f, 128f),
                _mapColor);
            map.anchorMin = new Vector2(1f, 1f);
            map.anchorMax = map.anchorMin;
            map.pivot = new Vector2(0.5f, 0.5f);

            var marker = CreateImage(
                map,
                "HUD_PlayerMarker",
                Vector2.zero,
                new Vector2(12f, 12f),
                _playerMarkerColor);
            marker.anchorMin = new Vector2(0.5f, 0.5f);
            marker.anchorMax = marker.anchorMin;
            return marker;
        }

        private RectTransform CreateImage(
            Transform parent,
            string name,
            Vector2 anchoredPosition,
            Vector2 size,
            Color color)
        {
            var imageObject = new GameObject(name, typeof(RectTransform), typeof(Image));
            var rect = imageObject.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = size;
            imageObject.GetComponent<Image>().color = color;
            return rect;
        }

        private static Vector2 GetAnchorFor(Vector2 anchoredPosition)
        {
            return anchoredPosition.x < 0f
                ? new Vector2(1f, 1f)
                : new Vector2(0f, 1f);
        }

        private static Vector2 GetPivotFor(Vector2 anchoredPosition)
        {
            return anchoredPosition.x < 0f
                ? new Vector2(1f, 0.5f)
                : new Vector2(0f, 0.5f);
        }
    }
}
