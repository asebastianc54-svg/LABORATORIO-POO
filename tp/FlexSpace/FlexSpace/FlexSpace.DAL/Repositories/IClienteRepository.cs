using FlexSpace.DAL.Entities;

namespace FlexSpace.DAL.Repositories;

public interface IClienteRepository
{
    Cliente? ObtenerPorId(int id);
}
