using Mirage.Urbanization.Simulation.Datameters;
using Mirage.Urbanization.ZoneStatisticsQuerying;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Urbanization.Tilesets
{
    public class DatameterColourDefinitions
    {
        public static DatameterColourDefinitions Instance { get; } = new DatameterColourDefinitions();

        private static readonly Dictionary<DataMeterValueCategory, Color> DataMeterValueBrushes = new Dictionary<DataMeterValueCategory, Color>
        {
            { DataMeterValueCategory.Low, Color.FromRgba(150, 200, 200, 190)},
            { DataMeterValueCategory.Medium, Color.FromRgba(70, 50, 180, 190)},
            { DataMeterValueCategory.High, Color.FromRgba(180, 0, 180, 190)},
            { DataMeterValueCategory.VeryHigh, Color.FromRgba(250, 0, 50, 190)}
        };

        public Color? GetColorFor(DataMeterValueCategory dataMeterValueCategory)
        {
            if (DataMeterValueBrushes.ContainsKey(dataMeterValueCategory))
                return DataMeterValueBrushes[dataMeterValueCategory];
            return null;
        }
    }
}
