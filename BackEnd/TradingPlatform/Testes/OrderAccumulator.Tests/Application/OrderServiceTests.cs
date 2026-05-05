using FluentAssertions;
using Moq;
using OpenTelemetry.Trace;
using OrderAccumulator.Application.Dtos;
using OrderAccumulator.Application.Interfaces;
using OrderAccumulator.Application.Services;
using OrderAccumulator.Domain.Entities;
using QuickFix.FIX44;
using Xunit;

namespace OrderAccumulator.Tests.Application;

public class OrderServiceTests
{
    private readonly Mock<IFixClient> _fixClientMock;
    private readonly OrderService _orderService;

    public OrderServiceTests()
    {
        _fixClientMock = new Mock<IFixClient>();
        _orderService = new OrderService(_fixClientMock.Object);
    }

    [Fact]
    public async Task SendOrderAsync_ShouldReturnSuccess_WhenOrderIsValid()
    {
        // Arrange
        var request = new OrderRequest
        {
            Symbol = "PETR4",
            Side = "Buy",
            Quantity = 100,
            Price = 25.50m
        };
        
        var expectedReport = new ExecutionReportDto
        {
            ExecType = "accepted",
            Text = "Ordem aceita e incluída no cálculo de exposição"
        };

        _fixClientMock
            .Setup(x => x.SendAsync(It.IsAny<NewOrderSingle>()))
            .ReturnsAsync(expectedReport);

        // Act
        var result = await _orderService.SendOrderAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be("New");
        result.Message.Should().Be(expectedReport.Text);

        _fixClientMock.Verify(x => x.SendAsync(It.IsAny<NewOrderSingle>()), Times.Once);
    }

    [Fact]
    public async Task SendOrderAsync_ShouldCallFixClient_WithCorrectMessage()
    {
        // Arrange
        var request = new OrderRequest
        {
            Symbol = "VALE3",
            Side = "Sell",
            Quantity = 500,
            Price = 75.25m
        };

        NewOrderSingle? capturedMessage = null;
        _fixClientMock
            .Setup(x => x.SendAsync(It.IsAny<NewOrderSingle>()))
            .Callback<NewOrderSingle>(msg => capturedMessage = msg)
            .ReturnsAsync(new ExecutionReportDto { ExecType = "0", Text = "OK" });

        // Act
        await _orderService.SendOrderAsync(request);

        // Assert
        capturedMessage.Should().NotBeNull();
        capturedMessage!.Symbol.Value.Should().Be(request.Symbol);
        capturedMessage.Side.Value.Should().Be('2'); // Sell = '2' in FIX
        capturedMessage.OrderQty.Value.Should().Be(request.Quantity);
        capturedMessage.Price.Value.Should().Be(request.Price);
    }

    [Fact]
    public async Task SendOrderAsync_ShouldThrowException_WhenFixClientFails()
    {
        // Arrange
        var request = new OrderRequest
        {
            Symbol = "PETR4",
            Side = "Buy",
            Quantity = 100,
            Price = 25.50m
        };

        _fixClientMock
            .Setup(x => x.SendAsync(It.IsAny<NewOrderSingle>()))
            .ThrowsAsync(new InvalidOperationException("Connection failed"));

        // Act
        Func<Task> act = async () => await _orderService.SendOrderAsync(request);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Connection failed");
    }
}
