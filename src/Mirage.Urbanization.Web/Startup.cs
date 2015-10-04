using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Mirage.Urbanization.Simulation;
using Owin;
using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

[assembly: OwinStartup(typeof(Mirage.Urbanization.Web.Startup))]

namespace Mirage.Urbanization.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Use(async (context, next) =>
            {
                if (context.Request.Path.Value.StartsWith("/tile/"))
                {
                    try
                    {
                        var bitmap = TilesetProvider.GetBitmapForHashcode(Convert.ToInt32(
                            context.Request.Path.Value.Split('/').Last()));

                        using (var stream = new MemoryStream())
                        {
                            bitmap.Save(stream, ImageFormat.Png);
                            context.Response.ContentType = "image/png";
                            await context.Response.WriteAsync(stream.ToArray());
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.WriteLine("An unhandled exception occurred whilst processing tile image request: " + ex);
                    }
                    return;
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
