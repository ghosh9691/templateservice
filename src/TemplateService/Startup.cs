using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetCore.AutoRegisterDi;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Prometheus;
using Serilog;
using TemplateService.Repository;
using TemplateService.SwaggerEnhancements;

namespace TemplateService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
            services.AddCors();
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            });
            services.AddOptions();
            services.AddHealthChecks();
            services.AddSwaggerGen(c =>
            {
                c.OperationFilter<AddTransactionHeaderParameter>();
            });
            // Configure Database here...
            var connString = Configuration.GetConnectionString("Default");
            services.AddDbContext<TemplateContext>(options =>
            {
                options.UseMySql(connString, new MySqlServerVersion(new Version(8, 0, 21)));
            });

            services.AddHttpContextAccessor();
            SetupDI(services);
        }

        private void SetupDI(IServiceCollection services)
        {
            services.RegisterAssemblyPublicNonGenericClasses()
                .AsPublicImplementedInterfaces();
                
            // setup any other DI here... (most normal DI should be taken care of by the statement above)
            // only those that are different, for example, an abstract class, or a generic class will need
            // to be manually added to the DI container. Do that below...
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, TemplateContext db)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSerilogRequestLogging();
            app.UseRouting();
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            app.UseHttpMetrics();


            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Template Web Service 1.0");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapMetrics();
                endpoints.MapHealthChecks("/health");
            });

            // uncomment the below to apply db migrations
            db.Database.Migrate();
        }
    }
}