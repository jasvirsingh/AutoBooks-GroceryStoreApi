using GroceryStore.Data;
using GroceryStore.Data.Access.Interfaces;
using GroceryStore.Data.Access.Repo;
using GroceryStore.Services;
using GroceryStore.Services.Interfaces;
using GroceryStoreAPI.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace GroceryStoreAPI
{
    public class Startup
    {
        /// <summary>Initializes a new instance of the <see cref="Startup" /> class.</summary>
        /// <param name="env">The env.</param>
        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        /// <summary>Gets the service provider.</summary>
        /// <value>The service provider.</value>
        public IServiceProvider ServiceProvider { get; set; }

        /// <summary>Gets or sets the service preprocessor.</summary>
        /// <value>The service preprocessor.</value>
        public Action<IServiceCollection> ServiceCollection { get; set; }

        /// <summary>Gets the configuration.</summary>
        /// <value>The configuration.</value>
        public IConfiguration Configuration { get; }

        /// <summary>Configures the services.</summary>
        /// <param name="services">The services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            // ----------------------
            // Setting up MVC Options
            // ----------------------
            services
                .AddControllers()
                .AddJsonOptions((o) =>
                {
                    o.JsonSerializerOptions.AllowTrailingCommas = true;
                    o.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
                });

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen();

            // ---------------------------
            // Setting up API MVC Settings
            // ---------------------------
            services
                .AddLogging()

                // Disbaling modlestate error filter
                // to get full control of error handling
                .Configure<ApiBehaviorOptions>(options =>
                {
                    options.SuppressModelStateInvalidFilter = true;
                })

                // Used so we don't have to access the HttpContext directly
                // * Helps with unit tests and mocking
                .AddHttpContextAccessor()

                 // Adding Cors support TODO: More work needed here
                .AddCors((o) =>
                {
                    // TODO: Need to modify this to not be so wide-open
                    o.AddPolicy("Default", (p) =>
                    {
                        p.AllowAnyOrigin()
                            .AllowCredentials()
                            .AllowAnyHeader()
                            .SetPreflightMaxAge(TimeSpan.FromSeconds(600))
                            .Build();
                    });
                });

            services.AddTransient<CustomerController>();

            // Services
            services.AddTransient<ICustomerService, CustomerService>();

            // Repositories
            services.AddTransient<ICustomerRepository, CustomerJsonRepository>();

            // A pre-processor hook is provided to allow for unit tests to 
            // pre configure/replace services prior to the service provider being built
            if (ServiceCollection != null)
            {
                ServiceCollection(services);
            }
        }

        /// <summary>Configures the specified application.</summary>
        /// <param name="app">The application.</param>
        /// <param name="env">The env.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsEnvironment("Local"))
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                // Registering a local copy of the IServiceProvider here so it can be 
                // used in the post configure methods of the various configuration sections
                this.ServiceProvider = app.ApplicationServices;
            });

            app.UseExceptionHandler((builder) =>
            {
                builder.Run(async (context) =>
                {
                    var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (exceptionHandlerFeature?.Error != null)
                    {
                        await OnException(context: context, ex: exceptionHandlerFeature.Error).ConfigureAwait(false);
                    }
                });
            });
        }

        /// <summary>Called when [exception].</summary>
        /// <param name="context">The context.</param>
        /// <param name="ex">The ex.</param>
        /// <returns></returns>
        private Task OnException(HttpContext context, Exception ex)
        {
            try
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var logger = context.RequestServices.GetService<ILogger<Startup>>();

                if (ex is OperationCanceledException)
                {
                    // Issuing non-standard status code when the client 
                    // has disconnected (This is based off of nginx)
                    context.Response.StatusCode = 499;
                    context.Features.Get<IHttpResponseFeature>().ReasonPhrase = "Client Closed Request";
                    logger.LogWarning(ex, "The client disconnected");
                }
                else
                {
                    logger.LogError(ex, "An unhandled exception occured.");
                }
            }
            catch (Exception) { }

            return Task.CompletedTask;
        }
    }
}