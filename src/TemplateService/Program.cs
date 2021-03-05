using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace TemplateService
{
    public class Program
    {
        public static LoggingLevelSwitch LogLevelSwitch = new LoggingLevelSwitch();


        public static void Main(string[] args)
        {
            LogLevelSwitch.MinimumLevel = LogEventLevel.Information;    //default

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(LogLevelSwitch)
                .Enrich.FromLogContext()
                .Enrich.WithCorrelationIdHeader("request-id")
                .Filter.ByExcluding(c => c.Properties.Any(p =>
                    p.Value.ToString().Contains("health") || p.Value.ToString().Contains("metrics")))
                .WriteTo.Console(new RenderedCompactJsonFormatter())
                .CreateLogger();
            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal($"Startup Failed: {ex.Message}");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddEnvironmentVariables();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
            .UseSerilog();
    }
}
