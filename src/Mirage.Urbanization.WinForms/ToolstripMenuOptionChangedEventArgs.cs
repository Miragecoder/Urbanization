using System;

namespace Mirage.Urbanization.WinForms
{
    public class ToolstripMenuOptionChangedEventArgs<TToolstripMenuOption> : EventArgs
        where TToolstripMenuOption : IToolstripMenuOption
    {
        private readonly TToolstripMenuOption _toolstripMenuOption;
        public TToolstripMenuOption ToolstripMenuOption => _toolstripMenuOption;

        public ToolstripMenuOptionChangedEventArgs(TToolstripMenuOption toolstripMenuOption)
        {
            if (toolstripMenuOption == null) throw new ArgumentNullException(nameof(toolstripMenuOption));
            _toolstripMenuOption = toolstripMenuOption;
        }
    }
}