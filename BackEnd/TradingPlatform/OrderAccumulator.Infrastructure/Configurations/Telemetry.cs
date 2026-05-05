using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderAccumulator.Infrastructure.Configurations;
public static class Telemetry
{
    private const string HealthEndpointPath = "/health";
    private const string AlivenessEndpointPath = "/alive";

    public static IServiceCollection AddTelemetry(this IServiceCollection services)
    {
        services.AddOpenTelemetry()
       .WithTracing(tracing =>
       {
           tracing
               .AddAspNetCoreInstrumentation(tracing =>
                        // Exclude health check requests from tracing
                        tracing.Filter = context =>
                            !context.Request.Path.StartsWithSegments(HealthEndpointPath)
                            && !context.Request.Path.StartsWithSegments(AlivenessEndpointPath))
               .AddHttpClientInstrumentation()
               .AddSource("Orders")
               .AddConsoleExporter();
       })
       .WithMetrics(metrics =>
       {
           metrics
               .AddAspNetCoreInstrumentation()
               .AddRuntimeInstrumentation()
               .AddConsoleExporter();
       });
        return services;
    }
}