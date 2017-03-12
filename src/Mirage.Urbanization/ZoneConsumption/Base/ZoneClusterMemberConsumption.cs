using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Mirage.Urbanization.ZoneStatisticsQuerying;

namespace Mirage.Urbanization.ZoneConsumption.Base
{
    public class ZoneClusterMemberConsumption : BaseZoneConsumption, IAreaObjectWithSeed, IAreaObjectWithPopulationDensity
    {
        public override char KeyChar { get { throw new NotImplementedException(); } }

        public override BuildStyle BuildStyle { get { throw new NotImplementedException(); } }

        public int Id { get; } = Random.Next(0, Int32.MaxValue);

        private static readonly Random Random = new Random();

        public int PositionInClusterY { get; }

        public int SingleCellCost => ParentBaseZoneClusterConsumption.Cost / ParentBaseZoneClusterConsumption.ZoneClusterMembers.Count;

        public int PositionInClusterX { get; }

        public override Color Color { get; }
        public int RelativeToParentCenterX { get; }
        public int RelativeToParentCenterY { get; }

        public override int Cost { get { throw new InvalidOperationException(); } }


        public bool IsCentralClusterMember => RelativeToParentCenterX == 0 && RelativeToParentCenterY == 0;

        public bool IsMemberOf<T>()
            where T : BaseZoneClusterConsumption
        {
            return ParentBaseZoneClusterConsumption.GetType() == typeof(T);
        }

        public BaseZoneClusterConsumption ParentBaseZoneClusterConsumption { get; }

        public QueryResult<BaseGrowthZoneClusterConsumption> QueryParentAsBaseGrowthZoneClusterConsumption()
        {
            return QueryResult<BaseGrowthZoneClusterConsumption>.Create(ParentBaseZoneClusterConsumption as BaseGrowthZoneClusterConsumption);
        }

        public T GetParentAs<T>() where T : BaseZoneClusterConsumption
        {
            var x = ParentBaseZoneClusterConsumption as T;
            if (x == null) throw new InvalidOperationException(
                $"ParentBaseZoneClusterConsumption was not of type: \"{typeof(T)}\", but was: \"{ParentBaseZoneClusterConsumption?.GetType()}\"."
            );
            return x;
        }

        public override string Name { get; }

        public override IGetCanOverrideWithResult GetCanOverrideWith(IAreaZoneConsumption consumption)
        {
            if (ParentBaseZoneClusterConsumption.ClusterMembersAreUnlocked)
            {
                return new AreaZoneConsumptionOverrideInfoResult(consumption, consumption);
            }

            return new AreaZoneConsumptionOverrideInfoResult(
                resultingAreaConsumption: this,
                toBeDeployedAreaConsumption: consumption
            );
        }

        private readonly ZoneInfoFinder _zoneInfoFinder;

        public QueryResult<IZoneInfo> GetZoneInfo()
        {
            return _zoneInfoFinder.GetZoneInfoFor(this);
        }

        public ZoneClusterMemberConsumption(
            BaseZoneClusterConsumption parentBaseZoneClusterConsumption,
            Func<ZoneInfoFinder> createZoneInfoFinderFunc,
            string name,
            int relativeToParentCenterX,
            int relativeToParentCenterY,
            Color color,
            int positionInClusterX,
            int positionInClusterY
        )
        {
            ParentBaseZoneClusterConsumption = parentBaseZoneClusterConsumption ?? throw new ArgumentNullException(nameof(parentBaseZoneClusterConsumption));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            RelativeToParentCenterX = relativeToParentCenterX;
            RelativeToParentCenterY = relativeToParentCenterY;
            Color = color;
            PositionInClusterX = positionInClusterX;
            PositionInClusterY = positionInClusterY;

            _zoneInfoFinder = createZoneInfoFinderFunc();
            PositionInCluster = new Point(PositionInClusterX - 1, PositionInClusterY - 1);
        }

        public Point PositionInCluster { get; }

        public static IEnumerable<ZoneClusterMemberConsumption> Generate(BaseZoneClusterConsumption parent, Func<ZoneInfoFinder> createZoneInfoFinderFunc, int widthInZones, int heightInZones, Color color)
        {
            if (widthInZones <= 1)
                throw new ArgumentOutOfRangeException(nameof(widthInZones));
            if (heightInZones <= 1)
                throw new ArgumentOutOfRangeException(nameof(heightInZones));

            int zeroBasedWidth = widthInZones - 1;
            int zeroBasedHeight = heightInZones - 1;

            int widthOffset = zeroBasedWidth / 2;
            int heightOffset = zeroBasedHeight / 2;

            return
                from widthIndex in Enumerable.Range(0, widthInZones)
                from heightIndex in Enumerable.Range(0, heightInZones)
                select
                    new ZoneClusterMemberConsumption(
                        createZoneInfoFinderFunc: createZoneInfoFinderFunc,
                        parentBaseZoneClusterConsumption: parent,
                        name: parent.Name,
                        relativeToParentCenterX: (widthIndex - widthOffset),
                        relativeToParentCenterY: (heightIndex - heightOffset),
                        positionInClusterX: widthIndex + 1,
                        positionInClusterY: heightIndex + 1,
                        color: color
                    );
        }

        public int PopulationDensity => GetParentAs<BaseGrowthZoneClusterConsumption>().PopulationDensity;
    }
}