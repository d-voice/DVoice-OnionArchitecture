using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using OnionArchitecture.API.Middlewares;
using OnionArchitecture.Application;
using OnionArchitecture.Infrastructure;
using OnionArchitecture.Infrastructure.Configurations;
using OnionArchitecture.Persistence;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
// secret.json dosyasını ekliyoruz
builder.Configuration.AddJsonFile("secret.json", optional: true, reloadOnChange: true);

// AwsSettings'i konfigürasyon ile bağlıyoruz
builder.Services.Configure<AwsSettings>(builder.Configuration.GetSection("AwsSettings"));




builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration));

ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

Configure(app, app.Environment);

app.Run();

void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    //Application Services
    services.AddApplication(configuration);
    
    //Infrastructure Services
    services.AddInfrastructure(configuration);
    
    //Persistence Services
    services.AddPersistence(configuration);

    services.AddMemoryCache();
    services.AddEndpointsApiExplorer();
    services.AddControllers();
    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "OnionArchitecture.API",
            Version = "v1"
        });
    
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement{
        {
            new OpenApiSecurityScheme{
                Reference = new OpenApiReference{
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            }, new List<string>()
        }});
    });
}

void Configure(WebApplication app, IWebHostEnvironment env)
{
    if (env.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseRouting();
    app.UseMiddleware<RequestLogContextMiddleware>();
    app.UseMiddleware<GlobalExceptionHandler>();
    app.MapControllers();
    
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        services.AddSeeds();
    }
}