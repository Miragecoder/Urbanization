namespace Mirage.Urbanization
{
    public interface IGrowthAlgorithmHighlightState
    {
        HighlightState Current { get; }
        void SetState(HighlightState highlightState);
    }
}