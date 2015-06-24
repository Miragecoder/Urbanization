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
        private readonly string _name;

        public string Name { get { return _name; } }

        private readonly Func<Panel, Action, IGraphicsManagerWrapper> _factory;

        public Func<Panel, Action, IGraphicsManagerWrapper> Factory { get { return _factory; } }

        public GraphicsManagerWrapperOption(string name, Func<Panel, Action, BaseGraphicsManagerWrapper> factory)
        {
            _name = name;
            _factory = factory;
        }

        public static IEnumerable<GraphicsManagerWrapperOption> GetOptions()
        {
            yield return new GraphicsManagerWrapperOption("Direct2D", (panel, action) => new SharpDxGraphicsManagerWrapper(panel, action));
            yield return new GraphicsManagerWrapperOption("Buffered graphics", (panel, action) => new BufferedGraphicsManagerWrapper(panel, action));
        }
    }
}