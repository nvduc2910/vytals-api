using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Vytals
{
    public class Program
    {
        
        //protected Program() { }

        public static void Main(string[] args)
        {
            FooBuildWebHost(args).Run();
        }

        public static IWebHost FooBuildWebHost(string[] args)
        {
            return new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .UseSetting("detailedErrors", "true")
                .UseIISIntegration()
                .CaptureStartupErrors(true)
                .Build();
        }
    }


}
