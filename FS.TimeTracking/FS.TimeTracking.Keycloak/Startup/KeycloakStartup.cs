﻿using FS.TimeTracking.Core.Models.Configuration;
using FS.TimeTracking.Keycloak.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;

namespace FS.TimeTracking.Keycloak.Startup;

internal static class KeycloakStartup
{
    public static IServiceCollection RegisterKeycloakAuthentication(this IServiceCollection services, TimeTrackingConfiguration configuration)
    {
        if (!configuration.Features.Authorization)
            return services;

        var keycloakConfiguration = configuration.Keycloak;
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = $"{keycloakConfiguration.AuthServerUrl}/realms/{keycloakConfiguration.Realm}";
                options.Audience = keycloakConfiguration.ClientId;
                options.RequireHttpsMetadata = keycloakConfiguration.SslRequired;
                //options.AllowJwtTokenFromQueryParam();
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // https://stackoverflow.com/a/53627747, 
                    // https://nikiforovall.github.io/aspnetcore/dotnet/2022/08/24/dotnet-keycloak-auth.html
                    ValidateAudience = keycloakConfiguration.VerifyTokenAudience,
                    NameClaimType = "preferred_username",
                };
            });

        services.AddScoped<RealmRoleTransformation, RealmRoleTransformation>();
        services.AddScoped<ClientRoleTransformation, ClientRoleTransformation>();
        services.AddScoped<RptRoleTransformation, RptRoleTransformation>();
        services.AddScoped<IClaimsTransformation, KeycloakJwtTransformation>();

        return services;
    }

    private static void AllowJwtTokenFromQueryParam(this JwtBearerOptions options)
        => options.Events = new JwtBearerEvents
        {
            OnMessageReceived = static context =>
            {
                if (context.Request.Query.TryGetValue("access_token", out var token))
                    context.Token = token;
                return Task.CompletedTask;
            }
        };
}