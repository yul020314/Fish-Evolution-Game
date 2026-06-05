namespace FishEvolution.Tutorial
{
    public readonly struct TutorialStepChangedEvent
    {
        public TutorialStepChangedEvent(GuideStep step)
        {
            Step = step;
        }

        public GuideStep Step { get; }
    }
}
