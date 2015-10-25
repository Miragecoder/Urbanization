using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.WebControls;
using Mirage.Urbanization.Simulation.Persistence;
using Image = System.Drawing.Image;

namespace Mirage.Urbanization.Charts
{
    public class GraphDefinition
    {
        private static int WebIdCounter;

        public GraphDefinition(string title, params GraphSeries[] graphSeriesSet)
        {
            Title = title;
            GraphSeriesSet = graphSeriesSet;
        }

        public int WebId { get; } = ++WebIdCounter;

        public string Title { get; }
        public IReadOnlyCollection<GraphSeries> GraphSeriesSet { get; }
    }
}