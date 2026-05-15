namespace MenuFlow.API.DTOs;

public class ProdutoMaisVendidoDTO
{
    public int ProdutoId { get; set; }
    public string NomeProduto { get; set; } = string.Empty;
    public int QuantidadeVendida { get; set; }
    public decimal TotalVendido { get; set; }
}