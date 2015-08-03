using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;

[assembly: OwinStartup(typeof(Mirage.Urbanization.Web.Startup))]

namespace Mirage.Urbanization.Web
{
    public class FileSystem : IFileSystem
    {
        public bool TryGetFileInfo(string subpath, out IFileInfo fileInfo)
        {
            throw new NotImplementedException();
        }

        public bool TryGetDirectoryContents(string subpath, out IEnumerable<IFileInfo> contents)
        {
            throw new NotImplementedException();
        }
    }

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseErrorPage();
            app.MapSignalR();
            app.UseFileServer(new FileServerOptions()
            {
                FileSystem = new EmbeddedResourceFileSystem(Assembly.GetAssembly(GetType()), "Mirage.Urbanization.Web.Www")
            });

            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
        }
    }
}
