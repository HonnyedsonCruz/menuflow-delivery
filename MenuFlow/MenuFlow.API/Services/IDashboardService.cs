using MenuFlow.API.DTOs;

namespace MenuFlow.API.Services;

public interface IDashboardService
{
    Task<DashboardResumoDTO> ObterResumoAsync();
    Task<List<ProdutoMaisVendidoDTO>> ObterProdutosMaisVendidosAsync();
    Task<List<PedidosPorStatusDTO>> ObterPedidosPorStatusAsync();
    Task<List<FaturamentoMensalDTO>> ObterFaturamentoMensalAsync(int ano);
}