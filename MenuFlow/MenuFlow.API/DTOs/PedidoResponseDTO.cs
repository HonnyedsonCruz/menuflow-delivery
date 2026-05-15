using MenuFlow.API.Enums;

namespace MenuFlow.API.DTOs;

public class PedidoResponseDTO
{
    public int Id { get; set; }
    public string NomeCliente { get; set; } = string.Empty;
    public string TelefoneCliente { get; set; } = string.Empty;
    public string EnderecoEntrega { get; set; } = string.Empty;
    public string Observacao { get; set; } = string.Empty;

    public decimal Subtotal { get; set; }
    public decimal TaxaEntrega { get; set; }
    public decimal Total { get; set; }

    public StatusPedido Status { get; set; }
    public FormaPagamento FormaPagamento { get; set; }

    public DateTime CriadoEm { get; set; }

    public List<ItemPedidoResponseDTO> Itens { get; set; } = new();
}