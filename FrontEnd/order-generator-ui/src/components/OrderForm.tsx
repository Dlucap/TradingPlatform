import { useState } from "react";
import type { OrderRequest, OrderResponse } from "../types/order";
import { OrderService } from "../services/OrderService";

export default function OrderForm() {
  const [form, setForm] = useState<OrderRequest>({
    symbol: "PETR4",
    side: "BUY",
    quantity: 0,
    price: 0
  });

  const [result, setResult] = useState<OrderResponse | null>(null);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);

  const styles: Record<string, React.CSSProperties> = {
    container: {
      maxWidth: "420px",
      margin: "60px auto",
      padding: "20px",
      borderRadius: "12px",
      boxShadow: "0 4px 12px rgba(0,0,0,0.1)",
      fontFamily: "sans-serif"
    },
    input: {
      width: "100%",
      padding: "10px",
      marginBottom: "12px",
      borderRadius: "6px",
      border: "1px solid #ccc"
    },
    button: {
      width: "100%",
      padding: "12px",
      border: "none",
      borderRadius: "6px",
      cursor: "pointer",
      fontWeight: "bold"
    },
    result: {
      marginTop: "15px",
      padding: "10px",
      backgroundColor: "#f4f4f4",
      borderRadius: "6px"
    },
    error: {
      marginTop: "15px",
      padding: "10px",
      backgroundColor: "#ffdddd",
      color: "#a00",
      borderRadius: "6px"
    }
  };

  const handleChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>
  ) => {
    const { name, value } = e.target;

    setForm((prev) => ({
      ...prev,
      [name]:
        name === "quantity" || name === "price"
          ? Number(value)
          : value
    }));
  };

  const validate = (): string | null => {
  if (form.quantity <= 0 || form.quantity >= 100000) {
    return "Quantidade deve ser entre 1 e 99.999";
  }

  if (
    form.price <= 0 ||
    form.price >= 1000 ||
    !Number.isInteger(form.price * 100)
  ) {
    return "Preço inválido (múltiplo de 0.01 e < 1000)";
  }

  return null;
};

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    setError(null);
    setResult(null);

    const validationError = validate();
    if (validationError) {
      setError(validationError);
      return;
    }

    try {
      setLoading(true);

      const response = await OrderService.sendOrder(form);

      setResult(response);
    } catch (err: unknown) {
      if (err instanceof Error) {
        setError(err.message);
      } else {
        setError(String(err));
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <div style={styles.container}>
      <h2>Order Generator</h2>

      <form onSubmit={handleSubmit}>
        <select
          name="symbol"
          value={form.symbol}
          onChange={handleChange}
          style={styles.input}
        >
          <option value="PETR4">PETR4</option>
          <option value="VALE3">VALE3</option>
          <option value="VIIA4">VIIA4</option>
        </select>

        <select
          name="side"
          value={form.side}
          onChange={handleChange}
          style={styles.input}
        >
          <option value="BUY">Compra</option>
          <option value="SELL">Venda</option>
        </select>

        <input
          type="number"
          name="quantity"
          placeholder="Quantidade"
          value={form.quantity || ""}
          onChange={handleChange}
          style={styles.input}
        />

        <input
          type="number"
          step="0.01"
          name="price"
          placeholder="Preço"
          value={form.price || ""}
          onChange={handleChange}
          style={styles.input}
        />

        <button type="submit" style={styles.button} disabled={loading}>
          {loading ? "Enviando..." : "Enviar Ordem"}
        </button>
      </form>

      {result && (
        <div style={styles.result}>
          <strong>Resposta:</strong>
          <pre>{JSON.stringify(result, null, 2)}</pre>
        </div>
      )}

      {error && (
        <div style={styles.error}>
          <strong>Erro:</strong> {error}
        </div>
      )}
    </div>
  );
}