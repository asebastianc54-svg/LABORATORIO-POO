using FlexSpace.DAL.Entities;

namespace FlexSpace.BLL.Tarifas;

/// <summary>Datos de entrada que necesitan las reglas para calcular el precio.</summary>
public class ContextoTarifa
{
    public decimal TarifaBasePorHora { get; init; }
    public DateTime Inicio { get; init; }
    public DateTime Fin { get; init; }
    public TipoCliente TipoCliente { get; init; }
    public int SancionesActivas { get; init; }

    public decimal Horas => (decimal)(Fin - Inicio).TotalHours;

    /// <summary>True si algun dia del rango (sin contar el instante exacto de fin) es sabado o domingo.</summary>
    public bool IncluyeFinDeSemana
    {
        get
        {
            var ultimoInstante = Fin > Inicio ? Fin.AddTicks(-1) : Fin;
            for (var dia = Inicio.Date; dia <= ultimoInstante.Date; dia = dia.AddDays(1))
            {
                if (dia.DayOfWeek == DayOfWeek.Saturday || dia.DayOfWeek == DayOfWeek.Sunday)
                    return true;
            }
            return false;
        }
    }
}
