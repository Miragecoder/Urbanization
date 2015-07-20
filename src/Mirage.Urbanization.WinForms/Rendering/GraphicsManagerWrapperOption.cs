using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using Mirage.Urbanization.WinForms.Rendering.BufferedGraphics;
using Mirage.Urbanization.WinForms.Rendering.SharpDx;

namespace Mirage.Urbanization.WinForms.Rendering
{
    public class GraphicsManagerWrapperOption : IToolstripMenuOption
    {
        public string Name { get; }

        public Func<Panel, Action, IGraphicsManagerWrapper> Factory { get; }

        public GraphicsManagerWrapperOption(string name, Func<Panel, Action, BaseGraphicsManagerWrapper> factory)
        {
            Name = name;
            Factory = factory;
        }

        public static IEnumerable<GraphicsManagerWrapperOption> GetOptions()
        {
            yield return new GraphicsManagerWrapperOption("Buffered graphics", (panel, action) => new BufferedGraphicsManagerWrapper(panel, action));
            yield return new GraphicsManagerWrapperOption("Direct2D", (panel, action) => new SharpDxGraphicsManagerWrapper(panel, action));
        }
    }
}