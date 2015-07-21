using System;
using System.Windows.Forms;

namespace Mirage.Urbanization.WinForms
{
    class FormManager<TForm>
        where TForm : Form
    {
        private readonly Func<TForm> _factory;
        private TForm _currentFormInstance;

        public FormManager(Func<TForm> factory)
        {
            _factory = factory;
        }

        public void Show(IWin32Window parent)
        {
            if (_currentFormInstance == null)
            {
                var instance = _factory();

                instance.Closed += (x, y) =>
                {
                    _currentFormInstance.Dispose();
                    _currentFormInstance = null;
                };

                _currentFormInstance = instance;
                _currentFormInstance.Show(parent);
            }
        }
    }
}