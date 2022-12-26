using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DevArkStudio.Presentation
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            // Console.WriteLine("Рабочая папка");
            // Console.WriteLine(System.IO.Directory.GetCurrentDirectory());
            // Console.WriteLine("Папки внутри");
            // foreach (var dir in System.IO.Directory.GetDirectories(System.IO.Directory.GetCurrentDirectory()))
            // {
            //     Console.WriteLine(dir);
            // }
            // foreach (var arg in args)
            // {
            //     Console.WriteLine(arg);
            // }
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>(); 
                });
    }
}