using FlexSpace.DAL.Entities;

namespace FlexSpace.DAL.Repositories;

public interface IPuestoRepository
{
    Puesto? ObtenerPorId(int id);
}
