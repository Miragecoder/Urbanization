using System;
using System.Collections.Generic;
using System.Linq;
using Mirage.Urbanization.ZoneConsumption;
using Mirage.Urbanization.ZoneStatisticsQuerying;

namespace Mirage.Urbanization.Vehicles
{
    internal class AirplaneController : BaseStructureVehicleController<IAirplane, AirportZoneClusterConsumption>
    {
        public AirplaneController(Func<ISet<IZoneInfo>> getZoneInfosFunc)
            : base(getZoneInfosFunc)
        {

        }

        private static readonly Func<IZoneInfo, QueryResult<IZoneInfo, RelativeZoneInfoQuery>>[] Directions =
        {
            x => x.GetNorth(),
            x => x.GetSouth(),
            x => x.GetEast(),
            x => x.GetWest()
        };

        protected override void PrepareVehiclesWithStructures(AirportZoneClusterConsumption[] structures)
        {
            int desiredAmountOfPlanes = structures.Count() * 2;

            while (Vehicles.Count() < desiredAmountOfPlanes)
            {
                var spawnPoint = structures.OrderBy(x => Random.Next()).First();
                var centralCell = spawnPoint.ZoneClusterMembers.Single(y => y.IsCentralClusterMember);

                centralCell.GetZoneInfo().WithResultIfHasMatch(zoneInfo =>
                {
                    var steerDirection = Directions.OrderBy(d => Random.Next()).First();
                    var alternateDirection = zoneInfo.GetSteerDirections(steerDirection).OrderBy(x => Random.Next()).First();

                    AddVehicle(new Airplane(GetZoneInfosFunc, zoneInfo, steerDirection, alternateDirection, Random.Next(5, 10)));
                });
            }
        }

        private class Airplane : BaseVehicle, IAirplane
        {
            private readonly Func<IZoneInfo, QueryResult<IZoneInfo, RelativeZoneInfoQuery>> _directionQuery;
            private readonly Func<IZoneInfo, QueryResult<IZoneInfo, RelativeZoneInfoQuery>> _alternateDirectionQuery;
            private readonly int _alternateRate;
            private readonly IEnumerator<Func<IZoneInfo, QueryResult<IZoneInfo, RelativeZoneInfoQuery>>> _directionEnumerator;

            public Airplane(Func<ISet<IZoneInfo>> getZoneInfosFunc, IZoneInfo currentPosition,
                Func<IZoneInfo, QueryResult<IZoneInfo, RelativeZoneInfoQuery>> directionQuery,
                Func<IZoneInfo, QueryResult<IZoneInfo, RelativeZoneInfoQuery>> alternateDirectionQuery,
                int alternateRate
                )
                : base(getZoneInfosFunc, currentPosition)
            {
                _directionQuery = directionQuery;
                _alternateDirectionQuery = alternateDirectionQuery;
                _alternateRate = alternateRate;
                _directionEnumerator = DirectionQuery().GetEnumerator();
            }

            private IEnumerable<Func<IZoneInfo, QueryResult<IZoneInfo, RelativeZoneInfoQuery>>> DirectionQuery()
            {
                while (true)
                {
                    foreach (var iteration in Enumerable.Range(0, _alternateRate))
                        yield return _directionQuery;
                    yield return _alternateDirectionQuery;
                }
            }

            protected override int SpeedInMilliseconds => 200;

            public void Move()
            {
                IfMustBeMoved(() =>
                {
                    _directionEnumerator.MoveNext();
                    var result = _directionEnumerator.Current(CurrentPosition);
                    Move(result.MatchingObject);
                });
            }
        }
    }
}