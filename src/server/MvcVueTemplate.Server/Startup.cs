/*-----------------------------------------------------------------*\
 *
 * Startup.cs
 *   MvcVueTemplate.Server
 *     mvc-vue-template
 *
 * See LICENSE at root directory
 *
 * CREATED: 2023-7-28 09:58 PM
 * AUTHORS: Mohammed Elghamry <elghamry.connect[at]outlook[dot]com>
 *
\*-----------------------------------------------------------------*/

using System;
using System.IO;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MvcVueTemplate.Server
{
    internal class Startup
    {
        private readonly IConfiguration m_config;
        private readonly IWebHostEnvironment m_env;

        private readonly string m_clientAppPath = default!;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            m_config = configuration;
            m_env = env;

            m_clientAppPath = Path.GetFullPath(Environment.ExpandEnvironmentVariables(
                m_config.GetValue<string>(Constants.CONFIG_CLIENT_APP_PATH_KEY)
                ?? throw new NullReferenceException("Empty client app path")
                ));

            m_env.WebRootFileProvider = new PhysicalFileProvider(m_clientAppPath);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            if (m_env.IsDevelopment())
            {
                services.AddEndpointsApiExplorer();
                services.AddSwaggerGen();
            }

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, ILogger<Startup> logger)
        {
            if (m_env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler();
            }

            if (m_env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            if (!m_env.IsDevelopment())
            {
                // Don't use in development avoiding hassle for certificates
                app.UseHttpsRedirection();
            }

            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(m_clientAppPath)
            });

            app.UseRouting();

            app.UseEndpoints(
                endpoints =>
                {
                    endpoints.MapControllers();

                    endpoints.Map("/error", () => Results.Problem());

                    endpoints.Map("/api/{*path}", async (context) =>
                    {
                        var problemDetailsFactory = context.RequestServices.GetRequiredService<ProblemDetailsFactory>();
                        context.Response.StatusCode = StatusCodes.Status404NotFound;
                        await context.Response.WriteAsJsonAsync(problemDetailsFactory.CreateProblemDetails(
                            context,
                            statusCode: StatusCodes.Status404NotFound
                            ));
                    });

                    // Map fallback to client file
                    endpoints.MapFallbackToFile("index.html");
                });
        }
    }
}
