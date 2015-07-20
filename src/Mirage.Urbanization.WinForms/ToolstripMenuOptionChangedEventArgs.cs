using System;

namespace Mirage.Urbanization.WinForms
{
    public class ToolstripMenuOptionChangedEventArgs<TToolstripMenuOption> : EventArgs
        where TToolstripMenuOption : IToolstripMenuOption
    {
        public TToolstripMenuOption ToolstripMenuOption { get; }

        public ToolstripMenuOptionChangedEventArgs(TToolstripMenuOption toolstripMenuOption)
        {
            if (toolstripMenuOption == null) throw new ArgumentNullException(nameof(toolstripMenuOption));
            ToolstripMenuOption = toolstripMenuOption;
        }
    }
}