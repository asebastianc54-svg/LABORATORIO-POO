namespace FlexSpace.BLL.Dtos;

/// <summary>Resultado del calculo de una reserva, listo para mostrar en pantalla.</summary>
public class ResumenReservaDto
{
    public string ClienteNombre { get; init; } = string.Empty;
    public string TipoCliente { get; init; } = string.Empty;
    public int SancionesActivas { get; init; }

    public string PuestoCodigo { get; init; } = string.Empty;
    public string TipoPuesto { get; init; } = string.Empty;

    public DateTime Inicio { get; init; }
    public DateTime Fin { get; init; }
    public decimal Horas { get; init; }

    public decimal TarifaBasePorHora { get; init; }
    public decimal Subtotal { get; init; }
    public decimal RecargoFinDeSemana { get; init; }
    public decimal DescuentoVolumen { get; init; }
    public decimal DescuentoVip { get; init; }
    public decimal RecargoSancion { get; init; }
    public decimal Total { get; init; }
}
