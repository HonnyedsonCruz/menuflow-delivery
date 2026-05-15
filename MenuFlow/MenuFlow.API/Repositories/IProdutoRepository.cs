using MenuFlow.API.Models;

namespace MenuFlow.API.Repositories;

public interface IProdutoRepository
{
    Task<List<Produto>> ListarAsync();
    Task<Produto?> BuscarPorIdAsync(int id);
    Task<List<Produto>> ListarPorCategoriaAsync(int categoriaId);
    Task CriarAsync(Produto produto);
    Task<Produto?> BuscarEntidadePorIdAsync(int id);
    Task<bool> CategoriaExisteAsync(int categoriaId);
    Task SalvarAlteracoesAsync();
    Task RemoverAsync(Produto produto);
}