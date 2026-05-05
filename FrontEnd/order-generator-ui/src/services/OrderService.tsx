import type { OrderRequest, OrderResponse } from "../types/order";

const API_URL = `${import.meta.env.VITE_API_URL}/api/orders`;

export class OrderService {
  static async sendOrder(order: OrderRequest): Promise<OrderResponse> {
    const response = await fetch(API_URL, {
      method: "POST",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify(order)
    });

    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(errorText || "Erro ao enviar ordem");
    }

    return response.json();
  }
}