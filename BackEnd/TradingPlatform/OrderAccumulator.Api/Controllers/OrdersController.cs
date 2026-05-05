using Microsoft.AspNetCore.Mvc;
using OrderAccumulator.Application.Dtos;
using OrderAccumulator.Application.Interfaces;

namespace OrderAccumulator.Api.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController(IOrderService orderService) : ControllerBase
{
    /// <summary>
    /// Cria uma nova ordem
    /// </summary>
    /// <param name="orderRequest">Dados da ordem</param>
    /// <returns>Resposta com o resultado da ordem</returns>
    /// <response code="200">Ordem criada com sucesso</response>
    /// <response code="400">Dados inválidos ou erro ao processar</response>
    [HttpPost]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] OrderRequest orderRequest)
    {       
        try
        {
            var response = await orderService.SendOrderAsync(orderRequest);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new 
            { 
                message = "Erro de validação", 
                error = ex.Message 
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new 
            { 
                message = "Erro ao processar ordem", 
                error = ex.Message 
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new 
            { 
                message = "Erro interno do servidor", 
                error = ex.Message 
            });
        }
    }
}