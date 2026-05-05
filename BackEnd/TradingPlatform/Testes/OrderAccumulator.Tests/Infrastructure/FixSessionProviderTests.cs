using FluentAssertions;
using OrderAccumulator.Infrastructure.Fix;
using QuickFix;
using Xunit;

namespace OrderAccumulator.Tests.Infrastructure;

public class FixSessionProviderTests
{
    [Fact]
    public void Set_ShouldStore_SessionId()
    {
        // Arrange
        var provider = new FixSessionProvider();
        var sessionId = new SessionID("FIX.4.4", "SENDER", "TARGET");

        // Act
        provider.Set(sessionId);

        // Assert
        var result = provider.Get();
        result.Should().Be(sessionId);
    }

    [Fact]
    public void Get_ShouldThrowException_WhenSessionNotSet()
    {
        // Arrange
        var provider = new FixSessionProvider();

        // Act
        Action act = () => provider.Get();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Sessão FIX não conectada. Aguarde o Logon.");
    }

    [Fact]
    public async Task WaitForSessionAsync_ShouldReturn_WhenSessionIsSet()
    {
        // Arrange
        var provider = new FixSessionProvider();
        var sessionId = new SessionID("FIX.4.4", "SENDER", "TARGET");
        var timeout = TimeSpan.FromSeconds(5);

        // Act
        var waitTask = provider.WaitForSessionAsync(timeout);
        provider.Set(sessionId);
        var result = await waitTask;

        // Assert
        result.Should().Be(sessionId);
    }

    [Fact]
    public async Task WaitForSessionAsync_ShouldReturnImmediately_WhenSessionAlreadySet()
    {
        // Arrange
        var provider = new FixSessionProvider();
        var sessionId = new SessionID("FIX.4.4", "SENDER", "TARGET");
        provider.Set(sessionId);
        var timeout = TimeSpan.FromSeconds(5);

        // Act
        var result = await provider.WaitForSessionAsync(timeout);

        // Assert
        result.Should().Be(sessionId);
    }

    [Fact]
    public async Task WaitForSessionAsync_ShouldTimeout_WhenSessionNotSet()
    {
        // Arrange
        var provider = new FixSessionProvider();
        var timeout = TimeSpan.FromMilliseconds(100);

        // Act
        Func<Task> act = async () => await provider.WaitForSessionAsync(timeout);

        // Assert
        await act.Should().ThrowAsync<TimeoutException>()
            .WithMessage("Timeout aguardando conexão FIX (*");
    }

    [Fact]
    public void IsConnected_ShouldReturnFalse_WhenSessionNotSet()
    {
        // Arrange
        var provider = new FixSessionProvider();

        // Act
        var result = provider.IsConnected();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsConnected_ShouldReturnTrue_WhenSessionIsSet()
    {
        // Arrange
        var provider = new FixSessionProvider();
        var sessionId = new SessionID("FIX.4.4", "SENDER", "TARGET");

        // Act
        provider.Set(sessionId);
        var result = provider.IsConnected();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task MultipleWaiters_ShouldAllBeReleased_WhenSessionIsSet()
    {
        // Arrange
        var provider = new FixSessionProvider();
        var sessionId = new SessionID("FIX.4.4", "SENDER", "TARGET");
        var timeout = TimeSpan.FromSeconds(5);

        // Pre-set the session before waiting
        provider.Set(sessionId);

        // Act - All waiters should return immediately since session is already set
        var waitTask1 = provider.WaitForSessionAsync(timeout);
        var waitTask2 = provider.WaitForSessionAsync(timeout);
        var waitTask3 = provider.WaitForSessionAsync(timeout);

        var results = await Task.WhenAll(waitTask1, waitTask2, waitTask3);

        // Assert
        results.Should().HaveCount(3);
        results.Should().AllSatisfy(r => r.Should().Be(sessionId));
    }
}
