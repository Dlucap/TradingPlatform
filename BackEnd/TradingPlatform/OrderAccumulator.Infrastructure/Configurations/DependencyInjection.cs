using Microsoft.Extensions.DependencyInjection;
using OrderAccumulator.Application.Interfaces;
using OrderAccumulator.Application.Services;
using OrderAccumulator.Infrastructure.Fix;
using OrderAccumulator.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderAccumulator.Infrastructure.Configurations;
public static class DependencyInjection
{
    public static IServiceCollection AddServicesApi(this IServiceCollection services)
    {
        services.AddSingleton<FixSessionProvider>();
        services.AddSingleton<IFixMessageStore, FixMessageStore>();
        services.AddSingleton<FixApplication>();
        services.AddSingleton<IOrderService, OrderService>();
        services.AddSingleton<IFixClient, FixClient>();
        services.AddHostedService<OrderAccumulator.Infrastructure.Services.FixInitiatorService>();

        return services;
    }
}
