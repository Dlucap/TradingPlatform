using FluentAssertions;
using OrderAccumulator.Application.Mapper;
using OrderAccumulator.Domain.Entities;
using QuickFix.Fields;
using Xunit;

namespace OrderAccumulator.Tests.Application;

public class FixMapperTests
{
    [Theory]
    [InlineData("PETR4", "Buy", 100, 25.50)]
    [InlineData("VALE3", "Sell", 500, 75.25)]
    public void ToNewOrderSingle_ShouldMapOrder_Correctly(string symbol, string side, int quantity, decimal price)
    {
        // Arrange
        var order = new Order(symbol, side, quantity, price);

        // Act
        var fixMessage = FixMapper.ToNewOrderSingle(order);

        // Assert
        fixMessage.Should().NotBeNull();
        fixMessage.ClOrdID.Value.Should().Be(order.Id.ToString());
        fixMessage.Symbol.Value.Should().Be(symbol);
        fixMessage.OrderQty.Value.Should().Be(quantity);
        fixMessage.Price.Value.Should().Be(price);
        fixMessage.OrdType.Value.Should().Be(OrdType.LIMIT);
    }

    [Fact]
    public void ToNewOrderSingle_ShouldMap_BuySide()
    {
        // Arrange
        var order = new Order("PETR4", "Buy", 100, 25.50m);

        // Act
        var fixMessage = FixMapper.ToNewOrderSingle(order);

        // Assert
        fixMessage.Side.Value.Should().Be(Side.BUY);
    }

    [Fact]
    public void ToNewOrderSingle_ShouldMap_SellSide()
    {
        // Arrange
        var order = new Order("VALE3", "Sell", 500, 75.25m);

        // Act
        var fixMessage = FixMapper.ToNewOrderSingle(order);

        // Assert
        fixMessage.Side.Value.Should().Be(Side.SELL);
    }

    [Fact]
    public void ToNewOrderSingle_ShouldInclude_TransactTime()
    {
        // Arrange
        var order = new Order("PETR4", "Buy", 100, 25.50m);
        var beforeMapping = DateTime.UtcNow;

        // Act
        var fixMessage = FixMapper.ToNewOrderSingle(order);

        // Assert
        var afterMapping = DateTime.UtcNow;
        fixMessage.TransactTime.Value.Should().BeAfter(beforeMapping.AddSeconds(-1));
        fixMessage.TransactTime.Value.Should().BeBefore(afterMapping.AddSeconds(1));
    }

    [Fact]
    public void ToNewOrderSingle_ShouldSet_LimitOrderType()
    {
        // Arrange
        var order = new Order("PETR4", "Buy", 100, 25.50m);

        // Act
        var fixMessage = FixMapper.ToNewOrderSingle(order);

        // Assert
        fixMessage.OrdType.Value.Should().Be(OrdType.LIMIT);
    }
}
