using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mirage.Urbanization.WinForms
{
    public partial class LogWindow : Form
    {
        public LogWindow()
        {
            InitializeComponent();

            Mirage.Urbanization.Logger.Instance.OnLogMessage += Instance_OnLogMessage;

            this.FormClosing += (sender, e) =>
            {
                Mirage.Urbanization.Logger.Instance.OnLogMessage -= Instance_OnLogMessage;
            };
        }

        private void Instance_OnLogMessage(object sender, LogEventArgs e)
        {
            if (!textBox1.IsHandleCreated)
            {
                return;
            }
            textBox1.BeginInvoke(new MethodInvoker(() =>
            {
                textBox1.AppendText($"{e.CreatedOn.ToLongTimeString()}: {e.LogMessage}" + Environment.NewLine);
            }));
        }
    }
}
