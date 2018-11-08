using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Splunk;
using System;
using System.IO;

namespace KnotDiary.ApiGateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((context, configuration) =>
                {
                    var ocelotConfig = context.HostingEnvironment.EnvironmentName == "Release" ? 
                        $"ocelot.{context.HostingEnvironment.EnvironmentName}.json" 
                        : "ocelot.json";

                    configuration.SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", true, true)
                        .AddJsonFile("ocelot.json")
                        .AddJsonFile(ocelotConfig, true, true)
                        .AddEnvironmentVariables();
                })
                .UseSerilog((context, logger) =>
                {
                    var splunkToken = context.Configuration.GetValue<string>("Logging:SplunkToken");
                    var splunkUrl = context.Configuration.GetValue<string>("Logging:SplunkCollectorUrl");
                    var splunkFormatter = new CompactSplunkJsonFormatter(true, context.HostingEnvironment.EnvironmentName, "api_log", Environment.MachineName);

                    logger.Enrich.FromLogContext()
                        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                        .WriteTo.EventCollector(splunkUrl, splunkToken, splunkFormatter)
                        .WriteTo.Console();
                })
                .ConfigureLogging((hostingContext, builder) =>
                {
                    builder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    builder.AddConsole();
                    builder.AddDebug();
                })
                .UseStartup<Startup>();
    }
}
