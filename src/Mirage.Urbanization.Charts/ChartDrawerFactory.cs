namespace Mirage.Urbanization.Charts
{
    public static class ChartDrawerFactory
    {
        public static IChartDrawer Create()
        {
            if (RuntimeInspection.IsRunningOnMono() || ForceZedGraph)
            {
                return new ZedGraphChartDrawer();
            }
            return new DataVisualizationChartDrawer();
        }

        public static bool ForceZedGraph { get; set; }
    }
}