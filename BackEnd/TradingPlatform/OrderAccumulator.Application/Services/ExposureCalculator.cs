using OrderAccumulator.Application.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace OrderAccumulator.Application.Services;

public class ExposureCalculator : IExposureCalculator
{
    private readonly decimal _exposureLimit = 100_000_000m;
    private readonly ConcurrentDictionary<string, decimal> _exposures = new();

    public bool CanAcceptOrder(string symbol, string side, int quantity, decimal price)
    {
        var orderValue = quantity * price;
        var currentExposure = _exposures.GetOrAdd(symbol, 0);

        var newExposure = side == "Buy"
            ? currentExposure + orderValue
            : currentExposure - orderValue;

        return Math.Abs(newExposure) <= _exposureLimit;
    }

    public void ProcessOrder(string symbol, string side, int quantity, decimal price)
    {
        var orderValue = quantity * price;

        _exposures.AddOrUpdate(
            symbol,
            side == "Buy" ? orderValue : -orderValue,
            (key, current) => side == "Buy" ? current + orderValue : current - orderValue
        );
    }

    public decimal GetExposure(string symbol)
    {
        return _exposures.GetOrAdd(symbol, 0);
    }
}