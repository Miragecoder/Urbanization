//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;
//using Mirage.Urbanization.Simulation;
//using Mirage.Urbanization.Web;

//namespace Mirage.Urbanization.WinForms
//{
//    public partial class WebServerForm : Form
//    {
//        readonly GameServer _webServer;
//        public WebServerForm(ISimulationSession simulationSession)
//        {
//            InitializeComponent();
//            this.FormClosing += WebServerForm_FormClosing;


//            _webServer = new GameServer(simulationSession, "http://*:80/", false);
//            _webServer.StartServer();
//        }

//        private void WebServerForm_FormClosing(object sender, FormClosingEventArgs e)
//        {
//            _webServer.Dispose();
//        }

//        private void button1_Click(object sender, EventArgs e)
//        {
//            this.Close();
//        }
//    }
//}
