namespace MenuFlow.API.DTOs;

public class DashboardResumoDTO
{
    public int TotalPedidos { get; set; }
    public int PedidosHoje { get; set; }
    public int PedidosPendentes { get; set; }
    public decimal FaturamentoTotal { get; set; }
    public decimal FaturamentoHoje { get; set; }
}