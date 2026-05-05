using OrderAccumulator.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderAccumulator.Domain.Entities;
public class Order : EntityBase
{
    public string Symbol { get; }
    public string Side { get; }
    public int Quantity { get; }
    public decimal Price { get; }

    public Order(string symbol, string side, int quantity, decimal price)
    {
        Symbol = symbol;
        Side = side;
        Quantity = quantity;
        Price = price;

        Validate();
    }

    private void Validate()
    {
        if (!new[] { "PETR4", "VALE3", "VIIA4" }.Contains(Symbol))
            throw new OrderRejectedException("Símbolo inválido");

        if (Quantity <= 0 || Quantity >= 100000)
            throw new OrderRejectedException("Quantidade inválida");

        if (Price <= 0 || Price >= 1000)
            throw new OrderRejectedException("Preço inválido");

        if (Price % 0.01m != 0)
            throw new OrderRejectedException("Preço deve ser múltiplo de 0.01");
    }
}