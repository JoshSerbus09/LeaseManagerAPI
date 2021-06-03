using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using System;

namespace LeaseManagerAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var nLog = NLogBuilder.ConfigureNLog("Nlog.config").GetCurrentClassLogger();

            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                nLog.Error($"unable to start program\n - {ex}");
                throw;
            }
            finally
            { 
                nLog.Factory.Shutdown(); 
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging(log =>
                {
                    log.ClearProviders();
                    log.SetMinimumLevel(LogLevel.Trace);
                })
                .UseNLog();
    }
}
