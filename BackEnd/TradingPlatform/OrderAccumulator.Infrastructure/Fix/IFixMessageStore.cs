using OrderAccumulator.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderAccumulator.Infrastructure.Fix;
public interface IFixMessageStore
{
    void Register(string clOrdId, TaskCompletionSource<ExecutionReportDto> tcs, TimeSpan timeout = default);
    void Resolve(string clOrdId, ExecutionReportDto response);
    void Fail(string clOrdId, string reason);
    void Remove(string clOrdId);
}