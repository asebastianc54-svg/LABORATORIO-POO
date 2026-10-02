namespace FlexSpace.BLL.Dtos;

public class CancelacionResultadoDto
{
    public int ReservaId { get; init; }
    public bool SeAplicoSancion { get; init; }
    public int SancionesActuales { get; init; }
}
