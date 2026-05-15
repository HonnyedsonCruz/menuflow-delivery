using MenuFlow.API.DTOs;

namespace MenuFlow.API.Services;

public interface IProdutoService
{
    Task<List<ProdutoResponseDTO>> ListarAsync();
    Task<ProdutoResponseDTO?> BuscarPorIdAsync(int id);
    Task<List<ProdutoResponseDTO>> ListarPorCategoriaAsync(int categoriaId);
    Task<ProdutoResponseDTO> CriarAsync(ProdutoDTO produtoDTO);
    Task<bool> AtualizarAsync(int id, ProdutoDTO produtoDTO);
    Task<ProdutoResponseDTO?> AlterarDisponibilidadeAsync(int id);
    Task<bool> RemoverAsync(int id);
}