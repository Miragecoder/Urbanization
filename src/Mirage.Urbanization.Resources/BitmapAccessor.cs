using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using Mirage.Urbanization.ZoneConsumption;
using Mirage.Urbanization.ZoneConsumption.Base;

namespace Mirage.Urbanization.Tilesets
{
    internal class BitmapSelector
    {
        private readonly object[] _objects;

        internal BitmapSelector(params Bitmap[] bitmaps)
            : this(bitmaps, null)
        {

        }

        internal BitmapSelector(Bitmap[] bitmaps = null, AnimatedBitmap[] animatedBitmaps = null)
        {
            var @objects = new List<object>();
            if (bitmaps != null)
                @objects.AddRange(bitmaps);
            if (animatedBitmaps != null)
                @objects.AddRange(animatedBitmaps);
            _objects = @objects.ToArray();
        }

        public Bitmap SelectOneWithId(int id)
        {
            var @object = _objects[id % _objects.Length];
            if (@object is Bitmap)
                return @object as Bitmap;
            else if (@object is AnimatedBitmap)
                return (@object as AnimatedBitmap).GetCurrentBitmapFrame();
            throw new InvalidOperationException("Unsupported object type encountered: " + @object.GetType().Name);
        }
    }

    internal class GrowthZonePredicateAndBitmapSelector<T>
        where T : IAreaObjectWithSeed, IAreaObjectWithPopulationDensity
    {
        public Func<IAreaObjectWithPopulationDensity, bool> Predicate { get; }

        internal BitmapSelector BitmapSelector { get; }

        public GrowthZonePredicateAndBitmapSelector(Func<IAreaObjectWithPopulationDensity, bool> predicate, BitmapSelector bitmapSelector)
        {
            Predicate = predicate;
            BitmapSelector = bitmapSelector;
        }
    }

    internal class GrowthZoneBitmapSelectorCollection<T>
        where T : IAreaObjectWithSeed, IAreaObjectWithPopulationDensity
    {
        private readonly IList<GrowthZonePredicateAndBitmapSelector<T>> _predicateAndBitmapSelectors;

        public GrowthZoneBitmapSelectorCollection(params GrowthZonePredicateAndBitmapSelector<T>[] growthZoneAndBitmapSelectors)
        {
            _predicateAndBitmapSelectors = growthZoneAndBitmapSelectors.ToList();
        }

        public Bitmap GetBitmapFor(T cluster)
        {
            var snapshot = new ClusterSnapshot(cluster);
            return _predicateAndBitmapSelectors
                .Single(x => x.Predicate(snapshot))
                .BitmapSelector
                .SelectOneWithId(cluster.Id);
        }

        private struct ClusterSnapshot : IAreaObjectWithPopulationDensity
        {
            public ClusterSnapshot(T cluster)
            {
                Id = cluster.Id;
                PopulationDensity = cluster.PopulationDensity;
            }
            public int Id { get; }
            public int PopulationDensity { get; }
        }
    }

    internal class BitmapSelectorCollections
    {
        internal readonly GrowthZoneBitmapSelectorCollection<BaseGrowthZoneClusterConsumption> CommercialCollection = new GrowthZoneBitmapSelectorCollection<BaseGrowthZoneClusterConsumption>(
            new GrowthZonePredicateAndBitmapSelector<BaseGrowthZoneClusterConsumption>(x => x.PopulationDensity == 0,
                new BitmapSelector(new[] { BitmapAccessor.GetImage("GrowthZones.Commercial.empty.png") })
            ),
            new GrowthZonePredicateAndBitmapSelector<BaseGrowthZoneClusterConsumption>(x => x.PopulationDensity > 0 && x.PopulationDensity < 25,
                new BitmapSelector(
                    BitmapAccessor.GetImage("GrowthZones.Commercial.d1q1n1.png"),
                    BitmapAccessor.GetImage("GrowthZones.Commercial.d1q1n2.png")
                )
            ),
            new GrowthZonePredicateAndBitmapSelector<BaseGrowthZoneClusterConsumption>(x => x.PopulationDensity >= 25,
                new BitmapSelector(
                    BitmapAccessor.GetImage("GrowthZones.Commercial.d2q1n1.png"),
                    BitmapAccessor.GetImage("GrowthZones.Commercial.d2q1n2.png"),
                    BitmapAccessor.GetImage("GrowthZones.Commercial.d2q1n3.png"),
                    BitmapAccessor.GetImage("GrowthZones.Commercial.d2q1n4.png")
                )
            )
        );

        internal readonly GrowthZoneBitmapSelectorCollection<BaseGrowthZoneClusterConsumption> IndustrialCollection = new GrowthZoneBitmapSelectorCollection<BaseGrowthZoneClusterConsumption>(
            new GrowthZonePredicateAndBitmapSelector<BaseGrowthZoneClusterConsumption>(x => x.PopulationDensity == 0,
                new BitmapSelector(BitmapAccessor.GetImage("GrowthZones.Industrial.empty.png"))
            ),
            new GrowthZonePredicateAndBitmapSelector<BaseGrowthZoneClusterConsumption>(x => x.PopulationDensity > 0 && x.PopulationDensity < 25,
                new BitmapSelector(
                    animatedBitmaps: new[]
                    {
                        new AnimatedBitmap(300,
                            BitmapAccessor.GetImage("GrowthZones.Industrial.d1q1n1a1.png"),
                            BitmapAccessor.GetImage("GrowthZones.Industrial.d1q1n1a2.png"),
                            BitmapAccessor.GetImage("GrowthZones.Industrial.d1q1n1a3.png")
                        ),
                    }
                )
            ),
            new GrowthZonePredicateAndBitmapSelector<BaseGrowthZoneClusterConsumption>(x => x.PopulationDensity >= 25,
                new BitmapSelector(
                    animatedBitmaps: new[]
                    {
                        new AnimatedBitmap(200,
                            BitmapAccessor.GetImage("GrowthZones.Industrial.d2q1n1a1.png"),
                            BitmapAccessor.GetImage("GrowthZones.Industrial.d2q1n1a2.png"),
                            BitmapAccessor.GetImage("GrowthZones.Industrial.d2q1n1a3.png")
                        ),
                    }
                )
            )
        );

        internal readonly GrowthZoneBitmapSelectorCollection<BaseGrowthZoneClusterConsumption> ResidentialCollection = new GrowthZoneBitmapSelectorCollection<BaseGrowthZoneClusterConsumption>(
            new GrowthZonePredicateAndBitmapSelector<BaseGrowthZoneClusterConsumption>(x => x.PopulationDensity <= 8,
                new BitmapSelector(BitmapAccessor.GetImage("GrowthZones.Residential.empty.png"))
            ),
            new GrowthZonePredicateAndBitmapSelector<BaseGrowthZoneClusterConsumption>(x => x.PopulationDensity > 8 && x.PopulationDensity < 25,
                new BitmapSelector(
                    BitmapAccessor.GetImage("GrowthZones.Residential.d1q1n1.png")
                )
            ),
            new GrowthZonePredicateAndBitmapSelector<BaseGrowthZoneClusterConsumption>(x => x.PopulationDensity >= 25,
                new BitmapSelector(
                    BitmapAccessor.GetImage("GrowthZones.Residential.d2q1n1.png"),
                    BitmapAccessor.GetImage("GrowthZones.Residential.d2q1n2.png"),
                    BitmapAccessor.GetImage("GrowthZones.Residential.d2q1n3.png"),
                    BitmapAccessor.GetImage("GrowthZones.Residential.d2q1n4.png")
                )
            )
        );

        internal readonly GrowthZoneBitmapSelectorCollection<ZoneClusterMemberConsumption> ResidentialHouseCollection = new GrowthZoneBitmapSelectorCollection<ZoneClusterMemberConsumption>(
            new GrowthZonePredicateAndBitmapSelector<ZoneClusterMemberConsumption>(x => x.PopulationDensity <= 8,
                new BitmapSelector(
                    BitmapAccessor.GetImage("GrowthZones.Residential.Houses.d1q1n1.png"),
                    BitmapAccessor.GetImage("GrowthZones.Residential.Houses.d1q1n2.png"),
                    BitmapAccessor.GetImage("GrowthZones.Residential.Houses.d1q1n3.png"),
                    BitmapAccessor.GetImage("GrowthZones.Residential.Houses.d1q1n4.png")
                )
            )
        );

    }

    public class MiscBitmaps
    {
        public readonly DirectionalBitmap Plane = new DirectionalBitmap(BitmapAccessor.GetImage("airplane.png"));
        public readonly DirectionalBitmap Train = new DirectionalBitmap(BitmapAccessor.GetImage("train.png"));

        public DirectionalBitmap GetShipBitmapFrame()
        {
            return DateTime.Now.Millisecond % 400 > 200 ? ShipFrameOne : ShipFrameTwo;
        }

        public readonly DirectionalBitmap ShipFrameOne = new DirectionalBitmap(BitmapAccessor.GetImage("shipanim1.png"));
        public readonly DirectionalBitmap ShipFrameTwo = new DirectionalBitmap(BitmapAccessor.GetImage("shipanim2.png"));
    }

    public class DirectionalBitmap
    {
        private readonly Bitmap _bitmapEast
            , _bitmapNorth,
            _bitmapWest,
            _bitmapSouth,
            _southEast,
            _southWest,
            _northWest,
            _northEast;

        public DirectionalBitmap(Bitmap bitmapEast)
        {
            _bitmapEast = bitmapEast;
            _bitmapSouth = _bitmapEast.Get90DegreesRotatedClone();
            _bitmapWest = _bitmapSouth.Get90DegreesRotatedClone();
            _bitmapNorth = _bitmapWest.Get90DegreesRotatedClone();

            _southEast = _bitmapEast.RotateImage(45);
            _southWest = _southEast.Get90DegreesRotatedClone();
            _northWest = _southWest.Get90DegreesRotatedClone();
            _northEast = _northWest.Get90DegreesRotatedClone();
        }

        public Bitmap GetBitmap(Orientation orientation)
        {
            switch (orientation)
            {
                case Orientation.East:
                    return _bitmapEast;
                case Orientation.West:
                    return _bitmapWest;
                case Orientation.North:
                    return _bitmapNorth;
                case Orientation.South:
                    return _bitmapSouth;
                case Orientation.NorthEast:
                    return _northEast;
                case Orientation.NorthWest:
                    return _northWest;
                case Orientation.SouthEast:
                    return _southEast;
                case Orientation.SouthWest:
                    return _southWest;
            }
            throw new InvalidOperationException();
        }
    }

    internal class BitmapAccessor
    {
        public readonly AnimatedBitmap PowerPlant = new AnimatedBitmap(250, GetImage("coal1.png"), GetImage("coal2.png"), GetImage("coal3.png"), GetImage("coal4.png"));

        public readonly Bitmap TrainStation = GetImage("trainstation.png");
        public readonly Bitmap Airport = GetImage("airport.png");

        public readonly Bitmap NuclearPowerplant = GetImage("nuclear.png");
        public readonly Bitmap Police = GetImage("police.png");
        public readonly Bitmap FireStation = GetImage("firestation.png");
        public readonly Bitmap SeaPort = GetImage("seaport.png");
        public readonly Bitmap Stadium = GetImage("stadium.png");

        public GrowthZones GrowthZonesInstance = new GrowthZones();

        public class GrowthZones
        {
            public readonly Bitmap NoElectricity = GetImage("GrowthZones.noelectricity.png");
        }
        public readonly Bitmap Rubbish = GetImage("rubbish.png");

        public NetworkZones NetworkZonesInstance;

        public BitmapAccessor()
        {
            NetworkZonesInstance = new NetworkZones();
        }

        public class NetworkZones
        {
            public readonly Bitmap RailNorthSouthRoadEastWest = GetImage("NetworkZones.railnsroadew.png");
            public readonly Lazy<Bitmap> RoadNorthSouthRailEastWest;

            public readonly Bitmap WaterNorthSouthRoadEastWest = GetImage("NetworkZones.waternsroadew.png");
            public readonly Lazy<Bitmap> RoadNorthSouthWaterEastWest;

            public readonly Bitmap PowerNorthSouthRoadEastWest = GetImage("NetworkZones.powernsroadew.png");
            public readonly Lazy<Bitmap> RoadNorthSouthPowerEastWest;

            public readonly Bitmap RailNorthSouthWaterEastWest = GetImage("NetworkZones.railnswaterew.png");
            public readonly Lazy<Bitmap> WaterNorthSouthRailEastWest;

            public readonly Bitmap PowerNorthSouthWaterEastWest = GetImage("NetworkZones.powernswaterew.png");
            public readonly Lazy<Bitmap> WaterNorthSouthPowerEastWest;


            public readonly Bitmap RailNorthSouthPowerEastWest = GetImage("NetworkZones.railnspowerew.png");
            public readonly Lazy<Bitmap> PowerNorthSouthRailEastWest;

            public NetworkZones()
            {
                PowerNorthSouthRailEastWest = new Lazy<Bitmap>(() =>
                    RailNorthSouthPowerEastWest.Get90DegreesRotatedClone()
                    );
                WaterNorthSouthPowerEastWest = new Lazy<Bitmap>(() =>
                    PowerNorthSouthWaterEastWest.Get90DegreesRotatedClone());
                WaterNorthSouthRailEastWest = new Lazy<Bitmap>(() =>
                    RailNorthSouthWaterEastWest.Get90DegreesRotatedClone());
                RoadNorthSouthPowerEastWest = new Lazy<Bitmap>(() =>
                    PowerNorthSouthRoadEastWest.Get90DegreesRotatedClone()
                    );
                RoadNorthSouthWaterEastWest = new Lazy<Bitmap>(() =>
                    WaterNorthSouthRoadEastWest.Get90DegreesRotatedClone()
                    );
                RoadNorthSouthRailEastWest = new Lazy<Bitmap>(() =>
                    RailNorthSouthRoadEastWest.Get90DegreesRotatedClone()
                    );

                RoadInstance = new Road(WaterInstance, this);
            }

            private static NetworkZoneTileset GenerateNetworkZoneTileSet(string formatter)
            {
                return new NetworkZoneTileset(
                    bitmapEast: GetImage(String.Format(formatter, "e")),
                    bitmapEastWest: GetImage(String.Format(formatter, "ew")),
                    bitmapNorthWest: GetImage(String.Format(formatter, "nw")),
                    bitmapWestNorthEast: GetImage(String.Format(formatter, "nwe")),
                    bitmapNorthEastSouthWest: GetImage(String.Format(formatter, "nwes")),
                    bitmapNoDirection: GetImage(String.Format(formatter, string.Empty))
                );
            }

            public readonly Road RoadInstance;

            public class Road
            {
                private readonly Water _waterInstance;
                private readonly NetworkZones _networkZones;
                public readonly NetworkZoneTileset RoadZoneTileSet;

                public Road(Water waterInstance, NetworkZones networkZones)
                {
                    _waterInstance = waterInstance;
                    _networkZones = networkZones;
                    RoadZoneTileSet = GenerateNetworkZoneTileSet("NetworkZones.Road.road{0}.png");
                }

                public Bitmap GetBitmapFor(RoadZoneConsumption consumption)
                {
                    switch (consumption.GetTrafficDensity())
                    {
                        case TrafficDensity.Low:
                            return TrafficAnimInstance.Low.GetBitmapFor(consumption);
                        case TrafficDensity.High:
                            return TrafficAnimInstance.High.GetBitmapFor(consumption);
                        case TrafficDensity.None:
                            return RoadZoneTileSet.GetBitmapFor(consumption);
                        default:
                            throw new NotImplementedException();
                    }
                }

                public BitmapLayer GetBitmapLayerFor(IIntersectingZoneConsumption intersection)
                {
                    if (intersection.GetIntersectingTypes().Any(x => x == typeof(WaterZoneConsumption)))
                    {
                        return new BitmapLayer(_waterInstance.WaterNorthWestEastSouth, GetBitmapFor(intersection));
                    }
                    return new BitmapLayer(GetBitmapFor(intersection));
                }

                private Bitmap GetBitmapFor(IIntersectingZoneConsumption intersection)
                {
                    if (intersection.EastWestZoneConsumption is RoadZoneConsumption ^
                        intersection.NorthSouthZoneConsumption is RoadZoneConsumption)
                    {
                        switch (((intersection.EastWestZoneConsumption as RoadZoneConsumption) ??
                                       (intersection.NorthSouthZoneConsumption as RoadZoneConsumption)).GetTrafficDensity())
                        {
                            case TrafficDensity.None:

                                if (intersection.EastWestZoneConsumption is RailRoadZoneConsumption)
                                {
                                    return _networkZones.RoadNorthSouthRailEastWest.Value;
                                }
                                else if (intersection.NorthSouthZoneConsumption is RailRoadZoneConsumption)
                                {
                                    return _networkZones.RailNorthSouthRoadEastWest;
                                }
                                else if (intersection.EastWestZoneConsumption is PowerLineConsumption)
                                {
                                    return _networkZones.RoadNorthSouthPowerEastWest.Value;
                                }
                                else if (intersection.NorthSouthZoneConsumption is PowerLineConsumption)
                                {
                                    return _networkZones.PowerNorthSouthRoadEastWest;
                                }
                                else if (intersection.NorthSouthZoneConsumption is WaterZoneConsumption)
                                {
                                    return _networkZones.WaterNorthSouthRoadEastWest;
                                }
                                else if (intersection.EastWestZoneConsumption is WaterZoneConsumption)
                                {
                                    return _networkZones.RoadNorthSouthWaterEastWest.Value;
                                }
                                else
                                {
                                    throw new InvalidOperationException();
                                }

                            case TrafficDensity.Low:
                                return TrafficAnimInstance.Low.GetBitmapFor(intersection);
                            case TrafficDensity.High:
                                return TrafficAnimInstance.High.GetBitmapFor(intersection);
                            default:
                                throw new NotImplementedException();
                        }
                    }
                    else throw new ArgumentException("Invalid intersection was specified.", nameof(intersection));
                }

                public TrafficAnim TrafficAnimInstance = new TrafficAnim();

                public class TrafficAnim
                {
                    private readonly InfiniteEnumeratorCycler Cycler = new InfiniteEnumeratorCycler();

                    public readonly AnimatedRoadNetworkZoneTileset Low;
                    public readonly AnimatedRoadNetworkZoneTileset High;

                    public TrafficAnim()
                    {
                        High = new AnimatedRoadNetworkZoneTileset(
                            frameTileSets: EnumerateHighFrames()
                                .Select(frameName => GenerateNetworkZoneTileSet(frameName + ".road{0}.png"))
                                .ToArray(),
                            powerNorthSouthRoadEastWestFrames: EnumerateHighFrames()
                                .Select(frameName => GetImage(frameName + ".powernsroadew.png"))
                                .ToArray(),
                            railNorthSouthRoadEastWestFrames: EnumerateHighFrames()
                                .Select(frameName => GetImage(frameName + ".railnsroadew.png"))
                                .ToArray(),
                            waterNorthSouthRoadEastWestFrames: EnumerateHighFrames()
                                .Select(frameName => GetImage(frameName + ".waternsroadew.png"))
                                .ToArray(),
                            cycler: Cycler
                            );
                        Low = new AnimatedRoadNetworkZoneTileset(
                            frameTileSets: EnumerateLowFrames()
                                .Select(frameName => GenerateNetworkZoneTileSet(frameName + ".road{0}.png"))
                                .ToArray(),
                            powerNorthSouthRoadEastWestFrames: EnumerateLowFrames()
                                .Select(frameName => GetImage(frameName + ".powernsroadew.png"))
                                .ToArray(),
                            railNorthSouthRoadEastWestFrames: EnumerateLowFrames()
                                .Select(frameName => GetImage(frameName + ".railnsroadew.png"))
                                .ToArray(),
                            waterNorthSouthRoadEastWestFrames: EnumerateLowFrames()
                                .Select(frameName => GetImage(frameName + ".waternsroadew.png"))
                                .ToArray(),
                            cycler: Cycler
                            );
                    }

                    private static IEnumerable<string> EnumerateLowFrames()
                    {
                        return
                            Enumerable.Range(1, 5)
                                .Select(iteration => "NetworkZones.Road.TrafficAnim.Low.Frame" + iteration);
                    }

                    private static IEnumerable<string> EnumerateHighFrames()
                    {
                        return
                            Enumerable.Range(1, 5)
                                .Select(iteration => "NetworkZones.Road.TrafficAnim.High.Frame" + iteration);
                    }
                }
            }

            public Rail RailInstance = new Rail();

            public class Rail
            {
                public readonly Bitmap RailNoDirection = GetImage("NetworkZones.Rail.rail.png");
                public readonly Bitmap RailEast = GetImage("NetworkZones.Rail.raile.png");
                public readonly Bitmap RailEastWest = GetImage("NetworkZones.Rail.railew.png");
                public readonly Bitmap RailNorthWest = GetImage("NetworkZones.Rail.railnw.png");
                public readonly Bitmap RailWestNorthEast = GetImage("NetworkZones.Rail.railnwe.png");
                public readonly Bitmap RailNorthWestEastSouth = GetImage("NetworkZones.Rail.railnwes.png");
            }

            public Power PowerInstance = new Power();

            public class Power
            {
                public readonly Bitmap PowerNoDirection = GetImage("NetworkZones.Power.power.png");
                public readonly Bitmap PowerEast = GetImage("NetworkZones.Power.powere.png");
                public readonly Bitmap PowerEastWest = GetImage("NetworkZones.Power.powerew.png");
                public readonly Bitmap PowerNorthWest = GetImage("NetworkZones.Power.powernw.png");
                public readonly Bitmap PowerWestNorthEast = GetImage("NetworkZones.Power.powernwe.png");
                public readonly Bitmap PowerNorthWestEastSouth = GetImage("NetworkZones.Power.powernwes.png");
            }

            public Wood WoodInstance = new Wood();

            public class Wood
            {
                public readonly Bitmap WoodNoDirection = GetImage("NetworkZones.Wood.wood.png");
                public readonly Bitmap WoodEast = GetImage("NetworkZones.Wood.woode.png");
                public readonly Bitmap WoodEastWest = GetImage("NetworkZones.Wood.woodew.png");
                public readonly Bitmap WoodNorthWest = GetImage("NetworkZones.Wood.woodnw.png");
                public readonly Bitmap WoodWestNorthEast = GetImage("NetworkZones.Wood.woodnwe.png");
                public readonly Bitmap WoodNorthWestEastSouth = GetImage("NetworkZones.Wood.woodnwes.png");
            }

            public Park ParkInstance = new Park();

            public class Park
            {
                public readonly Bitmap WoodNoDirection = GetImage("NetworkZones.Park.park.png");
                public readonly Bitmap WoodEast = GetImage("NetworkZones.Park.parke.png");
                public readonly Bitmap WoodEastWest = GetImage("NetworkZones.Park.parkew.png");
                public readonly Bitmap WoodNorthWest = GetImage("NetworkZones.Park.parknw.png");
                public readonly Bitmap WoodWestNorthEast = GetImage("NetworkZones.Park.parknwe.png");
                public readonly Bitmap WoodNorthWestEastSouth = GetImage("NetworkZones.Park.parknwes.png");
            }
            public Water WaterInstance = new Water();

            public class Water
            {
                public readonly Bitmap WaterNoDirection = GetImage("NetworkZones.Water.water.png");
                public readonly Bitmap WaterEast = GetImage("NetworkZones.Water.watere.png");
                public readonly Bitmap WaterEastWest = GetImage("NetworkZones.Water.waterew.png");
                public readonly Bitmap WaterNorthWest = GetImage("NetworkZones.Water.waternw.png");
                public readonly Bitmap WaterWestNorthEast = GetImage("NetworkZones.Water.waternwe.png");
                public readonly Bitmap WaterNorthWestEastSouth = GetImage("NetworkZones.Water.waternwes.png");
            }
        }

        public static Bitmap GetImage(string name)
        {
            Stream file = Assembly.GetExecutingAssembly().GetManifestResourceStream("Mirage.Urbanization.Tilesets." + name);

            if (file == null)
                throw new ArgumentException("Could not extract resource stream for file: " + name, nameof(name));
            return new Bitmap(Image.FromStream(file));
        }
    }
}