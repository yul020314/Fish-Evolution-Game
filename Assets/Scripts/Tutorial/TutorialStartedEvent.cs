namespace FishEvolution.Tutorial
{
    public readonly struct TutorialStartedEvent
    {
        public TutorialStartedEvent(GuideStep firstStep)
        {
            FirstStep = firstStep;
        }

        public GuideStep FirstStep { get; }
    }
}
