using MenuFlow.API.Data;
using MenuFlow.API.DTOs;
using MenuFlow.API.Enums;
using MenuFlow.API.Exceptions;
using MenuFlow.API.Models;
using MenuFlow.API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MenuFlow.API.Services;

public class PedidoService : IPedidoService
{
    private readonly IPedidoRepository _pedidoRepository;
    private readonly AppDbContext _context;

    public PedidoService(IPedidoRepository pedidoRepository, AppDbContext context)
    {
        _pedidoRepository = pedidoRepository;
        _context = context;
    }

    public async Task<List<PedidoResponseDTO>> ListarAsync()
    {
        var pedidos = await _pedidoRepository.ListarAsync();

        return pedidos.Select(MapearParaResponse).ToList();
    }

    public async Task<PedidoResponseDTO?> BuscarPorIdAsync(int id)
    {
        var pedido = await _pedidoRepository.BuscarPorIdAsync(id);

        if (pedido is null)
            return null;

        return MapearParaResponse(pedido);
    }

    public async Task<List<PedidoAcompanhamentoDTO>> AcompanharPorTelefoneAsync(string telefone)
    {
        var telefoneNormalizado = NormalizarTelefone(telefone);

        var pedidos = await _pedidoRepository
            .ListarPorTelefoneUltimas24HorasAsync(telefoneNormalizado);

        return pedidos.Select(MapearParaAcompanhamento).ToList();
    }

    public async Task<PedidoResponseDTO> CriarAsync(CriarPedidoDTO pedidoDTO)
    {
        var produtoIds = pedidoDTO.Itens.Select(i => i.ProdutoId).ToList();

        var produtos = await _context.Produtos
            .Where(p => produtoIds.Contains(p.Id) && p.Disponivel)
            .ToListAsync();

        if (produtos.Count != produtoIds.Distinct().Count())
            throw new RegraDeNegocioException("Um ou mais produtos não existem ou estão indisponíveis.");

        var pedido = new Pedido
        {
            NomeCliente = pedidoDTO.NomeCliente,
            TelefoneCliente = NormalizarTelefone(pedidoDTO.TelefoneCliente),
            EnderecoEntrega = pedidoDTO.EnderecoEntrega,
            Observacao = pedidoDTO.Observacao,
            FormaPagamento = pedidoDTO.FormaPagamento,
            Status = StatusPedido.Recebido,
            CriadoEm = DateTime.UtcNow,
            TaxaEntrega = 5.00m
        };

        foreach (var itemDTO in pedidoDTO.Itens)
        {
            var produto = produtos.First(p => p.Id == itemDTO.ProdutoId);

            var item = new ItemPedido
            {
                ProdutoId = produto.Id,
                NomeProduto = produto.Nome,
                PrecoUnitario = produto.Preco,
                Quantidade = itemDTO.Quantidade,
                Subtotal = produto.Preco * itemDTO.Quantidade
            };

            pedido.Itens.Add(item);
        }

        pedido.Subtotal = pedido.Itens.Sum(i => i.Subtotal);
        pedido.Total = pedido.Subtotal + pedido.TaxaEntrega;

        await _pedidoRepository.CriarAsync(pedido);

        return MapearParaResponse(pedido);
    }

    public async Task<bool> AlterarStatusAsync(int id, StatusPedido status)
    {
        var pedido = await _pedidoRepository.BuscarEntidadePorIdAsync(id);

        if (pedido is null)
            return false;

        pedido.Status = status;

        await _pedidoRepository.SalvarAlteracoesAsync();

        return true;
    }

    private static PedidoResponseDTO MapearParaResponse(Pedido pedido)
    {
        return new PedidoResponseDTO
        {
            Id = pedido.Id,
            NomeCliente = pedido.NomeCliente,
            TelefoneCliente = pedido.TelefoneCliente,
            EnderecoEntrega = pedido.EnderecoEntrega,
            Observacao = pedido.Observacao,
            Subtotal = pedido.Subtotal,
            TaxaEntrega = pedido.TaxaEntrega,
            Total = pedido.Total,
            Status = pedido.Status,
            FormaPagamento = pedido.FormaPagamento,
            CriadoEm = pedido.CriadoEm,
            Itens = pedido.Itens.Select(i => new ItemPedidoResponseDTO
            {
                ProdutoId = i.ProdutoId,
                NomeProduto = i.NomeProduto,
                PrecoUnitario = i.PrecoUnitario,
                Quantidade = i.Quantidade,
                Subtotal = i.Subtotal
            }).ToList()
        };
    }

    private static PedidoAcompanhamentoDTO MapearParaAcompanhamento(Pedido pedido)
    {
        return new PedidoAcompanhamentoDTO
        {
            Id = pedido.Id,
            NomeCliente = pedido.NomeCliente,
            EnderecoResumo = MascararEndereco(pedido.EnderecoEntrega),
            Observacao = pedido.Observacao,
            Subtotal = pedido.Subtotal,
            TaxaEntrega = pedido.TaxaEntrega,
            Total = pedido.Total,
            Status = pedido.Status,
            FormaPagamento = pedido.FormaPagamento,
            CriadoEm = pedido.CriadoEm,
            Itens = pedido.Itens.Select(i => new ItemPedidoResponseDTO
            {
                ProdutoId = i.ProdutoId,
                NomeProduto = i.NomeProduto,
                PrecoUnitario = i.PrecoUnitario,
                Quantidade = i.Quantidade,
                Subtotal = i.Subtotal
            }).ToList()
        };
    }

    private static string NormalizarTelefone(string telefone)
    {
        return new string(telefone.Where(char.IsDigit).ToArray());
    }

    private static string MascararEndereco(string endereco)
    {
        if (string.IsNullOrWhiteSpace(endereco))
            return "Endereço informado no pedido.";

        var partes = endereco.Split(',', StringSplitOptions.RemoveEmptyEntries);

        if (partes.Length >= 2)
            return $"{partes[0].Trim()}, ***";

        return "Endereço protegido por segurança.";
    }
}