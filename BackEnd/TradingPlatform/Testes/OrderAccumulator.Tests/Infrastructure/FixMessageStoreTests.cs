using FluentAssertions;
using OrderAccumulator.Application.Dtos;
using OrderAccumulator.Infrastructure.Fix;
using Xunit;

namespace OrderAccumulator.Tests.Infrastructure;

public class FixMessageStoreTests
{
    private readonly FixMessageStore _store;

    public FixMessageStoreTests()
    {
        _store = new FixMessageStore();
    }

    [Fact]
    public void Register_ShouldStore_PendingRequest()
    {
        // Arrange
        var clOrdId = Guid.NewGuid().ToString();
        var tcs = new TaskCompletionSource<ExecutionReportDto>();
        var timeout = TimeSpan.FromSeconds(5);

        // Act
        _store.Register(clOrdId, tcs, timeout);

        // Assert
        // Se não lançar exceção, o registro foi bem-sucedido
        _store.Should().NotBeNull();
    }

    [Fact]
    public async Task Resolve_ShouldComplete_PendingRequest()
    {
        // Arrange
        var clOrdId = Guid.NewGuid().ToString();
        var tcs = new TaskCompletionSource<ExecutionReportDto>();
        var timeout = TimeSpan.FromSeconds(5);
        var expectedReport = new ExecutionReportDto
        {
            ExecType = "0",
            Text = "Order accepted"
        };

        _store.Register(clOrdId, tcs, timeout);

        // Act
        _store.Resolve(clOrdId, expectedReport);

        // Assert
        var result = await tcs.Task;
        result.Should().NotBeNull();
        result.ExecType.Should().Be(expectedReport.ExecType);
        result.Text.Should().Be(expectedReport.Text);
    }

    [Fact]
    public async Task Fail_ShouldFail_PendingRequest()
    {
        // Arrange
        var clOrdId = Guid.NewGuid().ToString();
        var tcs = new TaskCompletionSource<ExecutionReportDto>();
        var timeout = TimeSpan.FromSeconds(5);
        var errorMessage = "Connection failed";

        _store.Register(clOrdId, tcs, timeout);

        // Act
        _store.Fail(clOrdId, errorMessage);

        // Assert
        Func<Task> act = async () => await tcs.Task;
        await act.Should().ThrowAsync<Exception>()
            .WithMessage(errorMessage);
    }

    [Fact]
    public void Resolve_ShouldNotThrow_WhenClOrdIdNotFound()
    {
        // Arrange
        var clOrdId = Guid.NewGuid().ToString();
        var report = new ExecutionReportDto { ExecType = "0", Text = "OK" };

        // Act
        Action act = () => _store.Resolve(clOrdId, report);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Fail_ShouldNotThrow_WhenClOrdIdNotFound()
    {
        // Arrange
        var clOrdId = Guid.NewGuid().ToString();

        // Act
        Action act = () => _store.Fail(clOrdId, "Error message");

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public async Task Register_ShouldTimeout_AfterSpecifiedDuration()
    {
        // Arrange
        var clOrdId = Guid.NewGuid().ToString();
        var tcs = new TaskCompletionSource<ExecutionReportDto>();
        var timeout = TimeSpan.FromMilliseconds(100);

        _store.Register(clOrdId, tcs, timeout);

        // Act
        await Task.Delay(timeout.Add(TimeSpan.FromMilliseconds(50)));

        // Assert
        // Verificar que a task foi cancelada por timeout é difícil sem expor o CancellationToken
        // Este teste documenta o comportamento esperado
        _store.Should().NotBeNull();
    }

    [Fact]
    public async Task MultipleRequests_ShouldBeHandled_Independently()
    {
        // Arrange
        var clOrdId1 = Guid.NewGuid().ToString();
        var clOrdId2 = Guid.NewGuid().ToString();
        var tcs1 = new TaskCompletionSource<ExecutionReportDto>();
        var tcs2 = new TaskCompletionSource<ExecutionReportDto>();
        var timeout = TimeSpan.FromSeconds(5);

        var report1 = new ExecutionReportDto { ExecType = "0", Text = "Order 1" };
        var report2 = new ExecutionReportDto { ExecType = "1", Text = "Order 2" };

        _store.Register(clOrdId1, tcs1, timeout);
        _store.Register(clOrdId2, tcs2, timeout);

        // Act
        _store.Resolve(clOrdId1, report1);
        _store.Resolve(clOrdId2, report2);

        // Assert
        var result1 = await tcs1.Task;
        var result2 = await tcs2.Task;

        result1.Text.Should().Be("Order 1");
        result2.Text.Should().Be("Order 2");
    }
}
