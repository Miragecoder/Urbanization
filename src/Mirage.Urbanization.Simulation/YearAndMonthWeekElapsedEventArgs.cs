namespace Mirage.Urbanization.Simulation
{
    public class YearAndMonthWeekElapsedEventArgs : EventArgsWithData<IYearAndMonth>
    {
        public YearAndMonthWeekElapsedEventArgs(IYearAndMonth data)
            : base(data)
        {
            
        }

    }
}