using Microsoft.Extensions.Logging;
using OrderAccumulator.Application.Interfaces;
using QuickFix;
using QuickFix.Fields;
using QuickFix.FIX44;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderAccumulator.Infrastructure.Fix;

public class FixAcceptorApplication : MessageCracker, IApplication
{
    private readonly ILogger<FixAcceptorApplication> _logger;
    private readonly IExposureCalculator _exposureCalculator;

    public FixAcceptorApplication(
        ILogger<FixAcceptorApplication> logger,
        IExposureCalculator exposureCalculator)
    {
        _logger = logger;
        _exposureCalculator = exposureCalculator;
    }

    public void OnCreate(SessionID sessionID)
    {
        _logger.LogInformation("Session created: {SessionID}", sessionID);
    }

    public void OnLogon(SessionID sessionID)
    {
        _logger.LogInformation("Logon: {SessionID}", sessionID);
    }

    public void OnLogout(SessionID sessionID)
    {
        _logger.LogInformation("Logout: {SessionID}", sessionID);
    }

    public void FromApp(QuickFix.Message message, SessionID sessionID)
    {
        try
        {
            Crack(message, sessionID);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message");
        }
    }

    public void OnMessage(NewOrderSingle order, SessionID sessionID)
    {
        try
        {
            var symbol = order.Symbol.Value;
            var side = order.Side.Value == Side.BUY ? "Buy" : "Sell";
            var quantity = (int)order.OrderQty.Value;
            var price = order.Price.Value;
            var clOrdId = order.ClOrdID.Value;

            _logger.LogInformation(
                "Received Order | ClOrdID={ClOrdID} | Symbol={Symbol} | Side={Side} | Qty={Qty} | Price={Price}",
                clOrdId, symbol, side, quantity, price);

            var canAccept = _exposureCalculator.CanAcceptOrder(symbol, side, quantity, price);

            var executionReport = new ExecutionReport(
                new OrderID(clOrdId),
                new ExecID(Guid.NewGuid().ToString()),
                new ExecType(canAccept ? ExecType.NEW : ExecType.REJECTED),
                new OrdStatus(canAccept ? OrdStatus.NEW : OrdStatus.REJECTED),
                order.Symbol,
                order.Side,
                new LeavesQty(canAccept ? quantity : 0),
                new CumQty(0),
                new AvgPx(0)
            );

            executionReport.Set(order.ClOrdID);
            executionReport.Set(order.OrderQty);
            executionReport.Set(order.Price);
            executionReport.Set(order.OrdType);

            if (canAccept)
            {
                _exposureCalculator.ProcessOrder(symbol, side, quantity, price);
                var exposure = _exposureCalculator.GetExposure(symbol);

                _logger.LogInformation(
                    "Order ACCEPTED | ClOrdID={ClOrdID} | Exposure {Symbol}={Exposure:N2}",
                    clOrdId, symbol, exposure);
            }
            else
            {
                executionReport.Set(new Text("Order rejected: exposure limit exceeded"));
                var exposure = _exposureCalculator.GetExposure(symbol);

                _logger.LogWarning(
                    "Order REJECTED | ClOrdID={ClOrdID} | Exposure {Symbol}={Exposure:N2} | Would exceed limit",
                    clOrdId, symbol, exposure);
            }

            Session.SendToTarget(executionReport, sessionID);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing NewOrderSingle");
        }
    }

    public void ToAdmin(QuickFix.Message message, SessionID sessionID) { }
    public void FromAdmin(QuickFix.Message message, SessionID sessionID) { }
    public void ToApp(QuickFix.Message message, SessionID sessionID) { }
}
