namespace MenuFlow.API.DTOs;

public class CategoriaDTO
{
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public bool Ativa { get; set; } = true;
}