﻿using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace SampleAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();

        //public static IWebHost BuildWebHost(string[] args)
        //{
        //    Log.Logger = new LoggerConfiguration()
        //        .MinimumLevel.Verbose()
        //        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        //        .MinimumLevel.Override("System", LogEventLevel.Warning)
        //        .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
        //        .Enrich.FromLogContext()
        //        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Literate)
        //        .CreateLogger();

        //    return WebHost.CreateDefaultBuilder(args)
        //            .UseStartup<Startup>()
        //            .ConfigureLogging(builder =>
        //            {
        //                builder.ClearProviders();
        //                builder.AddSerilog();
        //            })
        //            .Build();
        //}
    }
}
