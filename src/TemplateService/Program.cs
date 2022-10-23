using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TemplateService.Auth0;
using TemplateService.Repository;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerUI;



Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Pyxis International Template Service starting up...");

try
{
    var builder = WebApplication.CreateBuilder(args);

    //setup serilog
    builder.Host.UseSerilog((ctx, lc) => { lc.WriteTo.Console(); });

    //read connection string from environment (Heroku) or from AppSettings (local or other platforms)
    
    // Add services to the container.
    builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
    builder.Services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();
    var domain = builder.Configuration["Auth0:Authority"];
    builder.Services.AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(x =>
    {
        x.Authority = domain;
        x.Audience = builder.Configuration["Auth0:ApiIdentifier"];
    });

    builder.Services.AddAuthorization(x =>
    {
        x.AddPolicy("role:superadmin",
            policy => policy.Requirements.Add(new HasScopeRequirement("role:superadmin", domain)));
        x.AddPolicy("role:airlineadmin",
            policy => policy.Requirements.Add(new HasScopeRequirement("role:airlineadmin", domain)));
        x.AddPolicy("role:airlineuser",
            policy => policy.Requirements.Add(new HasScopeRequirement("role:airlineuser", domain)));
    });
    builder.Services.AddCors();
    builder.Services.AddControllers().AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.Converters.Add(new StringEnumConverter());
        options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    });
    builder.Services.AddOptions();
    builder.Services.AddHealthChecks();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Pyxis International Template Service v1.0",
            Version = "v1",
            Contact = new OpenApiContact
            {
                Email = "info@pyxisint.com",
                Name = "Pyxis International",
                Url = new Uri("https://www.pyxisint.com")
            }
        });
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter Bearer followed by a JWT token here...",
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] { }
            }
        });
    });
    builder.Services.AddSwaggerGenNewtonsoftSupport();

    //setup database connection
    var connString = builder.Configuration.GetConnectionString("Default");
    builder.Services.AddDbContext<TemplateDbContext>(options =>
    {
        options.UseMySql(connString,
            new MySqlServerVersion(new Version(8, 0, 21)),
            b => b.MigrationsAssembly("TemplateService"));
        options.EnableSensitiveDataLogging();
    });

    builder.Services.AddHttpContextAccessor();

    //setup application dependencies
    AddApplicationDependencies(builder.Services);


    //*********************** Setup App *****************************

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment()) app.UseDeveloperExceptionPage();

    app.UseSerilogRequestLogging();
    app.UseRouting();
    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
    app.UseStaticFiles();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pyxis International Template Service 1.0");
        c.DefaultModelRendering(ModelRendering.Model);
        c.DisplayOperationId();
        c.DisplayRequestDuration();
    });
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
        endpoints.MapHealthChecks("/health");
    });

    //apply database migrations
    using (var scope = app.Services.CreateScope())
    {
        var dc = scope.ServiceProvider.GetRequiredService<TemplateDbContext>();
        dc.Database.Migrate();
    }

    Log.Information("Application startup completed!");
    app.Run();
}
catch (Exception e)
{
    Log.Fatal(e, "Unhandled Exception!");
}
finally
{
    Log.Information("Pyxis International Template Service shutting down...");
    Log.CloseAndFlush();
}


void AddApplicationDependencies(IServiceCollection services)
{
}