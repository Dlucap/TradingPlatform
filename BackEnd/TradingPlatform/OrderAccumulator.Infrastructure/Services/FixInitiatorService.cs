using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OrderAccumulator.Infrastructure.Fix;
using QuickFix;
using QuickFix.Transport;

namespace OrderAccumulator.Infrastructure.Services;

public class FixInitiatorService : IHostedService
{
    private readonly ILogger<FixInitiatorService> _logger;
    private readonly FixApplication _fixApplication;
    private IInitiator? _initiator;

    public FixInitiatorService(ILogger<FixInitiatorService> logger, FixApplication fixApplication)
    {
        _logger = logger;
        _fixApplication = fixApplication;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            var settings = new SessionSettings("initiator.cfg");
            var storeFactory = new SimpleStoreFactory();
            var logFactory = new NullLogFactory();

            _initiator = new SocketInitiator(_fixApplication, storeFactory, settings, logFactory);

            _initiator.Start();
            _logger.LogInformation("FIX Initiator started - connecting to OrderAccumulator on port 5001");

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start FIX Initiator");
            throw;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("FIX Initiator stopping");
        _initiator?.Stop();
        return Task.CompletedTask;
    }
}
