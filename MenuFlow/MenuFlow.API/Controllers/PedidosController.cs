using MenuFlow.API.DTOs;
using MenuFlow.API.Enums;
using MenuFlow.API.Responses;
using MenuFlow.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MenuFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PedidosController : ControllerBase
{
    private readonly IPedidoService _pedidoService;

    public PedidosController(IPedidoService pedidoService)
    {
        _pedidoService = pedidoService;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<PedidoResponseDTO>>>> Listar()
    {
        var pedidos = await _pedidoService.ListarAsync();

        return Ok(ApiResponse<IEnumerable<PedidoResponseDTO>>.Ok(
            pedidos,
            "Pedidos listados com sucesso."
        ));
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<PedidoResponseDTO>>> BuscarPorId(int id)
    {
        var pedido = await _pedidoService.BuscarPorIdAsync(id);

        if (pedido is null)
            return NotFound(ApiResponse<PedidoResponseDTO>.Falha("Pedido não encontrado."));

        return Ok(ApiResponse<PedidoResponseDTO>.Ok(
            pedido,
            "Pedido encontrado com sucesso."
        ));
    }

    [HttpGet("acompanhar")]
    public async Task<ActionResult<ApiResponse<IEnumerable<PedidoAcompanhamentoDTO>>>> AcompanharPorTelefone([FromQuery] string telefone)
    {
        if (string.IsNullOrWhiteSpace(telefone))
            return BadRequest(ApiResponse<object>.Falha("Informe o telefone para acompanhar o pedido."));

        var pedidos = await _pedidoService.AcompanharPorTelefoneAsync(telefone);

        return Ok(ApiResponse<IEnumerable<PedidoAcompanhamentoDTO>>.Ok(
            pedidos,
            "Pedidos ativos encontrados com sucesso."
        ));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<PedidoResponseDTO>>> Criar(CriarPedidoDTO pedidoDTO)
    {
        var pedido = await _pedidoService.CriarAsync(pedidoDTO);

        return Created(
            "",
            ApiResponse<PedidoResponseDTO>.Ok(pedido, "Pedido criado com sucesso.")
        );
    }

    [Authorize(Roles = "Admin")]
    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> AlterarStatus(int id, StatusPedido status)
    {
        var atualizado = await _pedidoService.AlterarStatusAsync(id, status);

        if (!atualizado)
            return NotFound(ApiResponse<object>.Falha("Pedido não encontrado."));

        return Ok(ApiResponse<object>.Ok(
            new
            {
                pedidoId = id,
                status = status.ToString()
            },
            "Status do pedido atualizado com sucesso."
        ));
    }
}