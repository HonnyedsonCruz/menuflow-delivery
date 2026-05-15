using MenuFlow.API.DTOs;
using MenuFlow.API.Models;
using MenuFlow.API.Repositories;
using MenuFlow.API.Exceptions;

namespace MenuFlow.API.Services;

public class ProdutoService : IProdutoService
{
    private readonly IProdutoRepository _produtoRepository;

    public ProdutoService(IProdutoRepository produtoRepository)
    {
        _produtoRepository = produtoRepository;
    }

    public async Task<List<ProdutoResponseDTO>> ListarAsync()
    {
        var produtos = await _produtoRepository.ListarAsync();

        return produtos.Select(MapearParaResponse).ToList();
    }

    public async Task<ProdutoResponseDTO?> BuscarPorIdAsync(int id)
    {
        var produto = await _produtoRepository.BuscarPorIdAsync(id);

        if (produto is null)
            return null;

        return MapearParaResponse(produto);
    }

    public async Task<List<ProdutoResponseDTO>> ListarPorCategoriaAsync(int categoriaId)
    {
        var produtos = await _produtoRepository.ListarPorCategoriaAsync(categoriaId);

        return produtos.Select(MapearParaResponse).ToList();
    }

    public async Task<ProdutoResponseDTO> CriarAsync(ProdutoDTO produtoDTO)
    {
        var categoriaExiste = await _produtoRepository.CategoriaExisteAsync(produtoDTO.CategoriaId);

        if (!categoriaExiste)
            throw new RegraDeNegocioException("Categoria informada não existe.");

        var produto = new Produto
        {
            Nome = produtoDTO.Nome,
            Descricao = produtoDTO.Descricao,
            Preco = produtoDTO.Preco,
            ImagemUrl = produtoDTO.ImagemUrl,
            Disponivel = produtoDTO.Disponivel,
            CategoriaId = produtoDTO.CategoriaId
        };

        await _produtoRepository.CriarAsync(produto);

        var produtoCriado = await _produtoRepository.BuscarPorIdAsync(produto.Id);

        return MapearParaResponse(produtoCriado!);
    }

    public async Task<bool> AtualizarAsync(int id, ProdutoDTO produtoDTO)
    {
        var produtoExistente = await _produtoRepository.BuscarEntidadePorIdAsync(id);

        if (produtoExistente is null)
            return false;

        var categoriaExiste = await _produtoRepository.CategoriaExisteAsync(produtoDTO.CategoriaId);

        if (!categoriaExiste)
            throw new RegraDeNegocioException("Categoria informada não existe.");

        produtoExistente.Nome = produtoDTO.Nome;
        produtoExistente.Descricao = produtoDTO.Descricao;
        produtoExistente.Preco = produtoDTO.Preco;
        produtoExistente.ImagemUrl = produtoDTO.ImagemUrl;
        produtoExistente.Disponivel = produtoDTO.Disponivel;
        produtoExistente.CategoriaId = produtoDTO.CategoriaId;

        await _produtoRepository.SalvarAlteracoesAsync();

        return true;
    }

    public async Task<ProdutoResponseDTO?> AlterarDisponibilidadeAsync(int id)
    {
        var produto = await _produtoRepository.BuscarEntidadePorIdAsync(id);

        if (produto is null)
            return null;

        produto.Disponivel = !produto.Disponivel;

        await _produtoRepository.SalvarAlteracoesAsync();

        var produtoAtualizado = await _produtoRepository.BuscarPorIdAsync(id);

        return MapearParaResponse(produtoAtualizado!);
    }

    public async Task<bool> RemoverAsync(int id)
    {
        var produto = await _produtoRepository.BuscarEntidadePorIdAsync(id);

        if (produto is null)
            return false;

        await _produtoRepository.RemoverAsync(produto);

        return true;
    }

    private static ProdutoResponseDTO MapearParaResponse(Produto produto)
    {
        return new ProdutoResponseDTO
        {
            Id = produto.Id,
            Nome = produto.Nome,
            Descricao = produto.Descricao,
            Preco = produto.Preco,
            ImagemUrl = produto.ImagemUrl,
            Disponivel = produto.Disponivel,
            CategoriaId = produto.CategoriaId,
            CategoriaNome = produto.Categoria?.Nome ?? string.Empty
        };
    }
}