namespace Mirage.Urbanization.Charts
{
    public static class ChartDrawerFactory
    {
        public static IChartDrawer Create() => new ZedGraphChartDrawer();
    }
}