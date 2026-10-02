namespace FlexSpace.BLL.Exceptions;

/// <summary>Violacion generica de una regla de negocio (fechas invalidas, estado incorrecto, etc.).</summary>
public class ReglaNegocioException : Exception
{
    public ReglaNegocioException(string mensaje) : base(mensaje)
    {
    }
}
