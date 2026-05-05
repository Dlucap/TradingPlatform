using FluentAssertions;
using OrderAccumulator.Domain.Entities;
using OrderAccumulator.Domain.Exceptions;
using Xunit;

namespace OrderAccumulator.Tests.Domain;

public class OrderTests
{
    [Theory]
    [InlineData("PETR4", "Buy", 100, 150.50)]
    [InlineData("VALE3", "Sell", 500, 75.25)]
    [InlineData("VIIA4", "Buy", 1000, 25.00)]
    public void Order_ShouldBeCreated_WithValidParameters(string symbol, string side, int quantity, decimal price)
    {
        // Act
        var order = new Order(symbol, side, quantity, price);

        // Assert
        order.Should().NotBeNull();
        order.Id.Should().NotBeEmpty();
        order.Symbol.Should().Be(symbol);
        order.Side.Should().Be(side);
        order.Quantity.Should().Be(quantity);
        order.Price.Should().Be(price);
    }

    [Theory]
    [InlineData("AAPL", "Buy", 100, 150.50)]
    [InlineData("INVALID", "Buy", 100, 150.50)]
    [InlineData("", "Buy", 100, 150.50)]
    public void Order_ShouldThrowException_WhenSymbolIsInvalid(string symbol, string side, int quantity, decimal price)
    {
        // Act
        Action act = () => new Order(symbol, side, quantity, price);

        // Assert
        act.Should().Throw<OrderRejectedException>()
            .WithMessage("Símbolo inválido");
    }

    [Theory]
    [InlineData("PETR4", "Buy", 0, 150.50)]
    [InlineData("PETR4", "Buy", -100, 150.50)]
    [InlineData("PETR4", "Buy", 100000, 150.50)]
    [InlineData("PETR4", "Buy", 150000, 150.50)]
    public void Order_ShouldThrowException_WhenQuantityIsInvalid(string symbol, string side, int quantity, decimal price)
    {
        // Act
        Action act = () => new Order(symbol, side, quantity, price);

        // Assert
        act.Should().Throw<OrderRejectedException>()
            .WithMessage("Quantidade inválida");
    }

    [Theory]
    [InlineData("PETR4", "Buy", 100, 0)]
    [InlineData("PETR4", "Buy", 100, -150.50)]
    [InlineData("PETR4", "Buy", 100, 1000)]
    [InlineData("PETR4", "Buy", 100, 1500)]
    public void Order_ShouldThrowException_WhenPriceIsInvalid(string symbol, string side, int quantity, decimal price)
    {
        // Act
        Action act = () => new Order(symbol, side, quantity, price);

        // Assert
        act.Should().Throw<OrderRejectedException>()
            .WithMessage("Preço inválido");
    }

    [Theory]
    [InlineData("PETR4", "Buy", 100, 150.501)]
    [InlineData("PETR4", "Buy", 100, 150.555)]
    public void Order_ShouldThrowException_WhenPriceIsNotMultipleOf001(string symbol, string side, int quantity, decimal price)
    {
        // Act
        Action act = () => new Order(symbol, side, quantity, price);

        // Assert
        act.Should().Throw<OrderRejectedException>()
            .WithMessage("Preço deve ser múltiplo de 0.01");
    }

    [Fact]
    public void Order_ShouldGenerate_UniqueIds()
    {
        // Arrange & Act
        var order1 = new Order("PETR4", "Buy", 100, 150.50m);
        var order2 = new Order("PETR4", "Buy", 100, 150.50m);

        // Assert
        order1.Id.Should().NotBe(order2.Id);
    }

    [Theory]
    [InlineData("PETR4", "Buy", 1, 0.01)]
    [InlineData("VALE3", "Sell", 99999, 999.99)]
    public void Order_ShouldAccept_BoundaryValues(string symbol, string side, int quantity, decimal price)
    {
        // Act
        var order = new Order(symbol, side, quantity, price);

        // Assert
        order.Should().NotBeNull();
        order.Quantity.Should().Be(quantity);
        order.Price.Should().Be(price);
    }
}
