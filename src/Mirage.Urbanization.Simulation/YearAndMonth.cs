using System;

namespace Mirage.Urbanization.Simulation
{
    internal sealed class YearAndMonth : ReadOnlyYearAndMonth, IYearAndMonth
    {
        public void LoadTimeCode(int timeCode)
        {
            LoadTimeCodeProtected(timeCode);
        }

        public void AddWeek()
        {
            if (CurrentWeek == 4)
            {
                CurrentWeek = 1;
                AddMonth();
            }
            else if (CurrentWeek >= 1 && CurrentWeek < 4)
            {
                CurrentWeek++;
            }
            else
            {
                throw new InvalidOperationException();
            }
            RaiseOnWeekElapsed();
        }

        private void AddMonth()
        {
            if (CurrentMonth == 12)
            {
                CurrentMonth = 1;
                CurrentYearProtected++;
            }
            else if (CurrentMonth >= 1 && CurrentMonth < 12)
            {
                CurrentMonth++;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        private void RaiseOnWeekElapsed()
        {
            OnWeekElapsed?.Invoke(this, new YearAndMonthWeekElapsedEventArgs(this));
        }

        public event EventHandler<YearAndMonthWeekElapsedEventArgs> OnWeekElapsed;
    }
}