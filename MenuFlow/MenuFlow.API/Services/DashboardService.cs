using MenuFlow.API.Data;
using MenuFlow.API.DTOs;
using MenuFlow.API.Enums;
using Microsoft.EntityFrameworkCore;

namespace MenuFlow.API.Services;

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _context;

    public DashboardService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardResumoDTO> ObterResumoAsync()
    {
        var hoje = DateTime.UtcNow.Date;

        var totalPedidos = await _context.Pedidos.CountAsync();

        var pedidosHoje = await _context.Pedidos
            .CountAsync(p => p.CriadoEm.Date == hoje);

        var pedidosPendentes = await _context.Pedidos
            .CountAsync(p =>
                p.Status == StatusPedido.Recebido ||
                p.Status == StatusPedido.EmPreparo ||
                p.Status == StatusPedido.SaiuParaEntrega);

        var faturamentoTotal = await _context.Pedidos
            .Where(p => p.Status != StatusPedido.Cancelado)
            .SumAsync(p => p.Total);

        var faturamentoHoje = await _context.Pedidos
            .Where(p => p.CriadoEm.Date == hoje && p.Status != StatusPedido.Cancelado)
            .SumAsync(p => p.Total);

        return new DashboardResumoDTO
        {
            TotalPedidos = totalPedidos,
            PedidosHoje = pedidosHoje,
            PedidosPendentes = pedidosPendentes,
            FaturamentoTotal = faturamentoTotal,
            FaturamentoHoje = faturamentoHoje
        };
    }

    public async Task<List<ProdutoMaisVendidoDTO>> ObterProdutosMaisVendidosAsync()
    {
        return await _context.ItensPedido
            .GroupBy(i => new { i.ProdutoId, i.NomeProduto })
            .Select(g => new ProdutoMaisVendidoDTO
            {
                ProdutoId = g.Key.ProdutoId,
                NomeProduto = g.Key.NomeProduto,
                QuantidadeVendida = g.Sum(i => i.Quantidade),
                TotalVendido = g.Sum(i => i.Subtotal)
            })
            .OrderByDescending(p => p.QuantidadeVendida)
            .Take(5)
            .ToListAsync();
    }

    public async Task<List<PedidosPorStatusDTO>> ObterPedidosPorStatusAsync()
    {
        return await _context.Pedidos
            .GroupBy(p => p.Status)
            .Select(g => new PedidosPorStatusDTO
            {
                Status = g.Key.ToString(),
                Quantidade = g.Count()
            })
            .ToListAsync();
    }

    public async Task<List<FaturamentoMensalDTO>> ObterFaturamentoMensalAsync(int ano)
    {
        return await _context.Pedidos
            .Where(p => p.CriadoEm.Year == ano && p.Status != StatusPedido.Cancelado)
            .GroupBy(p => p.CriadoEm.Month)
            .Select(g => new FaturamentoMensalDTO
            {
                Mes = g.Key,
                Faturamento = g.Sum(p => p.Total),
                QuantidadePedidos = g.Count()
            })
            .OrderBy(x => x.Mes)
            .ToListAsync();
    }
}