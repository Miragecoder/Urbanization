using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Mirage.Urbanization.Simulation;
using Topshelf;

namespace Mirage.Urbanization.Web
{
    public class Program
    {
        private static string WebCityFileName => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\webcity.xml";
        public static void Main()
        {
            Logger.Instance.OnLogMessage += Instance_OnLogMessage;

            HostFactory.Run(x =>
            {
                var citySaveStateController = new CitySaveStateController(z => { });

                SimulationSession simulationSession;

                try
                {
                    var persistedSimulation = citySaveStateController
                        .LoadFile(WebCityFileName);
                    simulationSession =
                        new SimulationSession(new SimulationOptions(persistedSimulation,
                            new ProcessOptions(() => false, () => false)));
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogException(ex, $"Loading file: '{new FileInfo(WebCityFileName).FullName}'", 100);

                    simulationSession = new SimulationSession(
                        new SimulationOptions(new Func<TerraformingOptions>(() =>
                        {
                            var t = new TerraformingOptions();
                            t.HorizontalRiver = t.VerticalRiver = true;
                            t.SetWoodlands(80);
                            t.SetZoneWidthAndHeight(120);
                            return t;
                        })(), new ProcessOptions(() => false, () => false)));
                }

                x.Service<GameServer>(s =>
                {
                    s.ConstructUsing(name => new GameServer(simulationSession, "http://+:80/", true));
                    s.WhenStarted(tc =>
                    {
                        tc.StartServer();
                        simulationSession.StartSimulation();
                    });
                    s.WhenStopped(tc =>
                    {
                        citySaveStateController.SaveFile(WebCityFileName, tc.SimulationSession.GeneratePersistedArea());
                        tc.Dispose();
                    });
                });
                x.RunAsLocalSystem();

                x.SetDescription("Hosts the Urbanization web server on TCP Port 80 on all network devices. (For more information, see: https://github.com/Miragecoder/Urbanization)");
                x.SetDisplayName("Urbanization Web Server");
                x.SetServiceName("Urbanization");
            });
        }

        private static void Instance_OnLogMessage(object sender, LogEventArgs e)
        {
            Console.WriteLine(e.CreatedOn + " -" + e.LogMessage);
        }
    }
}