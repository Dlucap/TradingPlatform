using OrderAccumulator.Application.Dtos;
using QuickFix.FIX44;

namespace OrderAccumulator.Application.Interfaces;

public interface IFixClient
{
    Task<ExecutionReportDto> SendAsync(NewOrderSingle fixMessage);
}