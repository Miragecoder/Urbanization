using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using Mirage.Urbanization.Simulation.Datameters;
using Mirage.Urbanization.Simulation.Persistence;
using Mirage.Urbanization.ZoneStatisticsQuerying;
using Image = System.Drawing.Image;

namespace Mirage.Urbanization.Charts
{
    public class GraphDefinition
    {
        public QueryResult<DataMeter> DataMeter { get; }
        private static int WebIdCounter;
        public GraphDefinition(string title, bool isCurrency, params GraphSeries[] graphSeriesSet) : this(title, isCurrency, null, graphSeriesSet)
        {
        }

        public GraphDefinition(string title, bool isCurrency, DataMeter dataMeter, params GraphSeries[] graphSeriesSet)
        {
            DataMeter = QueryResult<DataMeter>.Create(dataMeter);
            Title = title;
            IsCurrency = isCurrency;
            GraphSeriesSet = graphSeriesSet;
        }

        public int WebId { get; } = ++WebIdCounter;

        public string Title { get; }
        public bool IsCurrency { get; }
        public IReadOnlyCollection<GraphSeries> GraphSeriesSet { get; }
    }
}