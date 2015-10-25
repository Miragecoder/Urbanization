namespace Mirage.Urbanization.Charts
{
    public static class ChartDrawerFactory
    {
        public static IChartDrawer Create()
        {
            if (RuntimeInspection.IsRunningOnMono())
            {
                
            }
            return new DataVisualizationChartDrawer();
        }
    }
}