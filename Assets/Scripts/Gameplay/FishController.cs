using FishEvolution.Config;
using UnityEngine;

namespace FishEvolution.Gameplay
{
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class FishController : MonoBehaviour
    {
        [SerializeField] private FishDataSO _fishData;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Collider2D _collider2D;
        [SerializeField] private Vector3 _baseScale = new Vector3(0.8f, 0.5f, 1f);
        [SerializeField] private Color _commonColor = new Color(0.3f, 0.86f, 1f, 1f);
        [SerializeField] private Color _rareColor = new Color(0.35f, 0.55f, 1f, 1f);
        [SerializeField] private Color _epicColor = new Color(0.75f, 0.35f, 1f, 1f);
        [SerializeField] private Color _legendaryColor = new Color(1f, 0.76f, 0.25f, 1f);
        [SerializeField] private Color _mythicColor = new Color(1f, 0.35f, 0.28f, 1f);

        public FishDataSO FishData => _fishData;

        private void Awake()
        {
            CacheComponents();
            _collider2D.isTrigger = true;
            ApplyFishData();
        }

        private void Reset()
        {
            CacheComponents();
            _baseScale = transform.localScale;
        }

        public void Initialize(FishDataSO fishData)
        {
            _fishData = fishData;
            ApplyFishData();
        }

        private void ApplyFishData()
        {
            if (_fishData == null)
            {
                return;
            }

            gameObject.name = _fishData.FishName;
            transform.localScale = _baseScale * Mathf.Max(0.01f, _fishData.Scale);
            _spriteRenderer.sprite = _fishData.Sprite != null
                ? _fishData.Sprite
                : _spriteRenderer.sprite;
            _spriteRenderer.color = _fishData.Sprite != null
                ? Color.white
                : GetRarityColor(_fishData.Rarity);
        }

        private Color GetRarityColor(FishRarity rarity)
        {
            switch (rarity)
            {
                case FishRarity.Rare:
                    return _rareColor;
                case FishRarity.Epic:
                    return _epicColor;
                case FishRarity.Legendary:
                    return _legendaryColor;
                case FishRarity.Mythic:
                    return _mythicColor;
                default:
                    return _commonColor;
            }
        }

        private void CacheComponents()
        {
            if (_spriteRenderer == null)
            {
                _spriteRenderer = GetComponent<SpriteRenderer>();
            }

            if (_collider2D == null)
            {
                _collider2D = GetComponent<Collider2D>();
            }

            if (_collider2D == null)
            {
                _collider2D = gameObject.AddComponent<BoxCollider2D>();
            }
        }
    }
}
