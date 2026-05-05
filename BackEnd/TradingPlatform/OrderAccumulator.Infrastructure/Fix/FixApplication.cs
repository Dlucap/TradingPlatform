using OrderAccumulator.Application.Dtos;
using QuickFix;
using QuickFix.FIX44;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderAccumulator.Infrastructure.Fix;

public class FixApplication : MessageCracker, IApplication
{
    private readonly FixSessionProvider _sessionProvider;
    private readonly IFixMessageStore _messageStore;

    public FixApplication(
        FixSessionProvider sessionProvider,
        IFixMessageStore messageStore)
    {
        _sessionProvider = sessionProvider;
        _messageStore = messageStore;
    }

    public void OnLogon(SessionID sessionID)
    {
        _sessionProvider.Set(sessionID);
    }

    public void FromApp(QuickFix.Message message, SessionID sessionID)
    {
        Crack(message, sessionID);
    }

    public void OnMessage(ExecutionReport report, SessionID sessionID)
    {
        var clOrdId = report.ClOrdID.Value;

        _messageStore.Resolve(clOrdId, new ExecutionReportDto
        {
            ExecType = report.ExecType.Value.ToString(),
            Text = report.IsSetText() ? report.Text.Value : ""
        });
    }

    public void OnCreate(SessionID sessionID) { }
    public void OnLogout(SessionID sessionID) { }
    public void ToAdmin(QuickFix.Message message, SessionID sessionID) { }
    public void FromAdmin(QuickFix.Message message, SessionID sessionID) { }
    public void ToApp(QuickFix.Message message, SessionID sessionID) { }
}
