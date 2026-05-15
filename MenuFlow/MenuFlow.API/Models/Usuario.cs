namespace MenuFlow.API.Models;

public class Usuario
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public string Telefone { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;

    public string SenhaHash { get; set; } = string.Empty;

    public string Role { get; set; } = "Cliente";

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}