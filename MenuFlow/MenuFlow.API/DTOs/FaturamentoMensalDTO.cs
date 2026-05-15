namespace MenuFlow.API.DTOs;

public class FaturamentoMensalDTO
{
    public int Mes { get; set; }
    public decimal Faturamento { get; set; }
    public int QuantidadePedidos { get; set; }
}