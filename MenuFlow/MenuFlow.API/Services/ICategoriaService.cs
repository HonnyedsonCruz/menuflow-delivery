using MenuFlow.API.DTOs;

namespace MenuFlow.API.Services;

public interface ICategoriaService
{
    Task<List<CategoriaResponseDTO>> ListarAsync();
    Task<CategoriaResponseDTO?> BuscarPorIdAsync(int id);
    Task<CategoriaResponseDTO> CriarAsync(CategoriaDTO categoriaDTO);
    Task<bool> AtualizarAsync(int id, CategoriaDTO categoriaDTO);
    Task<bool> RemoverAsync(int id);
}