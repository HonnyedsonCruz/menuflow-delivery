using MenuFlow.API.DTOs;
using MenuFlow.API.Enums;

namespace MenuFlow.API.Services;

public interface IPedidoService
{
    Task<List<PedidoResponseDTO>> ListarAsync();
    Task<PedidoResponseDTO?> BuscarPorIdAsync(int id);
    Task<List<PedidoAcompanhamentoDTO>> AcompanharPorTelefoneAsync(string telefone);
    Task<PedidoResponseDTO> CriarAsync(CriarPedidoDTO pedidoDTO);
    Task<bool> AlterarStatusAsync(int id, StatusPedido status);
}