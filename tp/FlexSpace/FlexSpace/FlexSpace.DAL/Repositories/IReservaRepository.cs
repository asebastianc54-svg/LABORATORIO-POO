using FlexSpace.DAL.Entities;

namespace FlexSpace.DAL.Repositories;

public interface IReservaRepository
{
    Reserva? ObtenerPorId(int id);

    /// <summary>Reservas en estado Confirmada del puesto cuyo rango se cruza con [inicio, fin).</summary>
    List<Reserva> ObtenerConfirmadasSolapadas(int puestoId, DateTime inicio, DateTime fin);

    int Insertar(Reserva reserva);

    /// <summary>
    /// Marca la reserva como Cancelada. Si clienteAPenalizarId tiene valor, suma +1 a sus sanciones
    /// en la MISMA transaccion. La decision de penalizar la toma la BLL; aca solo se ejecuta.
    /// </summary>
    void Cancelar(int reservaId, int? clienteAPenalizarId);
}
