namespace MenuFlow.API.DTOs;

public class ProdutoDTO
{
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public string ImagemUrl { get; set; } = string.Empty;
    public bool Disponivel { get; set; } = true;
    public int CategoriaId { get; set; }
}