using OrderAccumulator.Application.Dtos;
using OrderAccumulator.Application.Interfaces;
using QuickFix;
using QuickFix.FIX44;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace OrderAccumulator.Infrastructure.Fix;

public class FixClient(FixSessionProvider sessionProvider, IFixMessageStore store) : IFixClient
{
    internal readonly ConcurrentDictionary<string, TaskCompletionSource<ExecutionReportDto>> _pending = new();
    private readonly TimeSpan _timeout = TimeSpan.FromSeconds(5);
    private readonly TimeSpan _connectionTimeout = TimeSpan.FromSeconds(10);

    public async Task<ExecutionReportDto> SendAsync(NewOrderSingle fixMessage)
    {
        var clOrdId = fixMessage.ClOrdID.ToString();

        if (string.IsNullOrEmpty(clOrdId))
            throw new ArgumentException("ClOrdID is required");

        SessionID sessionId;
        try
        {
            sessionId = await sessionProvider.WaitForSessionAsync(_connectionTimeout);
        }
        catch (TimeoutException)
        {
            throw new InvalidOperationException(
                "Não foi possível conectar ao OrderAccumulator. Verifique se o Worker está rodando.");
        }

        var tcs = new TaskCompletionSource<ExecutionReportDto>();

        store.Register(clOrdId, tcs, _timeout);

        var sent = Session.SendToTarget(fixMessage, sessionId);

        if (!sent)
        {
            store.Fail(clOrdId, "SendToTarget failed");
            throw new InvalidOperationException("Falha ao enviar mensagem FIX. Sessão pode estar desconectada.");
        }

        var completed = await Task.WhenAny(tcs.Task, Task.Delay(_timeout));

        if (completed != tcs.Task)
        {
            store.Remove(clOrdId);
            throw new TimeoutException($"Timeout aguardando resposta para ordem {clOrdId}. OrderAccumulator pode estar indisponível.");
        }

        return await tcs.Task;
    }
}
