using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using NetCore.AutoRegisterDi;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Prometheus;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Compact;
using TemplateService.Repository;
using TemplateService.SwaggerEnhancements;


void SetupApplicationDependencyInjection(IServiceCollection services)
{
    //******** setting up for AutoDI
    services.RegisterAssemblyPublicNonGenericClasses()
        .AsPublicImplementedInterfaces();
    //********** add manual DI below this line


}

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();
Log.Information("Template Service is starting...");

LogLevelSwitch.MinimumLevel = LogEventLevel.Information;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((ctx, lc) => { lc.WriteTo.Console(); });
    builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
    builder.Services.AddCors();
    builder.Services.AddControllers().AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.Converters.Add(new StringEnumConverter());
        options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
    });
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddOptions();
    builder.Services.AddHealthChecks();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Template Services v1.0",
            Version = "v1",
            Contact = new OpenApiContact
            {
                Email = "template@info.com",
                Name = "Template Service",
                Url = new Uri("https://github.com/ghosh9691/templateservice")
            }
        });
        //add security bits here....
    });
    builder.Services.AddSwaggerGenNewtonsoftSupport();

    // Configure Database here...
    var connString = builder.Configuration.GetConnectionString("Default");
    builder.Services.AddDbContext<TemplateContext>(options =>
    {
        options.UseMySql(connString, new MySqlServerVersion(new Version(8, 0, 21)));
    });

    builder.Services.AddHttpContextAccessor();

    SetupApplicationDependencyInjection(builder.Services);

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    app.UseSerilogRequestLogging();

    //****** HSTS Settings - uncomment if NOT running in a Kubernetes Cluster *******
    //app.UseHsts();
    //*********** END HSTS Settings *******

    app.UseRouting();
    //******* customize CORS policy as needed
    app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
    app.UseHttpMetrics(); //uses Prometheus for metrics

    //******* SWAGGER: Put in if (app.Environment.IsDevelopment) section if not wanting to expose documentation *******
    app.UseSwagger();
    app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Template Web Service 1.0"); });
    //********* END OF SWAGGER ********

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
        endpoints.MapMetrics();
        endpoints.MapHealthChecks("/health");
    });

    //******* Create or Update database (code first database design) *********
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<TemplateContext>();
        db.Database.Migrate();
    }
    
    app.Run();
}
catch (Exception e)
{
    Log.Fatal(e, "Unhandled Exception!");
}
finally
{
    Log.Information("Template Service is shutting down...");
    Log.CloseAndFlush();
}



public partial class Program
{
    public static LoggingLevelSwitch LogLevelSwitch = new LoggingLevelSwitch();
}