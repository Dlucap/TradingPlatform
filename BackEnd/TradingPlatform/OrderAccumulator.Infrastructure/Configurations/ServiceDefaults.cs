using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderAccumulator.Infrastructure.Configurations;
public static class ServiceDefaults
{
    public static IServiceCollection AddServiceDefaults(this IServiceCollection services)
    {
        services.AddTelemetry();

        services.AddDefaultHealthChecks();

        services.AddServiceDiscovery();

        services.ConfigureHttpClientDefaults(http =>
        {
            http.AddStandardResilienceHandler();

            http.AddServiceDiscovery();
        });
        return services;
    }

    public static WebApplication MapDefaultEndpoints(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            // Health check completo - verifica todos os health checks registrados
            app.MapHealthChecks("/health");

            // Liveness check - verifica apenas health checks marcados com tag "live"
            // Usado para verificar se o processo está vivo (não travado)
            app.MapHealthChecks("/alive", new HealthCheckOptions
            {
                Predicate = r => r.Tags.Contains("live")
            });
        }

        return app;
    }
}
