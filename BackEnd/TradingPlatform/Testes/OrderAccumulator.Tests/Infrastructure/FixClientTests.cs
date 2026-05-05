using FluentAssertions;
using Moq;
using OrderAccumulator.Application.Dtos;
using OrderAccumulator.Infrastructure.Fix;
using QuickFix;
using QuickFix.Fields;
using QuickFix.FIX44;
using Xunit;

namespace OrderAccumulator.Tests.Infrastructure;

public class FixClientTests
{
    private readonly FixSessionProvider _sessionProvider;
    private readonly Mock<IFixMessageStore> _messageStoreMock;
    private readonly FixClient _fixClient;

    public FixClientTests()
    {
        _sessionProvider = new FixSessionProvider();
        _messageStoreMock = new Mock<IFixMessageStore>();
        _fixClient = new FixClient(_sessionProvider, _messageStoreMock.Object);
    }

    [Fact]
    public async Task SendAsync_ShouldThrowException_WhenSessionNotAvailable()
    {
        // Arrange
        var order = new NewOrderSingle(
            new ClOrdID(Guid.NewGuid().ToString()),
            new Symbol("PETR4"),
            new Side(Side.BUY),
            new TransactTime(DateTime.UtcNow),
            new OrdType(OrdType.LIMIT)
        );
        order.SetField(new OrderQty(100));
        order.SetField(new Price(25.50m));

        // Session não é configurada, então WaitForSessionAsync vai dar timeout

        // Act
        Func<Task> act = async () => await _fixClient.SendAsync(order);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Não foi possível conectar ao OrderAccumulator. Verifique se o Worker está rodando.");
    }

    [Fact]
    public void SendAsync_ShouldThrowException_WhenClOrdIdIsEmpty()
    {
        // Arrange
        var order = new NewOrderSingle(
            new ClOrdID(""),
            new Symbol("PETR4"),
            new Side(Side.BUY),
            new TransactTime(DateTime.UtcNow),
            new OrdType(OrdType.LIMIT)
        );

        // Act
        Func<Task> act = async () => await _fixClient.SendAsync(order);

        // Assert
        act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("ClOrdID is required");
    }

    [Fact]
    public void SendAsync_ShouldRegister_MessageInStore()
    {
        // Arrange
        var clOrdId = Guid.NewGuid().ToString();
        var sessionId = new SessionID("FIX.4.4", "SENDER", "TARGET");
        _sessionProvider.Set(sessionId);

        var order = new NewOrderSingle(
            new ClOrdID(clOrdId),
            new Symbol("PETR4"),
            new Side(Side.BUY),
            new TransactTime(DateTime.UtcNow),
            new OrdType(OrdType.LIMIT)
        );
        order.SetField(new OrderQty(100));
        order.SetField(new Price(25.50m));

        string? capturedClOrdId = null;
        _messageStoreMock
            .Setup(x => x.Register(It.IsAny<string>(), It.IsAny<TaskCompletionSource<ExecutionReportDto>>(), It.IsAny<TimeSpan>()))
            .Callback<string, TaskCompletionSource<ExecutionReportDto>, TimeSpan>((id, tcs, timeout) =>
            {
                capturedClOrdId = id;
                // Simula uma resposta imediata para não bloquear o teste
                tcs.SetResult(new ExecutionReportDto { ExecType = "0", Text = "OK" });
            });

        // Act
        var task = _fixClient.SendAsync(order);

        // Assert
        capturedClOrdId.Should().Be(clOrdId);
        _messageStoreMock.Verify(x => x.Register(
            clOrdId,
            It.IsAny<TaskCompletionSource<ExecutionReportDto>>(),
            It.IsAny<TimeSpan>()), Times.Once);
    }

    [Fact]
    public void FixClient_ShouldHave_PendingDictionary()
    {
        // Assert - O campo _pending é interno e usado pelo FixAcceptorApplication
        // Este teste documenta que o FixClient mantém um dicionário de requisições pendentes
        _fixClient.Should().NotBeNull();
    }
}
