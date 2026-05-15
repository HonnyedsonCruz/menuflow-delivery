using MenuFlow.API.DTOs;
using MenuFlow.API.Responses;
using MenuFlow.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MenuFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriasController : ControllerBase
{
    private readonly ICategoriaService _categoriaService;

    public CategoriasController(ICategoriaService categoriaService)
    {
        _categoriaService = categoriaService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<CategoriaResponseDTO>>>> Listar()
    {
        var categorias = await _categoriaService.ListarAsync();

        return Ok(ApiResponse<IEnumerable<CategoriaResponseDTO>>.Ok(
            categorias,
            "Categorias listadas com sucesso."
        ));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<CategoriaResponseDTO>>> BuscarPorId(int id)
    {
        var categoria = await _categoriaService.BuscarPorIdAsync(id);

        if (categoria is null)
            return NotFound(ApiResponse<CategoriaResponseDTO>.Falha("Categoria não encontrada."));

        return Ok(ApiResponse<CategoriaResponseDTO>.Ok(
            categoria,
            "Categoria encontrada com sucesso."
        ));
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<CategoriaResponseDTO>>> Criar(CategoriaDTO categoriaDTO)
    {
        var categoria = await _categoriaService.CriarAsync(categoriaDTO);

        return CreatedAtAction(
            nameof(BuscarPorId),
            new { id = categoria.Id },
            ApiResponse<CategoriaResponseDTO>.Ok(categoria, "Categoria criada com sucesso.")
        );
    }

    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar(int id, CategoriaDTO categoriaDTO)
    {
        var atualizado = await _categoriaService.AtualizarAsync(id, categoriaDTO);

        if (!atualizado)
            return NotFound(ApiResponse<object>.Falha("Categoria não encontrada."));

        return Ok(ApiResponse<object>.Ok(null, "Categoria atualizada com sucesso."));
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Remover(int id)
    {
        var removido = await _categoriaService.RemoverAsync(id);

        if (!removido)
            return NotFound(ApiResponse<object>.Falha("Categoria não encontrada."));

        return Ok(ApiResponse<object>.Ok(null, "Categoria removida com sucesso."));
    }
}