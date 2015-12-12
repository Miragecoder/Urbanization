using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Mirage.Urbanization.Charts;
using Mirage.Urbanization.Simulation;
using Owin;
using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

[assembly: OwinStartup(typeof(Mirage.Urbanization.Web.Startup))]

namespace Mirage.Urbanization.Web
{
    public class Startup
    {
        private static readonly Lazy<IChartDrawer> ChartDrawer = new Lazy<IChartDrawer>(Mirage.Urbanization.Charts.ChartDrawerFactory.Create);

        static async Task ServeImage(IOwinContext context, byte[] bytes)
        {
            context.Response.ContentType = "image/png";
            await context.Response.WriteAsync(bytes);
        }

        static async Task ServeImage(IOwinContext context, Image image)
        {
            try
            {
                using (var stream = new MemoryStream())
                {
                    image.Save(stream, ImageFormat.Png);
                    await ServeImage(context, stream.ToArray());
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteLine("An unhandled exception occurred whilst processing tile image request: " + ex);
            }
        }

        public void Configuration(IAppBuilder app)
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
                else if (context.Request.Path.Value.StartsWith("/tile/"))
                {
                    var bitmap = TilesetProvider.GetBitmapForId(Convert.ToInt32(
                        context.Request.Path.Value.Split('/').Last()));

                    await ServeImage(context, bitmap.PngBytes);
                    return;
                }
                else if (context.Request.Path.Value.StartsWith("/citysize/"))
                {
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(
                        Encoding.UTF8.GetBytes($"{{ \"width\": {GameServer.Instance.SimulationSession.Area.AmountOfZonesX}, \""
                        + $"height\": {GameServer.Instance.SimulationSession.Area.AmountOfZonesY} }}"));
                }
                await next();
            });


            app.UseErrorPage();
            app.MapSignalR(new HubConfiguration() { EnableDetailedErrors = true });
            app.UseFileServer(new FileServerOptions()
            {
                FileSystem = new EmbeddedResourceFileSystem(Assembly.GetAssembly(GetType()), "Mirage.Urbanization.Web.Www"),
                EnableDirectoryBrowsing = true
            });

            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
        }
    }
}
