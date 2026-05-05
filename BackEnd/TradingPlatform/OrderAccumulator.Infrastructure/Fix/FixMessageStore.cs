using OrderAccumulator.Application.Dtos;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace OrderAccumulator.Infrastructure.Fix;
public class FixMessageStore : IFixMessageStore
{
    private class Item
    {
        public TaskCompletionSource<ExecutionReportDto> Tcs { get; }
        public CancellationTokenSource Cts { get; }

        public Item(TaskCompletionSource<ExecutionReportDto> tcs, CancellationTokenSource cts)
        {
            Tcs = tcs;
            Cts = cts;
        }
    }

    private readonly ConcurrentDictionary<string, Item> _pending = new();

    public void Register(string clOrdId, TaskCompletionSource<ExecutionReportDto> tcs, TimeSpan timeout = default)
    {
        var cts = new CancellationTokenSource();

        _pending[clOrdId] = new Item(tcs, cts);

        if (timeout != default)
        {
            Task.Delay(timeout, cts.Token).ContinueWith(_ =>
            {
                if (_pending.TryRemove(clOrdId, out var item))
                {
                    item.Tcs.TrySetException(new TimeoutException("No ExecutionReport received"));
                }
            }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }
    }

    public void Resolve(string clOrdId, ExecutionReportDto response)
    {
        if (_pending.TryRemove(clOrdId, out var item))
        {
            item.Cts.Cancel();
            item.Tcs.TrySetResult(response);
        }
    }

    public void Fail(string clOrdId, string reason)
    {
        if (_pending.TryRemove(clOrdId, out var item))
        {
            item.Cts.Cancel();
            item.Tcs.TrySetException(new Exception(reason));
        }
    }

    public void Remove(string clOrdId)
    {
        if (_pending.TryRemove(clOrdId, out var item))
        {
            item.Cts.Cancel();
        }
    }
}