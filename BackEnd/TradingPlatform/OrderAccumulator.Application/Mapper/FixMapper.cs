using OrderAccumulator.Domain.Entities;
using QuickFix.Fields;
using QuickFix.FIX44;

namespace OrderAccumulator.Application.Mapper;

public class FixMapper
{
    public static NewOrderSingle ToNewOrderSingle(Order order)
    {
        var message = new NewOrderSingle(
           new ClOrdID(order.Id.ToString()),
           new Symbol(order.Symbol),
           new Side(order.Side == "Buy" ? Side.BUY : Side.SELL),
           new TransactTime(DateTime.UtcNow),
           new OrdType(OrdType.LIMIT)
       );

        message.Set(new OrderQty(order.Quantity));
        message.Set(new Price(order.Price));
        return message;
    }
}
