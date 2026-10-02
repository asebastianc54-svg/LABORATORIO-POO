namespace FlexSpace.BLL.Exceptions;

public class EntidadNoEncontradaException : Exception
{
    public EntidadNoEncontradaException(string entidad, int id)
        : base($"No existe {entidad} con Id {id}.")
    {
    }
}
