using System;
using System.Linq;

namespace Mirage.Urbanization.Tilesets
{
    public class CellBitmapNetwork
    {
        public DirectionalCellBitmap Center { get; set; }
        public DirectionalCellBitmap East { get; set; }
        public DirectionalCellBitmap EastWest { get; set; }
        public DirectionalCellBitmap NorthWest { get; set; }
        public DirectionalCellBitmap NorthWestEast { get; set; }
        public DirectionalCellBitmap NorthWestEastSouth { get; set; }

        public AnimatedCellBitmapSetLayers GetForDirections(bool north, bool east, bool south, bool west)
        {
            var specificationCount = new[] { north, east, south, west }.Count(x => x);

            switch (specificationCount)
            {
                case 0:
                    return Center.Right;
                case 1:
                    if (east)
                        return East.Up;
                    else if (south)
                        return East.Right;
                    else if (west)
                        return East.Down;
                    else if (north)
                        return East.Left;
                    else
                        throw new InvalidOperationException();
                case 2:
                    if (east && west)
                        return EastWest.Up;
                    else if (north && south)
                        return EastWest.Right;
                    else if (west && north)
                        return NorthWest.Up;
                    else if (north && east)
                        return NorthWest.Right;
                    else if (east && south)
                        return NorthWest.Down;
                    else if (south && west)
                        return NorthWest.Left;
                    else
                        throw new InvalidOperationException();
                case 3:
                    if (west && north && east)
                        return NorthWestEast.Up;
                    else if (north && east && south)
                        return NorthWestEast.Right;
                    else if (east && south && west)
                        return NorthWestEast.Down;
                    else if (south && west && north)
                        return NorthWestEast.Left;
                    else
                        throw new InvalidOperationException();
                case 4:
                    return NorthWestEastSouth.Up;
                default:
                    throw new InvalidOperationException();
            }
        }

        public CellBitmapNetwork(
            DirectionalCellBitmap center,
            DirectionalCellBitmap east,
            DirectionalCellBitmap eastWest,
            DirectionalCellBitmap northWest,
            DirectionalCellBitmap northWestEast,
            DirectionalCellBitmap northWestEastSouth)
        {
            Center = center;
            East = east;
            EastWest = eastWest;
            NorthWest = northWest;
            NorthWestEast = northWestEast;
            NorthWestEastSouth = northWestEastSouth;
        }
    }
}