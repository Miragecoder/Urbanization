using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Mirage.Urbanization.WinForms.Rendering;

namespace Mirage.Urbanization.WinForms
{
    public class GraphicsManagerWrapperChangedEventArgs : EventArgs
    {
        private readonly GraphicsManagerWrapperOption _graphicsManagerWrapperOption;
        public GraphicsManagerWrapperOption GraphicsManagerWrapperOption { get { return _graphicsManagerWrapperOption; } }

        public GraphicsManagerWrapperChangedEventArgs(GraphicsManagerWrapperOption graphicsManagerWrapperOption)
        {
            if (graphicsManagerWrapperOption == null) throw new ArgumentNullException("graphicsManagerWrapperOption");
            _graphicsManagerWrapperOption = graphicsManagerWrapperOption;
        }
    }

    public class GraphicsManagerSelection
    {
        public GraphicsManagerSelection(ToolStripMenuItem targetToopToolStripMenuItem)
        {
            foreach (var option in GraphicsManagerWrapperOption.GetOptions())
            {
                var localOption = option;
                var item = new ToolStripMenuItem(option.Name);
                item.Click += (sender, e) =>
                {
                    foreach (var x in targetToopToolStripMenuItem.DropDownItems.Cast<ToolStripMenuItem>())
                        x.Checked = false;
                    item.Checked = true;
                    currentOption = localOption;
                    if (OnSelectionChanged != null)
                        OnSelectionChanged(this, new GraphicsManagerWrapperChangedEventArgs(currentOption));
                };

                targetToopToolStripMenuItem.DropDownItems.Add(item);
                item.PerformClick();
            }
        }

        public event EventHandler<GraphicsManagerWrapperChangedEventArgs> OnSelectionChanged;

        private GraphicsManagerWrapperOption currentOption;

        public GraphicsManagerWrapperOption GetCurrentOption() { return currentOption; }
    }
}