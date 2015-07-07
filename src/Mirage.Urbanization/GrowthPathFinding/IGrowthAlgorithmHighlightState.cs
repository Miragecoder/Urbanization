namespace Mirage.Urbanization.GrowthPathFinding
{
    public interface IGrowthAlgorithmHighlightState
    {
        HighlightState Current { get; }
        void SetState(HighlightState highlightState);
    }
}