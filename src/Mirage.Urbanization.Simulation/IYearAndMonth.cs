using System;

namespace Mirage.Urbanization.Simulation
{
    public interface IReadOnlyYearAndMonth
    {
        bool IsAtBeginningOfNewYear { get; }
        string GetCurrentDescription();
        int CurrentYear { get; }
    }

    public interface IYearAndMonth : IReadOnlyYearAndMonth
    {
        event EventHandler<YearAndMonthWeekElapsedEventArgs> OnWeekElapsed;
    }
}