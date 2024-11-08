using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnionArchitecture.Application.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using OnionArchitecture.Application.Abstractions.Services;
using OnionArchitecture.Infrastructure.Services;
using OnionArchitecture.Application.Abstractions.Services.AwsBedrock;
using OnionArchitecture.Infrastructure.Services.AwsBedrock;
using OnionArchitecture.Infrastructure.Services.AwsPolly;
using OnionArchitecture.Application.Abstractions.Services.AwsPolly;
using OnionArchitecture.Application.Abstractions.Services.AwsTranscribe;
using OnionArchitecture.Infrastructure.Services.AwsTranscribe;
using OnionArchitecture.Infrastructure.Configurations;

namespace OnionArchitecture.Infrastructure;

public static class ServiceRegistration
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddJwtAuthentication(configuration);
        services.AddApplicationServices();
    }
    
    private static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AwsSettings>(configuration.GetSection("AwsSettings"));
        var jwtOptions = configuration.GetSection(JwtOptions.OptionKey).Get<JwtOptions>()!;

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                
                    ValidAudience = jwtOptions.Audience,
                    ValidIssuer = jwtOptions.Issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecurityKey)),
                    LifetimeValidator = (notBefore, expires, securityToken, validationParameters) =>
                        expires != null ? expires > DateTime.UtcNow : false
                };
            });
    }
    
    private static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<IPasswordHasher, PasswordHasher>();
        services.AddTransient<ITokenService, TokenService>();
        services.AddTransient<IAwsBedrockService, AwsBedrockService>();
        services.AddTransient<IAwsPollyService, AwsPollyService>();
        services.AddTransient<IAwsTranscribeService, AwsTranscribeService>();
    }
}