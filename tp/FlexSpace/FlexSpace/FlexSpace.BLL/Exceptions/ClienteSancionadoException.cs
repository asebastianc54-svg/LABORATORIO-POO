namespace FlexSpace.BLL.Exceptions;

/// <summary>El cliente acumulo 3 o mas sanciones y no puede reservar.</summary>
public class ClienteSancionadoException : Exception
{
    public ClienteSancionadoException(string nombreCliente, int sanciones)
        : base($"El cliente {nombreCliente} tiene {sanciones} sanciones activas y no puede realizar reservas.")
    {
    }
}
