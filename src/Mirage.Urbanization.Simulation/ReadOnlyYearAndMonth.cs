using System;
using System.Globalization;

namespace Mirage.Urbanization.Simulation
{
    internal class ReadOnlyYearAndMonth : IReadOnlyYearAndMonth
    {
        protected int CurrentYearProtected = 1900;
        protected int CurrentMonth = 1;
        protected int CurrentWeek = 1;

        public int CurrentYear => CurrentYearProtected;

        public string GetCurrentDescription() { return CurrentMonthDescription + ' ' + CurrentYear + " (Week: " + CurrentWeek + ")"; }

        private string CurrentMonthDescription => CultureInfo.CurrentUICulture.DateTimeFormat.GetMonthName(CurrentMonth);

        public bool IsAtBeginningOfNewYear => IsAtBeginningOfMonth && CurrentWeek == 1;

        public bool IsAtBeginningOfMonth => CurrentMonth == 1;

        public int TimeCode => (CurrentYear * 10000) +
                               (CurrentMonth * 100) +
                               CurrentWeek;

        protected ReadOnlyYearAndMonth() { }

        public ReadOnlyYearAndMonth(int timeCode)
        {
            LoadTimeCodeProtected(timeCode);
        }

        protected void LoadTimeCodeProtected(int timeCode)
        {
            if (timeCode < TimeCode) throw new ArgumentNullException(String.Format("Could not recognize the value {0} as a time code.", timeCode), "timeCode");
            var timeCodeAsString = timeCode.ToString(CultureInfo.InvariantCulture);

            CurrentYearProtected = Convert.ToInt32(timeCodeAsString.Substring(0, 4));
            CurrentMonth = Convert.ToInt32(timeCodeAsString.Substring(4, 2));
            CurrentWeek = Convert.ToInt32(timeCodeAsString.Substring(6, 2));
        }
    }
}