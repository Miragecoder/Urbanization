using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Mirage.Urbanization.Charts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.SignalR;

namespace Mirage.Urbanization.Web
{
    public class Startup
    {
        private static readonly Lazy<IChartDrawer> ChartDrawer = new Lazy<IChartDrawer>(Mirage.Urbanization.Charts.ChartDrawerFactory.Create);

        static async Task ServeImage(HttpContext context, byte[] bytes)
        {
            context.Response.ContentType = "image/png";
            await context.Response.BodyWriter.WriteAsync(bytes);
        }

        static async Task ServeImage(HttpContext context, Image image)
        {
            try
            {
                using var stream = new MemoryStream();
                image.Save(stream, ImageFormat.Png);
                await ServeImage(context, stream.ToArray());
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteLine("An unhandled exception occurred whilst processing tile image request: " + ex);
            }
        }

        private static Func<IHubContext<SimulationHub>> _getSimulationHub;
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();
            services.AddSignalRCore();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            app.Use(async (context, next) =>
            {
                if (context.Request.Path.Value.StartsWith("/graph/"))
                {
                    await context
                        .Request
                        .Path
                        .Value
                        .Split('/')
                        .Reverse()
                        .Skip(1)
                        .First()
                        .Pipe(x => Convert.ToInt32(x))
                        .Pipe(graphWebId => GraphDefinitions
                            .Instances
                            .Single(x => x.WebId == graphWebId)
                        )
                        .Pipe(graphDefinition => ChartDrawer.Value.Draw(
                            graphDefinition,
                            GameServer.Instance.SimulationSession.GetAllCityStatistics(),
                            SystemFonts.DefaultFont, new Size(600, 250))
                        )
                        .Pipe(image => ServeImage(context, image));
                    return;
                }
                else if (context.Request.Path.Value.StartsWith("/tileset/"))
                {
                    await ServeImage(context, TilesetProvider.TextureAtlas.GetAtlasBytes());
                }
                else if (context.Request.Path.Value.StartsWith("/cityinfo/"))
                {
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(
                        $"{{ \"mapWidth\": {GameServer.Instance.SimulationSession.Area.AmountOfZonesX}, "
                        + $"\"mapHeight\": {GameServer.Instance.SimulationSession.Area.AmountOfZonesY}, "
                        + $"\"cellsPerAtlasRow\": { TextureAtlas.CellsPerRow }, "
                        + $"\"vehicleTilesPerRow\": { TextureAtlas.VehicleTilesPerRow }, "
                        + $"\"cellSpriteOffset\": { TilesetProvider.TextureAtlas.CellSpriteOffset } }}");
                }
                await next();
            });

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<SimulationHub>("/simulationHub");
            });
            app.UseFileServer(new FileServerOptions()
            {
                FileProvider = new ManifestEmbeddedFileProvider(Assembly.GetEntryAssembly(), "Www"),
                EnableDirectoryBrowsing = true
            });
            var hubContext = serviceProvider.GetService<IHubContext<SimulationHub>>();
            _getSimulationHub = new Func<IHubContext<SimulationHub>>(() => hubContext);
        }

        public static IHubContext<SimulationHub> GetSimulationHub() => _getSimulationHub();
    }
}
