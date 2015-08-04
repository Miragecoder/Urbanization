using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Mirage.Urbanization.Simulation;
using Owin;

[assembly: OwinStartup(typeof(Mirage.Urbanization.Web.Startup))]

namespace Mirage.Urbanization.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseErrorPage();
            app.MapSignalR(new HubConfiguration());
            app.UseFileServer(new FileServerOptions()
            {
                FileSystem = new EmbeddedResourceFileSystem(Assembly.GetAssembly(GetType()), "Mirage.Urbanization.Web.Www"),
                EnableDirectoryBrowsing = true
            });
            app.UseFileServer(new FileServerOptions()
            {
                DirectoryBrowserOptions =
                {
                    RequestPath = new PathString("/Scripts"),
                },
                FileSystem = new EmbeddedResourceFileSystem(Assembly.GetAssembly(GetType()),
                "Mirage.Urbanization.Web.Scripts"),
                StaticFileOptions = { ServeUnknownFileTypes = true }
            });

            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
        }
    }
}
