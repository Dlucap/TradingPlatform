using FluentAssertions;
using Moq;
using OrderAccumulator.Application.Dtos;
using OrderAccumulator.Infrastructure.Fix;
using QuickFix;
using QuickFix.Fields;
using QuickFix.FIX44;
using Xunit;

namespace OrderAccumulator.Tests.Infrastructure;

public class FixApplicationTests
{
    private readonly FixSessionProvider _sessionProvider;
    private readonly Mock<IFixMessageStore> _messageStoreMock;
    private readonly FixApplication _fixApplication;

    public FixApplicationTests()
    {
        _sessionProvider = new FixSessionProvider();
        _messageStoreMock = new Mock<IFixMessageStore>();
        _fixApplication = new FixApplication(_sessionProvider, _messageStoreMock.Object);
    }

    [Fact]
    public void OnLogon_ShouldSet_SessionId()
    {
        // Arrange
        var sessionId = new SessionID("FIX.4.4", "SENDER", "TARGET");

        // Act
        _fixApplication.OnLogon(sessionId);

        // Assert
        var result = _sessionProvider.Get();
        result.Should().Be(sessionId);
    }

    [Fact]
    public void OnMessage_ShouldResolve_PendingOrder()
    {
        // Arrange
        var sessionId = new SessionID("FIX.4.4", "SENDER", "TARGET");
        var clOrdId = Guid.NewGuid().ToString();
        var execType = ExecType.NEW;
        var text = "Order accepted";

        var executionReport = new ExecutionReport(
            new OrderID("123"),
            new ExecID("456"),
            new ExecType(execType),
            new OrdStatus(OrdStatus.NEW),
            new Symbol("PETR4"),
            new Side(Side.BUY),
            new LeavesQty(0),
            new CumQty(0),
            new AvgPx(0)
        );
        executionReport.SetField(new ClOrdID(clOrdId));
        executionReport.SetField(new Text(text));

        ExecutionReportDto? capturedDto = null;
        _messageStoreMock
            .Setup(x => x.Resolve(It.IsAny<string>(), It.IsAny<ExecutionReportDto>()))
            .Callback<string, ExecutionReportDto>((id, dto) => capturedDto = dto);

        // Act
        _fixApplication.OnMessage(executionReport, sessionId);

        // Assert
        _messageStoreMock.Verify(x => x.Resolve(clOrdId, It.IsAny<ExecutionReportDto>()), Times.Once);
        capturedDto.Should().NotBeNull();
        capturedDto!.ExecType.Should().Be(execType.ToString());
        capturedDto.Text.Should().Be(text);
    }

    [Fact]
    public void OnMessage_ShouldHandle_ExecutionReportWithoutText()
    {
        // Arrange
        var sessionId = new SessionID("FIX.4.4", "SENDER", "TARGET");
        var clOrdId = Guid.NewGuid().ToString();

        var executionReport = new ExecutionReport(
            new OrderID("123"),
            new ExecID("456"),
            new ExecType(ExecType.NEW),
            new OrdStatus(OrdStatus.NEW),
            new Symbol("PETR4"),
            new Side(Side.BUY),
            new LeavesQty(0),
            new CumQty(0),
            new AvgPx(0)
        );
        executionReport.SetField(new ClOrdID(clOrdId));

        ExecutionReportDto? capturedDto = null;
        _messageStoreMock
            .Setup(x => x.Resolve(It.IsAny<string>(), It.IsAny<ExecutionReportDto>()))
            .Callback<string, ExecutionReportDto>((id, dto) => capturedDto = dto);

        // Act
        _fixApplication.OnMessage(executionReport, sessionId);

        // Assert
        capturedDto.Should().NotBeNull();
        capturedDto!.Text.Should().Be("");
    }

    [Fact]
    public void OnCreate_ShouldNotThrow()
    {
        // Arrange
        var sessionId = new SessionID("FIX.4.4", "SENDER", "TARGET");

        // Act
        Action act = () => _fixApplication.OnCreate(sessionId);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void OnLogout_ShouldNotThrow()
    {
        // Arrange
        var sessionId = new SessionID("FIX.4.4", "SENDER", "TARGET");

        // Act
        Action act = () => _fixApplication.OnLogout(sessionId);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void ToAdmin_ShouldNotThrow()
    {
        // Arrange
        var sessionId = new SessionID("FIX.4.4", "SENDER", "TARGET");
        var message = new QuickFix.FIX44.Logon();

        // Act
        Action act = () => _fixApplication.ToAdmin(message, sessionId);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void FromAdmin_ShouldNotThrow()
    {
        // Arrange
        var sessionId = new SessionID("FIX.4.4", "SENDER", "TARGET");
        var message = new QuickFix.FIX44.Logon();

        // Act
        Action act = () => _fixApplication.FromAdmin(message, sessionId);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void ToApp_ShouldNotThrow()
    {
        // Arrange
        var sessionId = new SessionID("FIX.4.4", "SENDER", "TARGET");
        var message = new NewOrderSingle();

        // Act
        Action act = () => _fixApplication.ToApp(message, sessionId);

        // Assert
        act.Should().NotThrow();
    }
}
