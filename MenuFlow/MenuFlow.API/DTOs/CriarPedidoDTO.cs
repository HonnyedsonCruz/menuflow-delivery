using MenuFlow.API.Enums;

namespace MenuFlow.API.DTOs;

public class CriarPedidoDTO
{
    public string NomeCliente { get; set; } = string.Empty;
    public string TelefoneCliente { get; set; } = string.Empty;
    public string EnderecoEntrega { get; set; } = string.Empty;
    public string Observacao { get; set; } = string.Empty;

    public FormaPagamento FormaPagamento { get; set; }

    public List<ItemPedidoDTO> Itens { get; set; } = new();
}