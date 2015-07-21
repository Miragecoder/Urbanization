using System;
using System.Windows.Forms;

namespace Mirage.Urbanization.WinForms
{
    class SimulationRenderHelperFormManager<TForm>
        where TForm : Form
    {
        private readonly Func<SimulationRenderHelper, TForm> _factory;
        private TForm _currentFormInstance;

        public SimulationRenderHelperFormManager(Func<SimulationRenderHelper, TForm> factory)
        {
            _factory = factory;
        }

        public void Show(IWin32Window parent, SimulationRenderHelper helper)
        {
            helper
                .SimulationSession
                .GetRecentStatistics()
                .WithResultIfHasMatch(statistics =>
                {
                    if (_currentFormInstance == null)
                    {
                        var instance = _factory(helper);

                        helper.Stopping += (x, y) => instance.Close();

                        instance.Closed += (x, y) =>
                        {
                            _currentFormInstance.Dispose();
                            _currentFormInstance = null;
                        };

                        _currentFormInstance = instance;
                        _currentFormInstance.Show(parent);
                    }
                });
        }
    }
}