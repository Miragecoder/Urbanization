using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Mirage.Urbanization.Tilesets;

namespace Mirage.Urbanization.Web
{
    class TextureAtlas
    {
        private class InitializedAtlas
        {
            public InitializedAtlas(byte[] pngBytes, int cellSpriteOffset)
            {
                PngBytes = pngBytes;
                CellSpriteOffset = cellSpriteOffset;
            }

            public byte[] PngBytes { get;  }
            public int CellSpriteOffset { get; }
        }

        public const int CellsPerRow = 50;
        public const int VehicleTilesPerRow = 25;
        public ITilesetAccessor TilesetAccessor { get; } = new TilesetAccessor();

        public byte[] GetAtlasBytes() => _initializedAtlasLazy.Value.PngBytes;
        public int CellSpriteOffset => _initializedAtlasLazy.Value.CellSpriteOffset;

        public TextureAtlas()
        {
            _initializedAtlasLazy = new Lazy<InitializedAtlas>(() =>
            {
                var cellSprites = TilesetAccessor
                    .GetAll()
                    .ToArray()
                    .Pipe(cellLayers =>
                    {
                        return cellLayers
                            .SelectMany(x => x.LayerOne.Bitmaps)
                            .Concat(cellLayers.Where(x => x.LayerTwo.HasMatch).SelectMany(x => x.LayerTwo.MatchingObject.Bitmaps))
                            .ToDictionary(
                                keySelector: cellBitmap =>
                                    new Point(
                                        x: (cellBitmap.Id % CellsPerRow) * 25,
                                        y: Convert.ToInt32(Math.Floor(Convert.ToDecimal(cellBitmap.Id / CellsPerRow)) * 25)), 
                                elementSelector: x => x);
                    });

                var vehicleSprites = TilesetAccessor
                    .GetAllVehicleBitmaps()
                    .ToArray()
                    .Pipe(vehicleBitmaps =>
                    {
                        return vehicleBitmaps
                            .ToDictionary(
                                keySelector: vehicleBitmap =>
                                    new Point(
                                        x: (vehicleBitmap.Id % (VehicleTilesPerRow)) * 50,
                                        y: Convert.ToInt32(Math.Floor(Convert.ToDecimal(vehicleBitmap.Id / (VehicleTilesPerRow))) * 50)), 
                                elementSelector: x => x);
                    });

                using (var memoryStream = new MemoryStream())
                {
                    memoryStream.Position = 0;
                    using (var bitmap = new Bitmap(
                        width: cellSprites.Keys.Max(x => x.X).Pipe(x => x + 50),
                        height: cellSprites.Keys.Max(x => x.Y)
                            .Pipe(x => x + 75 + vehicleSprites.Max(v => v.Key.Y)))
                        )
                    {
                        using (var graphics = Graphics.FromImage(bitmap))
                        {
                            foreach (var x in cellSprites)
                            {
                                graphics.DrawImage(x.Value.Bitmap, x.Key);
                            }

                            var cellSpriteOffset = cellSprites.Max(x => x.Key.Y) + 25;
                            foreach (var x in vehicleSprites)
                            {
                                graphics.DrawImage(x.Value.Bitmap, new Point { X = x.Key.X, Y = x.Key.Y + cellSpriteOffset });
                            }

                            graphics.Save();
                            bitmap.Save(memoryStream, ImageFormat.Png);
                            return new InitializedAtlas(memoryStream.ToArray(), cellSpriteOffset);
                        }
                    }
                }
            });
        }

        private readonly Lazy<InitializedAtlas> _initializedAtlasLazy;
    }
}