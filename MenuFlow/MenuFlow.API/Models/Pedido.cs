using MenuFlow.API.Enums;

namespace MenuFlow.API.Models;

public class Pedido
{

    public int Id { get; set; }
    public int? UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }

    public string NomeCliente { get; set; } = string.Empty;
    public string TelefoneCliente { get; set; } = string.Empty;
    public string EnderecoEntrega { get; set; } = string.Empty;
    public string Observacao { get; set; } = string.Empty;

    public decimal Subtotal { get; set; }
    public decimal TaxaEntrega { get; set; }
    public decimal Total { get; set; }

    public StatusPedido Status { get; set; } = StatusPedido.Recebido;
    public FormaPagamento FormaPagamento { get; set; }

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public ICollection<ItemPedido> Itens { get; set; } = new List<ItemPedido>();
}