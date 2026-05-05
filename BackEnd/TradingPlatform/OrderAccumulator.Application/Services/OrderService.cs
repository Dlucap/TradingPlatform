using OrderAccumulator.Application.Dtos;
using OrderAccumulator.Application.Interfaces;
using OrderAccumulator.Application.Mapper;
using OrderAccumulator.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderAccumulator.Application.Services;


public class OrderService(IFixClient fixClient) : IOrderService
{
    public async Task<OrderResponse> SendOrderAsync(OrderRequest request)
    {
        var order = new Order(request.Symbol,
                              request.Side,
                              request.Quantity,
                              request.Price);

        var fixMessage = FixMapper.ToNewOrderSingle(order);

        var executionReport = await fixClient.SendAsync(fixMessage);

        // Interpretar ExecType FIX    
        var isAccepted = executionReport.IsAccepted;       
        return new OrderResponse
        {
            Status = isAccepted ? "New" : "rejected",
            Message = isAccepted 
                ? "Ordem aceita e incluída no cálculo de exposição" 
                : !isAccepted
                    ? $"Ordem rejeitada: {executionReport.Text}" 
                    : executionReport.Text,            
        };
    }
}