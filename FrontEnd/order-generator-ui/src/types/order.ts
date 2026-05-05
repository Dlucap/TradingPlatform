
export type OrderSide = "Buy" | "Sell" | "BUY" | "SELL";

export type SymbolType = "PETR4" | "VALE3" | "VIIA4";

export interface OrderRequest {
  symbol: SymbolType | string;
  side: OrderSide;
  quantity: number;  // IMPORTANTE: Deve ser inteiro positivo (não decimal)
  price: number;     // Decimal com até 2 casas decimais (ex: 28.50)
}

export interface OrderResponse {
  status: string;      // "accepted", "rejected", "error"
  message: string;     // Mensagem descritiva 
}