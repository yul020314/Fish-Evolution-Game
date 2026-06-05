using UnityEngine;
using UnityEngine.UI;

namespace FishEvolution.UI
{
    public sealed class HudProgressBar
    {
        private readonly Image _fillImage;

        public HudProgressBar(Image fillImage)
        {
            _fillImage = fillImage;
        }

        public void SetValue(
            int currentValue,
            int maxValue)
        {
            if (_fillImage == null)
            {
                return;
            }

            _fillImage.fillAmount = GetFillAmount(currentValue, maxValue);
        }

        private static float GetFillAmount(
            int currentValue,
            int maxValue)
        {
            if (maxValue <= 0)
            {
                return 1f;
            }

            return Mathf.Clamp01((float)currentValue / maxValue);
        }
    }
}
