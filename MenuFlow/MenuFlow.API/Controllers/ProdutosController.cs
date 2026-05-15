using MenuFlow.API.DTOs;
using MenuFlow.API.Responses;
using MenuFlow.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MenuFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProdutosController : ControllerBase
{
    private readonly IProdutoService _produtoService;

    public ProdutosController(IProdutoService produtoService)
    {
        _produtoService = produtoService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProdutoResponseDTO>>>> Listar()
    {
        var produtos = await _produtoService.ListarAsync();

        return Ok(ApiResponse<IEnumerable<ProdutoResponseDTO>>.Ok(
            produtos,
            "Produtos listados com sucesso."
        ));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<ProdutoResponseDTO>>> BuscarPorId(int id)
    {
        var produto = await _produtoService.BuscarPorIdAsync(id);

        if (produto is null)
            return NotFound(ApiResponse<ProdutoResponseDTO>.Falha("Produto não encontrado."));

        return Ok(ApiResponse<ProdutoResponseDTO>.Ok(
            produto,
            "Produto encontrado com sucesso."
        ));
    }

    [HttpGet("categoria/{categoriaId:int}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProdutoResponseDTO>>>> ListarPorCategoria(int categoriaId)
    {
        var produtos = await _produtoService.ListarPorCategoriaAsync(categoriaId);

        return Ok(ApiResponse<IEnumerable<ProdutoResponseDTO>>.Ok(
            produtos,
            "Produtos da categoria listados com sucesso."
        ));
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ProdutoResponseDTO>>> Criar(ProdutoDTO produtoDTO)
    {
        var produto = await _produtoService.CriarAsync(produtoDTO);

        return CreatedAtAction(
            nameof(BuscarPorId),
            new { id = produto.Id },
            ApiResponse<ProdutoResponseDTO>.Ok(produto, "Produto criado com sucesso.")
        );
    }

    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar(int id, ProdutoDTO produtoDTO)
    {
        var atualizado = await _produtoService.AtualizarAsync(id, produtoDTO);

        if (!atualizado)
            return NotFound(ApiResponse<object>.Falha("Produto não encontrado."));

        return Ok(ApiResponse<object>.Ok(null, "Produto atualizado com sucesso."));
    }

    [Authorize]
    [HttpPatch("{id:int}/disponibilidade")]
    public async Task<ActionResult<ApiResponse<ProdutoResponseDTO>>> AlterarDisponibilidade(int id)
    {
        var produto = await _produtoService.AlterarDisponibilidadeAsync(id);

        if (produto is null)
            return NotFound(ApiResponse<ProdutoResponseDTO>.Falha("Produto não encontrado."));

        return Ok(ApiResponse<ProdutoResponseDTO>.Ok(
            produto,
            "Disponibilidade do produto alterada com sucesso."
        ));
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Remover(int id)
    {
        var removido = await _produtoService.RemoverAsync(id);

        if (!removido)
            return NotFound(ApiResponse<object>.Falha("Produto não encontrado."));

        return Ok(ApiResponse<object>.Ok(null, "Produto removido com sucesso."));
    }
}