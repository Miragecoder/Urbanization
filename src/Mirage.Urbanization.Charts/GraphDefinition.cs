using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.WebControls;
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
        public GraphDefinition(string title, params GraphSeries[] graphSeriesSet) : this(title, null, graphSeriesSet)
        {
        }

        public GraphDefinition(string title, DataMeter dataMeter, params GraphSeries[] graphSeriesSet)
        {
            DataMeter = QueryResult<DataMeter>.Create(dataMeter); 
            Title = title;
            GraphSeriesSet = graphSeriesSet;
        }

        public int WebId { get; } = ++WebIdCounter;

        public string Title { get; }
        public IReadOnlyCollection<GraphSeries> GraphSeriesSet { get; }
    }
}