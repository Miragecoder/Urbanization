using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Mirage.Urbanization.Simulation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Mirage.Urbanization.Web
{
    public class UrbanizationService : BackgroundService
    {
        private readonly CitySaveStateController _citySaveStateController = new CitySaveStateController(z => { });
        private GameServer _gameServer;
        private static string WebCityFileName => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\webcity.xml";

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            SimulationSession simulationSession;

            try
            {
                var persistedSimulation = _citySaveStateController
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
                        t.SetLakes(30);
                        t.SetZoneWidthAndHeight(120);
                        return t;
                    })(), new ProcessOptions(() => false, () => false)));
            }
            simulationSession.StartSimulation();
            _gameServer = new GameServer(simulationSession, "http://+:80/", true);
            _gameServer.StartServer();
            return Task.CompletedTask;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
            }
            catch (OperationCanceledException) { }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _citySaveStateController.SaveFile(WebCityFileName, _gameServer.SimulationSession.GeneratePersistedArea());
            _gameServer.Dispose();
            return Task.CompletedTask;
        }
    }
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Logger.Instance.OnLogMessage += Instance_OnLogMessage;
            await CreateHostBuilder(args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureAppConfiguration((context, config) =>
                {
                    // Configure the app here.
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<UrbanizationService>();
                })
                // Only required if the service responds to requests.
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static void Instance_OnLogMessage(object sender, LogEventArgs e)
        {
            Console.WriteLine(e.CreatedOn + " -" + e.LogMessage);
        }
    }
}