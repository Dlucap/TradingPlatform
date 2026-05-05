using System;
using System.Collections.Generic;
using System.Text;

namespace OrderAccumulator.Application.Interfaces;
public interface IExposureCalculator
{
    bool CanAcceptOrder(string symbol, string side, int quantity, decimal price);
    void ProcessOrder(string symbol, string side, int quantity, decimal price);
    decimal GetExposure(string symbol);
}