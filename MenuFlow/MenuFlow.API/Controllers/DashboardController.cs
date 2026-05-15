using MenuFlow.API.DTOs;
using MenuFlow.API.Responses;
using MenuFlow.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MenuFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("resumo")]
    public async Task<ActionResult<ApiResponse<DashboardResumoDTO>>> Resumo()
    {
        var resumo = await _dashboardService.ObterResumoAsync();

        return Ok(ApiResponse<DashboardResumoDTO>.Ok(
            resumo,
            "Resumo do dashboard obtido com sucesso."
        ));
    }

    [HttpGet("produtos-mais-vendidos")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProdutoMaisVendidoDTO>>>> ProdutosMaisVendidos()
    {
        var produtos = await _dashboardService.ObterProdutosMaisVendidosAsync();

        return Ok(ApiResponse<IEnumerable<ProdutoMaisVendidoDTO>>.Ok(
            produtos,
            "Produtos mais vendidos obtidos com sucesso."
        ));
    }

    [HttpGet("pedidos-por-status")]
    public async Task<ActionResult<ApiResponse<IEnumerable<PedidosPorStatusDTO>>>> PedidosPorStatus()
    {
        var dados = await _dashboardService.ObterPedidosPorStatusAsync();

        return Ok(ApiResponse<IEnumerable<PedidosPorStatusDTO>>.Ok(
            dados,
            "Pedidos por status obtidos com sucesso."
        ));
    }

    [HttpGet("faturamento-mensal/{ano:int}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<FaturamentoMensalDTO>>>> FaturamentoMensal(int ano)
    {
        var dados = await _dashboardService.ObterFaturamentoMensalAsync(ano);

        return Ok(ApiResponse<IEnumerable<FaturamentoMensalDTO>>.Ok(
            dados,
            "Faturamento mensal obtido com sucesso."
        ));
    }
}