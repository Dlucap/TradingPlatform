using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OrderAccumulator.Infrastructure;
using OrderAccumulator.Infrastructure.Configurations;
using OrderAccumulator.Infrastructure.Fix;
using QuickFix;
using QuickFix.Transport;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<Worker>();
builder.Services.AddTelemetry();
builder.Services.AddServicesWorker();

var host = builder.Build();
host.Run();

class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly FixAcceptorApplication _fixApplication;
    private IAcceptor? _acceptor;

    public Worker(ILogger<Worker> logger, FixAcceptorApplication fixApplication)
    {
        _logger = logger;
        _fixApplication = fixApplication;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var settings = new SessionSettings("acceptor.cfg");
            var storeFactory = new SimpleStoreFactory();
            var logFactory = new NullLogFactory();

            _acceptor = new ThreadedSocketAcceptor(_fixApplication, storeFactory, settings, logFactory);

            _acceptor.Start();           
            _logger.LogInformation("OrderAccumulator FIX Acceptor started on port 5001");

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in FIX Acceptor");
            throw;
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("OrderAccumulator FIX Acceptor stopping");
        _acceptor?.Stop();
        return base.StopAsync(cancellationToken);
    }
}
