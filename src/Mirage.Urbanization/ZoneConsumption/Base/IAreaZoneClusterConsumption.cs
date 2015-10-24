using System.Collections.Generic;

namespace Mirage.Urbanization.ZoneConsumption.Base
{
    public interface IAreaZoneClusterConsumption : IAreaConsumption
    {
        IReadOnlyCollection<ZoneClusterMemberConsumption> ZoneClusterMembers { get; }

        int WidthInCells { get; }
        int HeightInCells { get; }
        int HorizontalCellOffset { get; }
        int VerticalCellOffset { get; }
    }
}