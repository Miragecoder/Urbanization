using System;

namespace Mirage.Urbanization.Simulation
{
    public interface IYearAndMonth : IReadOnlyYearAndMonth
    {
        event EventHandler<YearAndMonthWeekElapsedEventArgs> OnWeekElapsed;
    }
}