using UnityEngine;

namespace FishEvolution.Config
{
    [CreateAssetMenu(fileName = "FoodData", menuName = "Fish Evolution/Config/Food Data")]
    public sealed class FoodDataSO : ScriptableObject
    {
        [SerializeField] private string _foodId = string.Empty;
        [SerializeField] private string _foodName = string.Empty;
        [SerializeField] private int _level = 1;
        [SerializeField] private int _experience = 1;
        [SerializeField] private float _scale = 0.2f;
        [SerializeField] private Color _color = Color.white;

        public string FoodId => _foodId;
        public string FoodName => _foodName;
        public int Level => _level;
        public int Experience => _experience;
        public float Scale => _scale;
        public Color Color => _color;
    }
}
