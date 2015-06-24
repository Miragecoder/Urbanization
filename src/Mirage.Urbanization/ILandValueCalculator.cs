using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirage.Urbanization
{
    public interface ILandValueCalculator
    {
        QueryResult<IQueryLandValueResult> GetFor(IReadOnlyZoneInfo zoneInfo);
    }
}
