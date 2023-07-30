/*-----------------------------------------------------------------*\
 *
 * SerilogLoggingHostBuilderExtensions.cs
 *   MvcVueTemplate.Server
 *     mvc-vue-template
 *
 * See LICENSE at root directory
 *
 * CREATED: 2023-7-28 07:50 PM
 * AUTHORS: Mohammed Elghamry <elghamry.connect[at]outlook[dot]com>
 *
\*-----------------------------------------------------------------*/

using System;

using Microsoft.Extensions.Hosting;

using Serilog;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.Destructurers;
using Serilog.Templates;
using Serilog.Templates.Themes;

namespace MvcVueTemplate.Server.Extensions.HostBuilder
{
    internal static class SerilogLoggingHostBuilderExtensions
    {
        // This template is following CLEF format: https://github.com/serilog/serilog-expressions#formatting-with-expressiontemplate
        private const string SERILOG_EXPRESSION_TEMPLATE =
            "{ {@t, @mt, @r, @l: if @l = 'Information' then undefined() else @l, @x, ..@p} }\n";

        /// <summary>
        /// Use configured serilog for host logging.
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <returns>Passed IHostBuilder.</returns>
        public static IHostBuilder UseSerilogLogging(this IHostBuilder hostBuilder)
        {
            hostBuilder.UseSerilog((hostingContext, loggerConfig) =>
            {
                IHostEnvironment env = hostingContext.HostingEnvironment;

                if (env.IsDevelopment())
                {
                    loggerConfig.MinimumLevel.Debug();
                }
                else
                {
                    loggerConfig.MinimumLevel.Information();
                }

                loggerConfig
                    .Enrich.WithProperty(name: "SourceContext", value: "Undefined-Context")
                    .Enrich.FromLogContext()
                    .Enrich.WithExceptionDetails(
                        new DestructuringOptionsBuilder()
                            .WithDefaultDestructurers()
                            .WithDestructurers(new IExceptionDestructurer[]
                            {
                                // TODO: Use destructurers when necessary.
                                //new DbUpdateExceptionDestructurer(),
                                //new SqlExceptionDestructurer()
                            })
                        );

                // --- Add Sinks ---
                // # Console Sink
                if (env.IsDevelopment())
                {
                    // Use a more human readable format in the console while in development,
                    //  CLEF format in production is interpreted by other tools to be readable [refer to comment above].
                    loggerConfig.WriteTo.Console(
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}]: {SourceContext}:{NewLine}" +
                            "\t{Message:lj}{NewLine}{Exception}",
                        theme: Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Code
                        );
                }
                else
                {
                    loggerConfig.WriteTo.Console(
                        formatter: new ExpressionTemplate(SERILOG_EXPRESSION_TEMPLATE, theme: TemplateTheme.Code)
                        );
                }

                // # Debug sink if under development environment
                if (env.IsDevelopment())
                {
                    loggerConfig.WriteTo.Debug(formatter: new ExpressionTemplate(SERILOG_EXPRESSION_TEMPLATE));
                }

                // # File sink if in an environment that isn't development, and log file path is set
                if (!env.IsDevelopment())
                {
                    string? logFile = hostingContext.Configuration[Constants.CONFIG_LOG_FILE_KEY];
                    if (!String.IsNullOrWhiteSpace(logFile))
                    {
                        loggerConfig.WriteTo.File(
                            path: logFile,
                            fileSizeLimitBytes: 26214400, // 25 MB
                            rollOnFileSizeLimit: true,
                            rollingInterval: RollingInterval.Day,
                            retainedFileCountLimit: 5,
                            formatter: new ExpressionTemplate(SERILOG_EXPRESSION_TEMPLATE)
                            );
                    }
                }

                // --- Load configuration from the application and override and set here ---
                loggerConfig.ReadFrom.Configuration(hostingContext.Configuration);
            });

            return hostBuilder;
        }
    }
}
