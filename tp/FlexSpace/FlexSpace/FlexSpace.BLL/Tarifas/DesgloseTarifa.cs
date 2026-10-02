namespace FlexSpace.BLL.Tarifas;

/// <summary>Cada regla va completando su parte; el Total siempre se deriva de los componentes.</summary>
public class DesgloseTarifa
{
    public decimal Subtotal { get; set; }
    public decimal RecargoFinDeSemana { get; set; }
    public decimal DescuentoVolumen { get; set; }
    public decimal DescuentoVip { get; set; }
    public decimal RecargoSancion { get; set; }

    /// <summary>Total acumulado hasta el momento (sin redondear).</summary>
    public decimal TotalAcumulado =>
        Subtotal + RecargoFinDeSemana - DescuentoVolumen - DescuentoVip + RecargoSancion;
}
