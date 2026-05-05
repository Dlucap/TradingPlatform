using OrderAccumulator.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderAccumulator.Application.Interfaces;

public interface IOrderService
{
    Task<OrderResponse> SendOrderAsync(OrderRequest request);
}
