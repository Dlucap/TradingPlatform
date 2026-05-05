using System.ComponentModel.DataAnnotations;

namespace OrderAccumulator.Application.Dtos;

public class OrderRequest
{
    [Required(ErrorMessage = "O símbolo é obrigatório")]
    [StringLength(10, MinimumLength = 1, ErrorMessage = "O símbolo deve ter entre 1 e 10 caracteres")]
    public string Symbol { get; set; } = string.Empty;

    [Required(ErrorMessage = "O lado da ordem (Buy/Sell) é obrigatório")]
    [RegularExpression("^(Buy|Sell|BUY|SELL)$", ErrorMessage = "Side deve ser 'Buy' ou 'Sell'")]
    public string Side { get; set; } = string.Empty;

    [Required(ErrorMessage = "A quantidade é obrigatória")]
    [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser um número inteiro positivo")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "O preço é obrigatório")]
    [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser maior que zero")]
    public decimal Price { get; set; }
}