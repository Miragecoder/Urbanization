using System;

namespace Mirage.Urbanization
{
    public class TerraformingOptions
    {
        public const int 
            MaxWoodlands = 150,MinWoodlands = 0,
            MaxWidthAndHeight = 300, MinWidthAndHeight = 50;

        public bool HorizontalRiver { get; set; }
        public bool VerticalRiver { get; set; }

        private int _woodlands = MinWoodlands;
        private int _zoneWidthAndHeight = MinWidthAndHeight;

        public int Woodlands => _woodlands;
        public int ZoneWidthAndHeight => _zoneWidthAndHeight;

        public void SetZoneWidthAndHeight(int value)
        {
            if (value >= MinWidthAndHeight && value <= MaxWidthAndHeight)
                _zoneWidthAndHeight = value;
            else
                throw new ArgumentOutOfRangeException(nameof(value));
        }

        public void SetWoodlands(int value)
        {
            if (value >= MinWoodlands && value <= MaxWoodlands)
                _woodlands = value;
            else
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
        }
    }
}