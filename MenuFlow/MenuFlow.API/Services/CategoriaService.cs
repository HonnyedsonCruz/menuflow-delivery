using MenuFlow.API.DTOs;
using MenuFlow.API.Models;
using MenuFlow.API.Repositories;

namespace MenuFlow.API.Services;

public class CategoriaService : ICategoriaService
{
    private readonly ICategoriaRepository _categoriaRepository;

    public CategoriaService(ICategoriaRepository categoriaRepository)
    {
        _categoriaRepository = categoriaRepository;
    }

    public async Task<List<CategoriaResponseDTO>> ListarAsync()
    {
        var categorias = await _categoriaRepository.ListarAsync();

        return categorias.Select(MapearParaResponse).ToList();
    }

    public async Task<CategoriaResponseDTO?> BuscarPorIdAsync(int id)
    {
        var categoria = await _categoriaRepository.BuscarPorIdAsync(id);

        if (categoria is null)
            return null;

        return MapearParaResponse(categoria);
    }

    public async Task<CategoriaResponseDTO> CriarAsync(CategoriaDTO categoriaDTO)
    {
        var categoria = new Categoria
        {
            Nome = categoriaDTO.Nome,
            Descricao = categoriaDTO.Descricao,
            Ativa = categoriaDTO.Ativa
        };

        await _categoriaRepository.CriarAsync(categoria);

        return MapearParaResponse(categoria);
    }

    public async Task<bool> AtualizarAsync(int id, CategoriaDTO categoriaDTO)
    {
        var categoria = await _categoriaRepository.BuscarEntidadePorIdAsync(id);

        if (categoria is null)
            return false;

        categoria.Nome = categoriaDTO.Nome;
        categoria.Descricao = categoriaDTO.Descricao;
        categoria.Ativa = categoriaDTO.Ativa;

        await _categoriaRepository.SalvarAlteracoesAsync();

        return true;
    }

    public async Task<bool> RemoverAsync(int id)
    {
        var categoria = await _categoriaRepository.BuscarEntidadePorIdAsync(id);

        if (categoria is null)
            return false;

        await _categoriaRepository.RemoverAsync(categoria);

        return true;
    }

    private static CategoriaResponseDTO MapearParaResponse(Categoria categoria)
    {
        return new CategoriaResponseDTO
        {
            Id = categoria.Id,
            Nome = categoria.Nome,
            Descricao = categoria.Descricao,
            Ativa = categoria.Ativa
        };
    }
}