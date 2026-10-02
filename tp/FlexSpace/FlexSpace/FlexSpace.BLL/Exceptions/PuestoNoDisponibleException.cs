namespace FlexSpace.BLL.Exceptions;

/// <summary>El puesto ya tiene una reserva Confirmada que se solapa con el rango pedido.</summary>
public class PuestoNoDisponibleException : Exception
{
    public PuestoNoDisponibleException(string codigoPuesto, DateTime inicio, DateTime fin)
        : base($"El puesto {codigoPuesto} no esta disponible entre {inicio:dd/MM/yyyy HH:mm} y {fin:dd/MM/yyyy HH:mm}.")
    {
    }
}
