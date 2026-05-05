using QuickFix;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderAccumulator.Infrastructure.Fix;

public class FixSessionProvider
{
    private SessionID? _sessionId;
    private readonly SemaphoreSlim _semaphore = new(0);

    public void Set(SessionID sessionId)
    {
        _sessionId = sessionId;
        _semaphore.Release();
    }

    public SessionID Get()
    {
        if (_sessionId == null)
            throw new InvalidOperationException("Sessão FIX não conectada. Aguarde o Logon.");

        return _sessionId;
    }

    public async Task<SessionID> WaitForSessionAsync(TimeSpan timeout)
    {
        if (_sessionId != null)
            return _sessionId;

        var cts = new CancellationTokenSource(timeout);

        try
        {
            await _semaphore.WaitAsync(cts.Token);

            if (_sessionId == null)
                throw new InvalidOperationException("Sessão FIX não foi estabelecida");

            return _sessionId;
        }
        catch (OperationCanceledException)
        {
            throw new TimeoutException($"Timeout aguardando conexão FIX ({timeout.TotalSeconds}s)");
        }
    }

    public bool IsConnected() => _sessionId != null;
}