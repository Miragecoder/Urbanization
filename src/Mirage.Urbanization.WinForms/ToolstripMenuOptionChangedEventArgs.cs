using System;

namespace Mirage.Urbanization.WinForms
{
    public class ToolstripMenuOptionChangedEventArgs<TToolstripMenuOption> : EventArgs
        where TToolstripMenuOption : class, IToolstripMenuOption
    {
        public TToolstripMenuOption ToolstripMenuOption { get; }

        public ToolstripMenuOptionChangedEventArgs(TToolstripMenuOption toolstripMenuOption)
        {
            ToolstripMenuOption = toolstripMenuOption ?? throw new ArgumentNullException(nameof(toolstripMenuOption));
        }
    }
}