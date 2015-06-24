using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Mirage.Urbanization.ZoneConsumption.Base
{
    public class ZoneClusterMemberConsumption : BaseZoneConsumption, IAreaObjectWithSeed
    {
        private readonly int _id = Random.Next(0, Int32.MaxValue);

        public override char KeyChar { get { throw new NotImplementedException(); } }

        public int Id { get { return _id; } }

        private static readonly Random Random = new Random();
        private readonly BaseZoneClusterConsumption _parentBaseZoneClusterConsumption;
        private readonly string _name;
        private readonly int _relativeToParentCenterX;
        private readonly int _relativeToParentCenterY;

        private readonly int _positionInClusterX, _positionInClusterY;

        public int PositionInClusterY
        {
            get { return _positionInClusterY; }
        }

        public int SingleCellCost
        {
            get
            {
                return ParentBaseZoneClusterConsumption.Cost / ParentBaseZoneClusterConsumption.ZoneClusterMembers.Count;
            }
        }

        public int PositionInClusterX
        {
            get { return _positionInClusterX; }
        }

        private readonly Color _color;

        public override Color Color { get { return _color; } }
        public int RelativeToParentCenterX { get { return _relativeToParentCenterX; } }
        public int RelativeToParentCenterY { get { return _relativeToParentCenterY; } }

        public override int Cost { get { throw new InvalidOperationException(); } }


        public bool IsCentralClusterMember
        {
            get { return RelativeToParentCenterX == 0 && RelativeToParentCenterY == 0; }
        }

        public bool IsMemberOf<T>()
            where T : BaseZoneClusterConsumption
        {
            return ParentBaseZoneClusterConsumption.GetType() == typeof(T);
        }

        public BaseZoneClusterConsumption ParentBaseZoneClusterConsumption { get { return _parentBaseZoneClusterConsumption; } }

        public QueryResult<BaseGrowthZoneClusterConsumption> QueryParentAsBaseGrowthZoneClusterConsumption()
        {
            return new QueryResult<BaseGrowthZoneClusterConsumption>(ParentBaseZoneClusterConsumption as BaseGrowthZoneClusterConsumption);
        }

        public T GetParentAs<T>() where T : BaseZoneClusterConsumption
        {
            var x = ParentBaseZoneClusterConsumption as T;
            if (x == null) throw new InvalidOperationException(String.Format("ParentBaseZoneClusterConsumption was not of type: \"{0}\", but was: \"{1}\".",
                typeof(T), ParentBaseZoneClusterConsumption != null ? ParentBaseZoneClusterConsumption.GetType() : null));
            return x;
        }

        public override string Name { get { return _name; } }

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
            if (name == null) throw new ArgumentNullException("name");
            if (parentBaseZoneClusterConsumption == null) throw new ArgumentNullException("parentBaseZoneClusterConsumption");

            _parentBaseZoneClusterConsumption = parentBaseZoneClusterConsumption;
            _name = name;
            _relativeToParentCenterX = relativeToParentCenterX;
            _relativeToParentCenterY = relativeToParentCenterY;
            _color = color;
            _positionInClusterX = positionInClusterX;
            _positionInClusterY = positionInClusterY;

            _zoneInfoFinder = createZoneInfoFinderFunc();
        }

        public static IEnumerable<ZoneClusterMemberConsumption> Generate(BaseZoneClusterConsumption parent, Func<ZoneInfoFinder> createZoneInfoFinderFunc, int widthInZones, int heightInZones, Color color)
        {
            if (widthInZones <= 1)
                throw new ArgumentOutOfRangeException("widthInZones");
            if (heightInZones <= 1)
                throw new ArgumentOutOfRangeException("heightInZones");

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
    }
}