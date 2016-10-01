using System;

namespace Mirage.Urbanization
{
    public class TerraformingOptions
    {
        public const int 
            MaxWoodlands = 150,MinWoodlands = 0,
            MaxLakes = 150, MinLakes = 0,
            MaxWidthAndHeight = 300, MinWidthAndHeight = 50;

        public bool HorizontalRiver { get; set; }
        public bool VerticalRiver { get; set; }

        public bool EastCoast { get; set; }
        public bool NorthCoast { get; set; }
        public bool SouthCoast { get; set; }
        public bool WestCoast { get; set; }

        public int Woodlands { get; private set; } = MinWoodlands;
        public int Lakes { get; private set; } = MinLakes;

        public int ZoneWidthAndHeight { get; private set; } = MinWidthAndHeight;

        public void SetZoneWidthAndHeight(int value)
        {
            if (value >= MinWidthAndHeight && value <= MaxWidthAndHeight)
                ZoneWidthAndHeight = value;
            else
                throw new ArgumentOutOfRangeException(nameof(value));
        }

        public void SetWoodlands(int value)
        {
            if (value >= MinWoodlands && value <= MaxWoodlands)
                Woodlands = value;
            else
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
        }

        public void SetLakes(int value)
        {
            if (value >= MinLakes && value <= MaxLakes)
                Lakes = value;
            else
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
        }
    }
}