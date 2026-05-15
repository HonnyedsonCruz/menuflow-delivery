using MenuFlow.API.Models;

namespace MenuFlow.API.Repositories;

public interface ICategoriaRepository
{
    Task<List<Categoria>> ListarAsync();
    Task<Categoria?> BuscarPorIdAsync(int id);
    Task CriarAsync(Categoria categoria);
    Task<Categoria?> BuscarEntidadePorIdAsync(int id);
    Task SalvarAlteracoesAsync();
    Task RemoverAsync(Categoria categoria);
}