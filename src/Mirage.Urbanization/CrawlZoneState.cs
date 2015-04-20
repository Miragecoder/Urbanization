namespace Mirage.Urbanization
{
    public class CrawlZoneState
    {
        private readonly int _currentDistance;
        private readonly IZoneInfo _currentZoneInfo;

        public CrawlZoneState(int currentDistance, IZoneInfo currentZoneInfo)
        {
            _currentDistance = currentDistance;
            _currentZoneInfo = currentZoneInfo;
        }

        public IZoneInfo CurrentZoneInfo
        {
            get { return _currentZoneInfo; }
        }

        public int CurrentDistance
        {
            get { return _currentDistance; }
        }
    }
}