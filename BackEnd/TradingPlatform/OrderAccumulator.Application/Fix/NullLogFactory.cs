using QuickFix;
using QuickFix.Logger;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderAccumulator.Application.Fix;

public class NullLogFactory : ILogFactory
{
    public ILog Create(SessionID sessionID)
    {
        return new NullLog();
    }

    public ILog CreateNonSessionLog()
    {
        return new NullLog();
    }
}

public class NullLog : ILog
{
    public void Clear() { }
    public void OnIncoming(string msg) { }
    public void OnOutgoing(string msg) { }
    public void OnEvent(string msg) { }
    public void Dispose() { }
}
