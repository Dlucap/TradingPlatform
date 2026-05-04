
export type OrderSide = "BUY" | "SELL";

export type SymbolType = "PETR4" | "VALE3" | "VIIA4";

export interface OrderRequest {
  symbol: SymbolType;
  side: OrderSide;
  quantity: number;
  price: number;
}

export interface OrderResponse {
  success: boolean;
  message: string;
  data?: unknown;
}