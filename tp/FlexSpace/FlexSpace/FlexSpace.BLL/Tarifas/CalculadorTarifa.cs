namespace FlexSpace.BLL.Tarifas;

/// <summary>Ejecuta las reglas en el orden pedido por la consigna.</summary>
public class CalculadorTarifa
{
    private readonly IReadOnlyList<IReglaTarifa> _reglas;

    public CalculadorTarifa() : this(new IReglaTarifa[]
    {
        new ReglaSubtotalBase(),
        new ReglaRecargoFinDeSemana(),
        new ReglaDescuentoVolumen(),
        new ReglaDescuentoVip(),
        new ReglaPenalizacionPorSanciones()
    })
    {
    }

    public CalculadorTarifa(IEnumerable<IReglaTarifa> reglas)
    {
        _reglas = reglas.ToList();
    }

    public DesgloseTarifa Calcular(ContextoTarifa contexto)
    {
        var desglose = new DesgloseTarifa();
        foreach (var regla in _reglas)
            regla.Aplicar(contexto, desglose);
        return desglose;
    }

    /// <summary>Redondeo comercial a 2 decimales para el monto final.</summary>
    public static decimal Redondear(decimal monto) => Math.Round(monto, 2, MidpointRounding.AwayFromZero);
}
