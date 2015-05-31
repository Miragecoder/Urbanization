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
        private int _tileWidthAndSizeInPixels = 25;
        public int TileWidthAndSizeInPixels 
        { 
            get { return _tileWidthAndSizeInPixels; } 
            set { _tileWidthAndSizeInPixels = value; }
        }

        private readonly INetworkZoneTileset _woodNetworkZoneTileset = new NetworkZoneTileset(
            bitmapEastWest: BitmapAccessor.NetworkZones.Wood.WoodEastWest,
            bitmapNorthWest: BitmapAccessor.NetworkZones.Wood.WoodNorthWest,
            bitmapWestNorthEast: BitmapAccessor.NetworkZones.Wood.WoodWestNorthEast,
            bitmapNorthEastSouthWest: BitmapAccessor.NetworkZones.Wood.WoodNorthWestEastSouth,
            bitmapEast: BitmapAccessor.NetworkZones.Wood.WoodEast,
            bitmapNoDirection: BitmapAccessor.NetworkZones.Wood.WoodNoDirection
        );

        private readonly INetworkZoneTileset _railNetworkZoneTileset = new NetworkZoneTileset(
            bitmapEastWest: BitmapAccessor.NetworkZones.Rail.RailEastWest,
            bitmapNorthWest: BitmapAccessor.NetworkZones.Rail.RailNorthWest,
            bitmapWestNorthEast: BitmapAccessor.NetworkZones.Rail.RailWestNorthEast,
            bitmapNorthEastSouthWest: BitmapAccessor.NetworkZones.Rail.RailNorthWestEastSouth,
            bitmapEast: BitmapAccessor.NetworkZones.Rail.RailEast,
            bitmapNoDirection: BitmapAccessor.NetworkZones.Rail.RailNoDirection
        );

        private readonly INetworkZoneTileset _powerNetworkZoneTileset = new NetworkZoneTileset(
            bitmapEastWest: BitmapAccessor.NetworkZones.Power.PowerEastWest,
            bitmapNorthWest: BitmapAccessor.NetworkZones.Power.PowerNorthWest,
            bitmapWestNorthEast: BitmapAccessor.NetworkZones.Power.PowerWestNorthEast,
            bitmapNorthEastSouthWest: BitmapAccessor.NetworkZones.Power.PowerNorthWestEastSouth,
            bitmapEast: BitmapAccessor.NetworkZones.Power.PowerEast,
            bitmapNoDirection: BitmapAccessor.NetworkZones.Power.PowerNoDirection
        );

        private readonly INetworkZoneTileset _waterNetworkZoneTileset = new NetworkZoneTileset(
            bitmapEastWest: BitmapAccessor.NetworkZones.Water.WaterEastWest,
            bitmapNorthWest: BitmapAccessor.NetworkZones.Water.WaterNorthWest,
            bitmapWestNorthEast: BitmapAccessor.NetworkZones.Water.WaterWestNorthEast,
            bitmapNorthEastSouthWest: BitmapAccessor.NetworkZones.Water.WaterNorthWestEastSouth,
            bitmapEast: BitmapAccessor.NetworkZones.Water.WaterEast,
            bitmapNoDirection: BitmapAccessor.NetworkZones.Water.WaterNoDirection
        );


        public bool TryGetBitmapFor(IAreaZoneConsumption consumption, out Bitmap bitmap)
        {
            bitmap = null;

            if (consumption is RubbishZoneConsumption)
            {
                bitmap = BitmapAccessor.Rubbish;
            }
            else if (consumption is BaseNetworkZoneConsumption)
            {
                var networkConsumption = consumption as BaseNetworkZoneConsumption;

                if (consumption is BaseInfrastructureNetworkZoneConsumption)
                {
                    if (consumption is RoadZoneConsumption)
                    {
                        bitmap = BitmapAccessor.NetworkZones.Road.GetBitmapFor(networkConsumption as RoadZoneConsumption);
                    }
                    else if (consumption is RailRoadZoneConsumption)
                    {
                        bitmap = _railNetworkZoneTileset.GetBitmapFor(networkConsumption);
                    }
                    else if (consumption is PowerLineConsumption)
                    {
                        bitmap = _powerNetworkZoneTileset.GetBitmapFor(networkConsumption);
                    }
                    else if (consumption is WaterZoneConsumption)
                    {
                        bitmap = _waterNetworkZoneTileset.GetBitmapFor(networkConsumption);
                    }
                    else throw new InvalidOperationException();
                }
                else if (consumption is WoodlandZoneConsumption)
                {
                    bitmap = _woodNetworkZoneTileset.GetBitmapFor(networkConsumption);
                }
                else throw new InvalidOperationException();
            }

            else if (consumption is IIntersectingZoneConsumption)
            {
                var intersection = consumption as IIntersectingZoneConsumption;

                if (intersection.NorthSouthZoneConsumption is RoadZoneConsumption ||
                    intersection.EastWestZoneConsumption is RoadZoneConsumption)
                {
                    bitmap = BitmapAccessor.NetworkZones.Road.GetBitmapFor(intersection);
                }
                else if (intersection.NorthSouthZoneConsumption is RailRoadZoneConsumption
                    && intersection.EastWestZoneConsumption is PowerLineConsumption)
                {
                    bitmap = BitmapAccessor.NetworkZones.RailNorthSouthPowerEastWest;
                }
                else if (intersection.NorthSouthZoneConsumption is PowerLineConsumption
                    && intersection.EastWestZoneConsumption is RailRoadZoneConsumption)
                {
                    bitmap = BitmapAccessor.NetworkZones.PowerNorthSouthRailEastWest.Value;
                }
                else if (intersection.NorthSouthZoneConsumption is PowerLineConsumption
                         && intersection.EastWestZoneConsumption is WaterZoneConsumption)
                {
                    bitmap = BitmapAccessor.NetworkZones.PowerNorthSouthWaterEastWest;
                }
                else if (intersection.NorthSouthZoneConsumption is WaterZoneConsumption
                         && intersection.EastWestZoneConsumption is PowerLineConsumption)
                {
                    bitmap = BitmapAccessor.NetworkZones.WaterNorthSouthPowerEastWest.Value;
                }
                else if (intersection.NorthSouthZoneConsumption is RailRoadZoneConsumption
                         && intersection.EastWestZoneConsumption is WaterZoneConsumption)
                {
                    bitmap = BitmapAccessor.NetworkZones.RailNorthSouthWaterEastWest;
                }
                else if (intersection.NorthSouthZoneConsumption is WaterZoneConsumption
                         && intersection.EastWestZoneConsumption is RailRoadZoneConsumption)
                {
                    bitmap = BitmapAccessor.NetworkZones.WaterNorthSouthRailEastWest.Value;
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
                        selectedBitmap = BitmapAccessor.PowerPlant.GetCurrentBitmapFrame();
                    }
                    else if (parentConsumption is PoliceStationZoneClusterConsumption)
                    {
                        selectedBitmap = BitmapAccessor.Police;
                    }
                    else if (parentConsumption is TrainStationZoneClusterConsumption)
                    {
                        selectedBitmap = BitmapAccessor.TrainStation;
                    }
                    else if (parentConsumption is AirportZoneClusterConsumption)
                    {
                        selectedBitmap = BitmapAccessor.Airport;
                    }

                    var baseGrowthZoneConsumption = parentConsumption as BaseGrowthZoneClusterConsumption;

                    if (baseGrowthZoneConsumption != null)
                    {
                        if (baseGrowthZoneConsumption is ResidentialZoneClusterConsumption)
                        {
                            var residentialZoneConsumption = baseGrowthZoneConsumption as ResidentialZoneClusterConsumption;

                            if (residentialZoneConsumption.RenderAsHouse(zoneClusterMemberConsumption))
                            {
                                bitmap =
                                    BitmapSelectorCollections.ResidentialHouseCollection.GetBitmapFor(
                                        zoneClusterMemberConsumption);

                                return true;
                            }

                            selectedBitmap = BitmapSelectorCollections
                                .ResidentialCollection
                                .GetBitmapFor(baseGrowthZoneConsumption);
                        }
                        else if (baseGrowthZoneConsumption is CommercialZoneClusterConsumption)
                        {
                            selectedBitmap = BitmapSelectorCollections
                                .CommercialCollection
                                .GetBitmapFor(baseGrowthZoneConsumption);
                        }
                        else if (baseGrowthZoneConsumption is IndustrialZoneClusterConsumption)
                        {
                            selectedBitmap = BitmapSelectorCollections
                                .IndustrialCollection
                                .GetBitmapFor(baseGrowthZoneConsumption);
                        }
                    }
                }

                if (selectedBitmap != null)
                {
                    if (zoneClusterMemberConsumption.IsCentralClusterMember && showHasNoElectricitySymbol && DateTime.Now.Millisecond > 500)
                    {
                        bitmap = BitmapAccessor.GrowthZones.NoElectricity;
                    }
                    else
                    {
                        bitmap = bitmapSegmenter.GetSegment(selectedBitmap, zoneClusterMemberConsumption.PositionInClusterX,
                            zoneClusterMemberConsumption.PositionInClusterY, TileWidthAndSizeInPixels);
                    }
                }
            }
            return bitmap != null;

        }

        private readonly BitmapSegmenter bitmapSegmenter = new BitmapSegmenter();

        public Size GetAreaSize(IReadOnlyArea area)
        {
            return new Size(
                width: area.AmountOfZonesX * TileWidthAndSizeInPixels,
                height: area.AmountOfZonesY * TileWidthAndSizeInPixels
            );
        }
    }
}