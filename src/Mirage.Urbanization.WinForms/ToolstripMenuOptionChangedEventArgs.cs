using System;

namespace Mirage.Urbanization.WinForms
{
    public class ToolstripMenuOptionChangedEventArgs<TToolstripMenuOption> : EventArgs
        where TToolstripMenuOption : IToolstripMenuOption
    {
        private readonly TToolstripMenuOption _toolstripMenuOption;
        public TToolstripMenuOption ToolstripMenuOption { get { return _toolstripMenuOption; } }

        public ToolstripMenuOptionChangedEventArgs(TToolstripMenuOption toolstripMenuOption)
        {
            if (toolstripMenuOption == null) throw new ArgumentNullException("toolstripMenuOption");
            _toolstripMenuOption = toolstripMenuOption;
        }
    }
}