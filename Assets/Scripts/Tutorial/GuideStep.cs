using System;
using UnityEngine;

namespace FishEvolution.Tutorial
{
    [Serializable]
    public sealed class GuideStep
    {
        [SerializeField] private string _stepId = string.Empty;
        [SerializeField] private GuideStepType _stepType = GuideStepType.Move;
        [SerializeField] private string _message = string.Empty;
        [SerializeField] private Vector2 _highlightPosition = Vector2.zero;
        [SerializeField] private Vector2 _highlightSize = new Vector2(220f, 96f);

        public string StepId => _stepId;
        public GuideStepType StepType => _stepType;
        public string Message => _message;
        public Vector2 HighlightPosition => _highlightPosition;
        public Vector2 HighlightSize => _highlightSize;
    }
}
