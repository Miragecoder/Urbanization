using System;

namespace Mirage.Urbanization.Simulation
{
    public interface IYearAndMonth
    {
        bool IsAtBeginningOfNewYear { get; }
        string GetCurrentDescription();
        event EventHandler<YearAndMonthWeekElapsedEventArgs> OnWeekElapsed;
    }
}