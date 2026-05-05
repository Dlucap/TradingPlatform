using QuickFix;
using QuickFix.Store;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderAccumulator.Application.Fix;

public class SimpleStoreFactory : IMessageStoreFactory
{
    public IMessageStore Create(SessionID sessionID)
    {
        return new SimpleStore();
    }
}

public class SimpleStore : IMessageStore
{
    private ulong _nextSenderMsgSeqNum = 1;
    private ulong _nextTargetMsgSeqNum = 1;
    private readonly DateTime? _creationTime = DateTime.UtcNow;

    public ulong NextSenderMsgSeqNum
    {
        get => _nextSenderMsgSeqNum;
        set => _nextSenderMsgSeqNum = value;
    }

    public ulong NextTargetMsgSeqNum
    {
        get => _nextTargetMsgSeqNum;
        set => _nextTargetMsgSeqNum = value;
    }

    public DateTime? CreationTime => _creationTime;

    public void Get(ulong startSeqNum, ulong endSeqNum, List<string> messages) { }
    public bool Set(ulong seqNum, string msg) => true;
    public void IncrNextSenderMsgSeqNum() => _nextSenderMsgSeqNum++;
    public void IncrNextTargetMsgSeqNum() => _nextTargetMsgSeqNum++;
    public void Reset()
    {
        _nextSenderMsgSeqNum = 1;
        _nextTargetMsgSeqNum = 1;
    }
    public void Refresh() { }
    public void Dispose() { }
}
