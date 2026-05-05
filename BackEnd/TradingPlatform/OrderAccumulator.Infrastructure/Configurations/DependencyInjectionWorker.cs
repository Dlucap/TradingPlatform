using Microsoft.Extensions.DependencyInjection;
using OrderAccumulator.Application.Interfaces;
using OrderAccumulator.Application.Services;
using OrderAccumulator.Infrastructure.Fix;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderAccumulator.Infrastructure.Configurations;

public static class DependencyInjectionWorker
{
  public static IServiceCollection AddServicesWorker(this IServiceCollection services)
  {
    services.AddSingleton<IExposureCalculator, ExposureCalculator>();
    services.AddSingleton<FixAcceptorApplication>();

    return services;
  }
}