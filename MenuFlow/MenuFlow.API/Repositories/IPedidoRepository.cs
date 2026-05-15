using MenuFlow.API.Models;

namespace MenuFlow.API.Repositories;

public interface IPedidoRepository
{
    Task<List<Pedido>> ListarAsync();
    Task<Pedido?> BuscarPorIdAsync(int id);
    Task<List<Pedido>> ListarPorTelefoneUltimas24HorasAsync(string telefone);
    Task CriarAsync(Pedido pedido);
    Task<Pedido?> BuscarEntidadePorIdAsync(int id);
    Task SalvarAlteracoesAsync();
}