using MenuFlow.API.Data;
using MenuFlow.API.Models;
using Microsoft.EntityFrameworkCore;

namespace MenuFlow.API.Repositories;

public class PedidoRepository : IPedidoRepository
{
    private readonly AppDbContext _context;

    public PedidoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Pedido>> ListarAsync()
    {
        return await _context.Pedidos
            .Include(p => p.Itens)
            .AsNoTracking()
            .OrderByDescending(p => p.CriadoEm)
            .ToListAsync();
    }

    public async Task<Pedido?> BuscarPorIdAsync(int id)
    {
        return await _context.Pedidos
            .Include(p => p.Itens)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<Pedido>> ListarPorTelefoneUltimas24HorasAsync(string telefone)
    {
        var limite = DateTime.UtcNow.AddHours(-24);

        return await _context.Pedidos
            .Include(p => p.Itens)
            .AsNoTracking()
            .Where(p => p.TelefoneCliente == telefone && p.CriadoEm >= limite)
            .OrderByDescending(p => p.CriadoEm)
            .ToListAsync();
    }

    public async Task CriarAsync(Pedido pedido)
    {
        _context.Pedidos.Add(pedido);
        await _context.SaveChangesAsync();
    }

    public async Task<Pedido?> BuscarEntidadePorIdAsync(int id)
    {
        return await _context.Pedidos
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task SalvarAlteracoesAsync()
    {
        await _context.SaveChangesAsync();
    }
}