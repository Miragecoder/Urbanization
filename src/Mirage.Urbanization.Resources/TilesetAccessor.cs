using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using Mirage.Urbanization.ZoneConsumption;
using Mirage.Urbanization.ZoneConsumption.Base;

namespace Mirage.Urbanization.Tilesets
{
    public class TilesetAccessor : ITilesetAccessor
    {
        private const int DefaultTileWidthAndSizeInPixels = 25;

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

        public bool TryGetBitmapFor(IAreaZoneConsumption consumption, out BitmapLayer bitmapLayer)
        {
            bitmapLayer = null;
            Bitmap bitmapOne = null, bitmapTwo = null;

            if (consumption is RubbishZoneConsumption)
            {
                bitmapOne = _bitmapAccessor.Rubbish;
            }
            else if (consumption is BaseNetworkZoneConsumption)
            {
                var networkConsumption = consumption as BaseNetworkZoneConsumption;

                if (consumption is BaseInfrastructureNetworkZoneConsumption)
                {
                    if (consumption is RoadZoneConsumption)
                    {
                        bitmapOne = _bitmapAccessor.NetworkZonesInstance.RoadInstance.GetBitmapFor(networkConsumption as RoadZoneConsumption);
                    }
                    else if (consumption is RailRoadZoneConsumption)
                    {
                        bitmapOne = _railNetworkZoneTileset.GetBitmapFor(networkConsumption);
                    }
                    else if (consumption is PowerLineConsumption)
                    {
                        bitmapOne = _powerNetworkZoneTileset.GetBitmapFor(networkConsumption);
                    }
                    else if (consumption is WaterZoneConsumption)
                    {
                        bitmapOne = _waterNetworkZoneTileset.GetBitmapFor(networkConsumption);
                    }
                    else throw new InvalidOperationException();
                }
                else if (consumption is WoodlandZoneConsumption)
                {
                    bitmapOne = _woodNetworkZoneTileset.GetBitmapFor(networkConsumption);
                }
                else if (consumption is ParkZoneConsumption)
                {
                    bitmapOne = _parkNetworkZoneTileset.GetBitmapFor(networkConsumption);
                }
                else throw new InvalidOperationException();
            }

            else if (consumption is IIntersectingZoneConsumption)
            {
                var intersection = consumption as IIntersectingZoneConsumption;

                if (intersection.NorthSouthZoneConsumption is RoadZoneConsumption ||
                    intersection.EastWestZoneConsumption is RoadZoneConsumption)
                {
                    bitmapLayer = _bitmapAccessor.NetworkZonesInstance.RoadInstance.GetBitmapLayerFor(intersection);
                }
                else if (intersection.NorthSouthZoneConsumption is RailRoadZoneConsumption
                    && intersection.EastWestZoneConsumption is PowerLineConsumption)
                {
                    bitmapOne = _bitmapAccessor.NetworkZonesInstance.RailNorthSouthPowerEastWest;
                }
                else if (intersection.NorthSouthZoneConsumption is PowerLineConsumption
                    && intersection.EastWestZoneConsumption is RailRoadZoneConsumption)
                {
                    bitmapOne = _bitmapAccessor.NetworkZonesInstance.PowerNorthSouthRailEastWest.Value;
                }
                else if (intersection.NorthSouthZoneConsumption is PowerLineConsumption
                         && intersection.EastWestZoneConsumption is WaterZoneConsumption)
                {
                    bitmapOne = _waterNetworkZoneTileset.GetBitmapFor(intersection.GetZoneConsumptionOfType<WaterZoneConsumption>());
                    bitmapTwo = _bitmapAccessor.NetworkZonesInstance.PowerNorthSouthWaterEastWest;
                }
                else if (intersection.NorthSouthZoneConsumption is WaterZoneConsumption
                         && intersection.EastWestZoneConsumption is PowerLineConsumption)
                {
                    bitmapOne = _waterNetworkZoneTileset.GetBitmapFor(intersection.GetZoneConsumptionOfType<WaterZoneConsumption>());
                    bitmapTwo = _bitmapAccessor.NetworkZonesInstance.WaterNorthSouthPowerEastWest.Value;
                }
                else if (intersection.NorthSouthZoneConsumption is RailRoadZoneConsumption
                         && intersection.EastWestZoneConsumption is WaterZoneConsumption)
                {
                    bitmapOne = _waterNetworkZoneTileset.GetBitmapFor(intersection.GetZoneConsumptionOfType<WaterZoneConsumption>());
                    bitmapTwo = _bitmapAccessor.NetworkZonesInstance.RailNorthSouthWaterEastWest;
                }
                else if (intersection.NorthSouthZoneConsumption is WaterZoneConsumption
                         && intersection.EastWestZoneConsumption is RailRoadZoneConsumption)
                {
                    bitmapOne = _waterNetworkZoneTileset.GetBitmapFor(intersection.GetZoneConsumptionOfType<WaterZoneConsumption>());
                    bitmapTwo = _bitmapAccessor.NetworkZonesInstance.WaterNorthSouthRailEastWest.Value;
                }
                else throw new InvalidOperationException();
            }

            else if (consumption is ZoneClusterMemberConsumption)
            {
                bool showHasNoElectricitySymbol = false;

                var zoneClusterMemberConsumption = consumption as ZoneClusterMemberConsumption;
                var parentConsumption = (consumption as ZoneClusterMemberConsumption).ParentBaseZoneClusterConsumption;

                Bitmap selectedBitmap = null;

                if (parentConsumption is BaseImplementedZoneClusterConsumption)
                {
                    showHasNoElectricitySymbol = !(parentConsumption as BaseImplementedZoneClusterConsumption).HasPower;

                    if (parentConsumption is CoalPowerPlantZoneClusterConsumption)
                    {
                        selectedBitmap = _bitmapAccessor.PowerPlant.GetCurrentBitmapFrame();
                    }
                    else if (parentConsumption is NuclearPowerPlantZoneClusterConsumption)
                    {
                        selectedBitmap = _bitmapAccessor.NuclearPowerplant;
                    }
                    else if (parentConsumption is PoliceStationZoneClusterConsumption)
                    {
                        selectedBitmap = _bitmapAccessor.Police;
                    }
                    else if (parentConsumption is FireStationZoneclusterConsumption)
                    {
                        selectedBitmap = _bitmapAccessor.FireStation;
                    }
                    else if (parentConsumption is TrainStationZoneClusterConsumption)
                    {
                        selectedBitmap = _bitmapAccessor.TrainStation;
                    }
                    else if (parentConsumption is AirportZoneClusterConsumption)
                    {
                        selectedBitmap = _bitmapAccessor.Airport;
                    }
                    else if (parentConsumption is SeaPortZoneClusterConsumption)
                    {
                        selectedBitmap = _bitmapAccessor.SeaPort;
                    }
                    else if (parentConsumption is StadiumZoneClusterConsumption)
                    {
                        selectedBitmap = _bitmapAccessor.Stadium;
                    }

                    var baseGrowthZoneConsumption = parentConsumption as BaseGrowthZoneClusterConsumption;

                    if (baseGrowthZoneConsumption != null)
                    {
                        if (baseGrowthZoneConsumption is ResidentialZoneClusterConsumption)
                        {
                            var residentialZoneConsumption = baseGrowthZoneConsumption as ResidentialZoneClusterConsumption;

                            if (residentialZoneConsumption.RenderAsHouse(zoneClusterMemberConsumption))
                            {
                                bitmapOne =
                                    BitmapSelectorCollectionsInstance.ResidentialHouseCollection.GetBitmapFor(
                                        zoneClusterMemberConsumption);

                                bitmapLayer = new BitmapLayer(bitmapOne);
                                return true;
                            }

                            selectedBitmap = BitmapSelectorCollectionsInstance
                                .ResidentialCollection
                                .GetBitmapFor(baseGrowthZoneConsumption);
                        }
                        else if (baseGrowthZoneConsumption is CommercialZoneClusterConsumption)
                        {
                            selectedBitmap = BitmapSelectorCollectionsInstance
                                .CommercialCollection
                                .GetBitmapFor(baseGrowthZoneConsumption);
                        }
                        else if (baseGrowthZoneConsumption is IndustrialZoneClusterConsumption)
                        {
                            selectedBitmap = BitmapSelectorCollectionsInstance
                                .IndustrialCollection
                                .GetBitmapFor(baseGrowthZoneConsumption);
                        }
                    }
                }

                if (selectedBitmap != null)
                {
                    if (zoneClusterMemberConsumption.IsCentralClusterMember && showHasNoElectricitySymbol && DateTime.Now.Millisecond > 500)
                    {
                        bitmapOne = _bitmapAccessor.GrowthZonesInstance.NoElectricity;
                    }
                    else
                    {
                        bitmapOne = bitmapSegmenter.GetSegment(
                            image: selectedBitmap, 
                            x: zoneClusterMemberConsumption.PositionInClusterX, 
                            y: zoneClusterMemberConsumption.PositionInClusterY, 
                            multiplier: DefaultTileWidthAndSizeInPixels
                        );
                    }
                }
            }

            if (bitmapOne != null)
            {
                bitmapLayer = new BitmapLayer(bitmapOne, bitmapTwo);
            }

            return bitmapLayer != null;

        }

        internal BitmapSelectorCollections BitmapSelectorCollectionsInstance = new BitmapSelectorCollections();

        private readonly BitmapSegmenter bitmapSegmenter = new BitmapSegmenter();

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
    }
}