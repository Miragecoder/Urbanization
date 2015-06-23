using System;
using System.Globalization;

namespace Mirage.Urbanization.Simulation
{
    public class YearAndMonthWeekElapsedEventArgs : EventArgsWithData<IYearAndMonth>
    {
        public YearAndMonthWeekElapsedEventArgs(IYearAndMonth data)
            : base(data)
        {
            
        }

    }

    internal sealed class YearAndMonth : IYearAndMonth
    {
        private int _currentYear = 1900;
        private int _currentMonth = 1;
        private int _currentWeek = 1;

        public string GetCurrentDescription() { return CurrentMonth + ' ' + _currentYear + " (Week: " + _currentWeek + ")"; }

        public bool IsAtBeginningOfNewYear
        {
            get { return IsAtBeginningOfMonth && _currentWeek == 1; }
        }

        public bool IsAtBeginningOfMonth
        {
            get { return _currentMonth == 1; }
        }

        public int TimeCode
        {
            get
            {
                return (_currentYear * 10000) +
                       (_currentMonth * 100) +
                       _currentWeek;
            }
        }

        public void LoadTimeCode(int timeCode)
        {
            if (timeCode < TimeCode) throw new ArgumentNullException(String.Format("Could not recognize the value {0} as a time code.", timeCode), "timeCode");
            var timeCodeAsString = timeCode.ToString(CultureInfo.InvariantCulture);

            _currentYear = Convert.ToInt32(timeCodeAsString.Substring(0, 4));
            _currentMonth = Convert.ToInt32(timeCodeAsString.Substring(4, 2));
            _currentWeek = Convert.ToInt32(timeCodeAsString.Substring(6, 2));
        }

        private string CurrentMonth
        {
            get
            {
                return CultureInfo.CurrentUICulture.DateTimeFormat.GetMonthName(_currentMonth);
            }
        }

        public void AddWeek()
        {
            if (_currentWeek == 4)
            {
                _currentWeek = 1;
                AddMonth();
            }
            else if (_currentWeek >= 1 && _currentWeek < 4)
            {
                _currentWeek++;
            }
            else
            {
                throw new InvalidOperationException();
            }
            RaiseOnWeekElapsed();
        }

        private void AddMonth()
        {
            if (_currentMonth == 12)
            {
                _currentMonth = 1;
                _currentYear++;
            }
            else if (_currentMonth >= 1 && _currentMonth < 12)
            {
                _currentMonth++;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        private void RaiseOnWeekElapsed()
        {
            var onWeekElapsed = OnWeekElapsed;
            if (onWeekElapsed != null)
                onWeekElapsed(this, new YearAndMonthWeekElapsedEventArgs(this));
        }

        public event EventHandler<YearAndMonthWeekElapsedEventArgs> OnWeekElapsed;
    }
}