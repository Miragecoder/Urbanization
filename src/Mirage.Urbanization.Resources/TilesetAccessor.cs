using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using Mirage.Urbanization.Vehicles;
using Mirage.Urbanization.ZoneConsumption;
using Mirage.Urbanization.ZoneConsumption.Base;
using Mirage.Urbanization.ZoneStatisticsQuerying;

namespace Mirage.Urbanization.Tilesets
{
    public class TilesetAccessor : ITilesetAccessor
    {
        public const int DefaultTileWidthAndSizeInPixels = 25;

        public int TileWidthAndSizeInPixels { get; set; } = DefaultTileWidthAndSizeInPixels;

        public Size ResizeToTileWidthAndSize(Size size)
        {
            decimal resizeMultiplier = (decimal)TileWidthAndSizeInPixels / 25;
            return new Size(
                Convert.ToInt32(Math.Round(size.Width * resizeMultiplier)),
                Convert.ToInt32(Math.Round(size.Height * resizeMultiplier))
                );
        }

        private readonly INetworkZoneTileset _woodNetworkZoneTileset;

        private readonly INetworkZoneTileset _parkNetworkZoneTileset;

        private readonly INetworkZoneTileset _railNetworkZoneTileset;

        private readonly INetworkZoneTileset _powerNetworkZoneTileset;

        private readonly INetworkZoneTileset _waterNetworkZoneTileset;

        private readonly BitmapAccessor _bitmapAccessor = new BitmapAccessor();

        public QueryResult<BitmapLayer> TryGetBitmapFor(IAreaZoneConsumption consumption)
        {
            if (consumption is RubbishZoneConsumption)
            {
                return QueryResult<BitmapLayer>.Create(new BitmapLayer(_bitmapAccessor.Rubbish.ToBitmapInfo()));
            }
            else if (consumption is BaseNetworkZoneConsumption)
            {
                BitmapInfo bitmapOne;
                var networkConsumption = consumption as BaseNetworkZoneConsumption;

                if (consumption is BaseInfrastructureNetworkZoneConsumption)
                {
                    if (consumption is RoadZoneConsumption)
                    {
                        bitmapOne =
                            _bitmapAccessor.NetworkZonesInstance.RoadInstance.GetBitmapInfoFor(
                                networkConsumption as RoadZoneConsumption);
                    }
                    else if (consumption is RailRoadZoneConsumption)
                    {
                        bitmapOne = _railNetworkZoneTileset.GetBitmapInfoFor(networkConsumption);
                    }
                    else if (consumption is PowerLineConsumption)
                    {
                        bitmapOne = _powerNetworkZoneTileset.GetBitmapInfoFor(networkConsumption);
                    }
                    else if (consumption is WaterZoneConsumption)
                    {
                        bitmapOne = _waterNetworkZoneTileset.GetBitmapInfoFor(networkConsumption);
                    }
                    else throw new InvalidOperationException();
                }
                else if (consumption is WoodlandZoneConsumption)
                {
                    bitmapOne = _woodNetworkZoneTileset.GetBitmapInfoFor(networkConsumption);
                }
                else if (consumption is ParkZoneConsumption)
                {
                    bitmapOne = _parkNetworkZoneTileset.GetBitmapInfoFor(networkConsumption);
                }
                else throw new InvalidOperationException();

                return QueryResult<BitmapLayer>.Create(new BitmapLayer(bitmapOne));
            }
            else if (consumption is IIntersectingZoneConsumption)
            {
                var intersection = consumption as IIntersectingZoneConsumption;

                BitmapInfo bitmapOne, bitmapTwo;

                if (intersection.NorthSouthZoneConsumption is RoadZoneConsumption ||
                    intersection.EastWestZoneConsumption is RoadZoneConsumption)
                {
                    return QueryResult<BitmapLayer>.Create(_bitmapAccessor.NetworkZonesInstance.RoadInstance.GetBitmapLayerFor(intersection));
                }
                else if (intersection.NorthSouthZoneConsumption is RailRoadZoneConsumption
                         && intersection.EastWestZoneConsumption is PowerLineConsumption)
                {
                    return _bitmapAccessor.NetworkZonesInstance.RailNorthSouthPowerEastWest.ToBitmapInfo().ToBitmapLayer().ToQueryResult();
                }
                else if (intersection.NorthSouthZoneConsumption is PowerLineConsumption
                         && intersection.EastWestZoneConsumption is RailRoadZoneConsumption)
                {
                    return _bitmapAccessor.NetworkZonesInstance.PowerNorthSouthRailEastWest.Value.ToBitmapInfo().ToBitmapLayer().ToQueryResult();
                }
                else if (intersection.NorthSouthZoneConsumption is PowerLineConsumption
                         && intersection.EastWestZoneConsumption is WaterZoneConsumption)
                {
                    bitmapOne =
                        _waterNetworkZoneTileset.GetBitmapInfoFor(
                            intersection.GetZoneConsumptionOfType<WaterZoneConsumption>());
                    bitmapTwo = _bitmapAccessor.NetworkZonesInstance.PowerNorthSouthWaterEastWest.ToBitmapInfo();
                }
                else if (intersection.NorthSouthZoneConsumption is WaterZoneConsumption
                         && intersection.EastWestZoneConsumption is PowerLineConsumption)
                {
                    bitmapOne =
                        _waterNetworkZoneTileset.GetBitmapInfoFor(
                            intersection.GetZoneConsumptionOfType<WaterZoneConsumption>());
                    bitmapTwo = _bitmapAccessor.NetworkZonesInstance.WaterNorthSouthPowerEastWest.Value.ToBitmapInfo();
                }
                else if (intersection.NorthSouthZoneConsumption is RailRoadZoneConsumption
                         && intersection.EastWestZoneConsumption is WaterZoneConsumption)
                {
                    bitmapOne =
                        _waterNetworkZoneTileset.GetBitmapInfoFor(
                            intersection.GetZoneConsumptionOfType<WaterZoneConsumption>());
                    bitmapTwo = _bitmapAccessor.NetworkZonesInstance.RailNorthSouthWaterEastWest.ToBitmapInfo();
                }
                else if (intersection.NorthSouthZoneConsumption is WaterZoneConsumption
                         && intersection.EastWestZoneConsumption is RailRoadZoneConsumption)
                {
                    bitmapOne =
                        _waterNetworkZoneTileset.GetBitmapInfoFor(
                            intersection.GetZoneConsumptionOfType<WaterZoneConsumption>());
                    bitmapTwo = _bitmapAccessor.NetworkZonesInstance.WaterNorthSouthRailEastWest.Value.ToBitmapInfo();
                }
                else throw new InvalidOperationException();

                return new BitmapLayer(bitmapOne, bitmapTwo).ToQueryResult();
            }
            else if (consumption is ZoneClusterMemberConsumption)
            {
                var zoneClusterMemberConsumption = consumption as ZoneClusterMemberConsumption;
                var parentConsumption = (consumption as ZoneClusterMemberConsumption).ParentBaseZoneClusterConsumption;

                if (parentConsumption is BaseImplementedZoneClusterConsumption)
                {
                    var showHasNoElectricitySymbol = !(parentConsumption as BaseImplementedZoneClusterConsumption).HasPower;

                    if (zoneClusterMemberConsumption.IsCentralClusterMember && showHasNoElectricitySymbol &&
                        DateTime.Now.Millisecond > 500)
                    {
                        return _bitmapAccessor
                            .GrowthZonesInstance
                            .NoElectricity
                            .ToBitmapInfo()
                            .ToBitmapLayer()
                            .ToQueryResult();
                    }

                    if (parentConsumption is CoalPowerPlantZoneClusterConsumption)
                    {
                        return _bitmapAccessor.PowerPlant.GetBitmapLayer(zoneClusterMemberConsumption).ToQueryResult();
                    }
                    else if (parentConsumption is NuclearPowerPlantZoneClusterConsumption)
                    {
                        return _bitmapAccessor.NuclearPowerplant.GetBitmapLayerFor(zoneClusterMemberConsumption).ToQueryResult();
                    }
                    else if (parentConsumption is PoliceStationZoneClusterConsumption)
                    {
                        return _bitmapAccessor.Police.GetBitmapLayerFor(zoneClusterMemberConsumption).ToQueryResult();
                    }
                    else if (parentConsumption is FireStationZoneclusterConsumption)
                    {
                        return _bitmapAccessor.FireStation.GetBitmapLayerFor(zoneClusterMemberConsumption).ToQueryResult();
                    }
                    else if (parentConsumption is TrainStationZoneClusterConsumption)
                    {
                        return _bitmapAccessor.TrainStation.GetBitmapLayerFor(zoneClusterMemberConsumption).ToQueryResult();
                    }
                    else if (parentConsumption is AirportZoneClusterConsumption)
                    {
                        return _bitmapAccessor.Airport.GetBitmapLayerFor(zoneClusterMemberConsumption).ToQueryResult();
                    }
                    else if (parentConsumption is SeaPortZoneClusterConsumption)
                    {
                        return _bitmapAccessor.SeaPort.GetBitmapLayerFor(zoneClusterMemberConsumption).ToQueryResult();
                    }
                    else if (parentConsumption is StadiumZoneClusterConsumption)
                    {
                        return _bitmapAccessor.Stadium.GetBitmapLayerFor(zoneClusterMemberConsumption).ToQueryResult();
                    }
                    else if (parentConsumption is BaseGrowthZoneClusterConsumption)
                    {
                        var baseGrowthZoneConsumption = parentConsumption as BaseGrowthZoneClusterConsumption;
                        if (baseGrowthZoneConsumption is ResidentialZoneClusterConsumption)
                        {
                            var residentialZoneConsumption =
                                baseGrowthZoneConsumption as ResidentialZoneClusterConsumption;

                            if (residentialZoneConsumption.RenderAsHouse(zoneClusterMemberConsumption))
                            {
                                return BitmapSelectorCollectionsInstance.ResidentialHouseCollection.GetBitmapFor(
                                    zoneClusterMemberConsumption).GetBitmap(1,1)
                                    .ToBitmapInfo()
                                    .ToBitmapLayer()
                                    .ToQueryResult();
                            }

                            return BitmapSelectorCollectionsInstance
                                .ResidentialCollection
                                .GetBitmapFor(baseGrowthZoneConsumption)
                                .GetBitmapLayerFor(zoneClusterMemberConsumption)
                                .ToQueryResult();
                        }
                        else if (baseGrowthZoneConsumption is CommercialZoneClusterConsumption)
                        {
                            return BitmapSelectorCollectionsInstance
                                .CommercialCollection
                                .GetBitmapFor(baseGrowthZoneConsumption)
                                .GetBitmapLayerFor(zoneClusterMemberConsumption)
                                .ToQueryResult();
                        }
                        else if (baseGrowthZoneConsumption is IndustrialZoneClusterConsumption)
                        {
                            return BitmapSelectorCollectionsInstance
                                .IndustrialCollection
                                .GetBitmapFor(baseGrowthZoneConsumption)
                                .GetBitmapLayerFor(zoneClusterMemberConsumption)
                                .ToQueryResult();
                        }
                        else
                        {
                            throw new InvalidOperationException();
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
            else
            {
                return QueryResult<BitmapLayer>.Empty;
            }
        }

        internal BitmapSelectorCollections BitmapSelectorCollectionsInstance = new BitmapSelectorCollections();

        public TilesetAccessor()
        {
            _waterNetworkZoneTileset = new NetworkZoneTileset(
                bitmapEastWest: _bitmapAccessor.NetworkZonesInstance.WaterInstance.WaterEastWest,
                bitmapNorthWest: _bitmapAccessor.NetworkZonesInstance.WaterInstance.WaterNorthWest,
                bitmapWestNorthEast: _bitmapAccessor.NetworkZonesInstance.WaterInstance.WaterWestNorthEast,
                bitmapNorthEastSouthWest: _bitmapAccessor.NetworkZonesInstance.WaterInstance.WaterNorthWestEastSouth,
                bitmapEast: _bitmapAccessor.NetworkZonesInstance.WaterInstance.WaterEast,
                bitmapNoDirection: _bitmapAccessor.NetworkZonesInstance.WaterInstance.WaterNoDirection
                );
            _powerNetworkZoneTileset = new NetworkZoneTileset(
                bitmapEastWest: _bitmapAccessor.NetworkZonesInstance.PowerInstance.PowerEastWest,
                bitmapNorthWest: _bitmapAccessor.NetworkZonesInstance.PowerInstance.PowerNorthWest,
                bitmapWestNorthEast: _bitmapAccessor.NetworkZonesInstance.PowerInstance.PowerWestNorthEast,
                bitmapNorthEastSouthWest: _bitmapAccessor.NetworkZonesInstance.PowerInstance.PowerNorthWestEastSouth,
                bitmapEast: _bitmapAccessor.NetworkZonesInstance.PowerInstance.PowerEast,
                bitmapNoDirection: _bitmapAccessor.NetworkZonesInstance.PowerInstance.PowerNoDirection
                );
            _railNetworkZoneTileset = new NetworkZoneTileset(
                bitmapEastWest: _bitmapAccessor.NetworkZonesInstance.RailInstance.RailEastWest,
                bitmapNorthWest: _bitmapAccessor.NetworkZonesInstance.RailInstance.RailNorthWest,
                bitmapWestNorthEast: _bitmapAccessor.NetworkZonesInstance.RailInstance.RailWestNorthEast,
                bitmapNorthEastSouthWest: _bitmapAccessor.NetworkZonesInstance.RailInstance.RailNorthWestEastSouth,
                bitmapEast: _bitmapAccessor.NetworkZonesInstance.RailInstance.RailEast,
                bitmapNoDirection: _bitmapAccessor.NetworkZonesInstance.RailInstance.RailNoDirection
                );
            _parkNetworkZoneTileset = new NetworkZoneTileset(
                bitmapEastWest: _bitmapAccessor.NetworkZonesInstance.ParkInstance.WoodEastWest,
                bitmapNorthWest: _bitmapAccessor.NetworkZonesInstance.ParkInstance.WoodNorthWest,
                bitmapWestNorthEast: _bitmapAccessor.NetworkZonesInstance.ParkInstance.WoodWestNorthEast,
                bitmapNorthEastSouthWest: _bitmapAccessor.NetworkZonesInstance.ParkInstance.WoodNorthWestEastSouth,
                bitmapEast: _bitmapAccessor.NetworkZonesInstance.ParkInstance.WoodEast,
                bitmapNoDirection: _bitmapAccessor.NetworkZonesInstance.ParkInstance.WoodNoDirection
                );
            _woodNetworkZoneTileset = new NetworkZoneTileset(
                bitmapEastWest: _bitmapAccessor.NetworkZonesInstance.WoodInstance.WoodEastWest,
                bitmapNorthWest: _bitmapAccessor.NetworkZonesInstance.WoodInstance.WoodNorthWest,
                bitmapWestNorthEast: _bitmapAccessor.NetworkZonesInstance.WoodInstance.WoodWestNorthEast,
                bitmapNorthEastSouthWest: _bitmapAccessor.NetworkZonesInstance.WoodInstance.WoodNorthWestEastSouth,
                bitmapEast: _bitmapAccessor.NetworkZonesInstance.WoodInstance.WoodEast,
                bitmapNoDirection: _bitmapAccessor.NetworkZonesInstance.WoodInstance.WoodNoDirection
                );
        }

        public Size GetAreaSize(IReadOnlyArea area)
        {
            return new Size(
                width: area.AmountOfZonesX * TileWidthAndSizeInPixels,
                height: area.AmountOfZonesY * TileWidthAndSizeInPixels
                );
        }

        public IEnumerable<VehicleBitmapAndPoint> GetBitmapsAndPointsFor(IMoveableVehicle vehicle,
            MiscBitmaps miscBitmapsInstance)
        {

            if (vehicle.PreviousPreviousPreviousPreviousPosition == null)
                yield break;

            foreach (var pair in new[]
            {
                new
                {
                    Render = (vehicle is ITrain),
                    First = vehicle.CurrentPosition,
                    Second = vehicle.PreviousPosition,
                    Third = vehicle.PreviousPreviousPosition,
                    Head = true
                },
                new
                {
                    Render = true,
                    First = vehicle.PreviousPosition,
                    Second = vehicle.PreviousPreviousPosition,
                    Third = vehicle.PreviousPreviousPreviousPosition,
                    Head = false
                },
                new
                {
                    Render = (vehicle is ITrain),
                    First = vehicle.PreviousPreviousPosition,
                    Second = vehicle.PreviousPreviousPreviousPosition,
                    Third = vehicle.PreviousPreviousPreviousPreviousPosition,
                    Head = false
                }
            })
            {
                var orientation = (pair.Third.Point != pair.First.Point)
                    ? pair.Third.Point.OrientationTo(pair.First.Point)
                    : pair.Second.Point.OrientationTo(pair.First.Point);

                Bitmap bitmap;

                if (vehicle is IAirplane)
                    bitmap = miscBitmapsInstance.Plane.GetBitmap(orientation);
                else if (vehicle is ITrain)
                    bitmap = miscBitmapsInstance.Train.GetBitmap(orientation);
                else if (vehicle is IShip)
                    bitmap = miscBitmapsInstance.GetShipBitmapFrame().GetBitmap(orientation);
                else
                    throw new InvalidOperationException();

                if (pair.Render)
                {
                    yield return new VehicleBitmapAndPoint(bitmap, pair.Second, pair.Third, vehicle);
                }
            }
        }
    }
}