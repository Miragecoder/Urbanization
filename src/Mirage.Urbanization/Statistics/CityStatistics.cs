using System;

namespace Mirage.Urbanization.Statistics
{
    public class CityStatistics : ICityStatistics
    {
        private readonly int _timeCode;
        private readonly IPowerGridStatistics _powerGridStatistics;

        public IPowerGridStatistics PowerGridStatistics
        {
            get { return _powerGridStatistics; }
        }

        private readonly IGrowthZoneStatistics _growthZoneStatistics;
        private readonly IMiscCityStatistics _miscCityStatistics;

        public IGrowthZoneStatistics GrowthZoneStatistics
        {
            get { return _growthZoneStatistics; }
        }

        public IMiscCityStatistics MiscCityStatistics
        {
            get { return _miscCityStatistics; }
        }

        public CityStatistics(int timeCode, IPowerGridStatistics powerGridStatistics, IGrowthZoneStatistics growthZoneStatistics, IMiscCityStatistics miscCityStatistics)
        {
            if (powerGridStatistics == null) throw new ArgumentNullException("powerGridStatistics");
            if (growthZoneStatistics == null) throw new ArgumentNullException("growthZoneStatistics");

            _timeCode = timeCode;
            _powerGridStatistics = powerGridStatistics;
            _growthZoneStatistics = growthZoneStatistics;
            _miscCityStatistics = miscCityStatistics;
        }


        public int TimeCode
        {
            get { return _timeCode; }
        }
    }
}