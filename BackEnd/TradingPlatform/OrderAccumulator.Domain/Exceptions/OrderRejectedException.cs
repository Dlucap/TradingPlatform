using System;
using System.Collections.Generic;
using System.Text;

namespace OrderAccumulator.Domain.Exceptions;

public class OrderRejectedException : Exception
{
    public string Reason { get; }

    public OrderRejectedException(string reason) : base(reason)
    {
        Reason = reason;
    }

    public OrderRejectedException(string reason, Exception innerException)
        : base(reason, innerException)
    {
        Reason = reason;
    }
}
