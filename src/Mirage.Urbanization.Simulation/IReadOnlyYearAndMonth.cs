namespace Mirage.Urbanization.Simulation
{
    public interface IReadOnlyYearAndMonth
    {
        bool IsAtBeginningOfNewYear { get; }
        string GetCurrentDescription();
        int CurrentYear { get; }
    }
}