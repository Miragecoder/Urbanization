using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirage.Urbanization.Web
{
    static class Program
    {
        static void Main(string[] args)
        {
            using (Microsoft.Owin.Hosting.WebApp.Start<Startup>("http://localhost:9000"))
            {
                Console.WriteLine("Press [escape] to quit...");
                while (Console.ReadKey().Key != ConsoleKey.Escape) {}
            }
        }
    }
}
