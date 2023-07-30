/*-----------------------------------------------------------------*\
 *
 * Program.cs
 *   MvcVueTemplate.Server
 *     mvc-vue-template
 *
 * See LICENSE at root directory
 *
 * CREATED: 2023-7-28 07:19 PM
 * AUTHORS: Mohammed Elghamry <elghamry.connect[at]outlook[dot]com>
 *
\*-----------------------------------------------------------------*/

using System;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using MvcVueTemplate.Server.Extensions.HostBuilder;

namespace MvcVueTemplate.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ILogger? logger = null;

            PrimitiveLogger.LogInformation("Starting Application...");

            try
            {
                // ---
                // Build the host
                // ---

                var host = CreateHostBuilder(args).Build();

                // Get the logger
                logger = host.Services.GetRequiredService<ILogger<Program>>();
                if (logger is null)
                {
                    throw new NullReferenceException("Error obtaining logger.");
                }

                // Check configs for Log file and display warning if not set
                var configs = host.Services.GetRequiredService<IConfiguration>();
                if (String.IsNullOrWhiteSpace(configs[Constants.CONFIG_LOG_FILE_KEY]))
                {
                    logger.LogWarning("LogFile is not set.");
                }

                configs = null; // Remove reference for garbage collector to collect

                logger.LogInformation("Host building succeeded.");

                // Run the host!
                host.Run();
            }
            catch (Exception ex)
            {
                if (logger is not null)
                {
                    logger.LogCritical(ex, "Exception occurred while starting the application.");
                }
                else
                {
                    PrimitiveLogger.LogError($"Exception occurred while starting the application: {ex}");
                }
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
                .ConfigureLogging(logging => logging.ClearProviders())
                .UseSerilogLogging();
        }
    }
}
