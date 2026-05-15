using MenuFlow.API.Data;
using MenuFlow.API.Models;
using Microsoft.EntityFrameworkCore;

namespace MenuFlow.API.Repositories;

public class ProdutoRepository : IProdutoRepository
{
    private readonly AppDbContext _context;

    public ProdutoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Produto>> ListarAsync()
    {
        return await _context.Produtos
            .Include(p => p.Categoria)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Produto?> BuscarPorIdAsync(int id)
    {
        return await _context.Produtos
            .Include(p => p.Categoria)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<Produto>> ListarPorCategoriaAsync(int categoriaId)
    {
        return await _context.Produtos
            .Include(p => p.Categoria)
            .AsNoTracking()
            .Where(p => p.CategoriaId == categoriaId)
            .ToListAsync();
    }

    public async Task CriarAsync(Produto produto)
    {
        _context.Produtos.Add(produto);
        await _context.SaveChangesAsync();
    }

    public async Task<Produto?> BuscarEntidadePorIdAsync(int id)
    {
        return await _context.Produtos
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<bool> CategoriaExisteAsync(int categoriaId)
    {
        return await _context.Categorias
            .AnyAsync(c => c.Id == categoriaId);
    }

    public async Task SalvarAlteracoesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task RemoverAsync(Produto produto)
    {
        _context.Produtos.Remove(produto);
        await _context.SaveChangesAsync();
    }
}