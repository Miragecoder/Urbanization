using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mirage.Urbanization.ZoneStatisticsQuerying;

namespace Mirage.Urbanization
{
    public interface ILandValueCalculator
    {
        QueryResult<IQueryLandValueResult> GetFor(IReadOnlyZoneInfo zoneInfo);
    }
}
