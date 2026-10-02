using FlexSpace.DAL.Database;
using FlexSpace.DAL.Entities;

namespace FlexSpace.DAL.Repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly ConexionDb _db;

    public ClienteRepository(ConexionDb db)
    {
        _db = db;
    }

    public Cliente? ObtenerPorId(int id)
    {
        using var con = _db.Abrir();
        using var cmd = con.CreateCommand();
        cmd.CommandText = "SELECT Id, Nombre, Email, TipoCliente, SancionesActivas FROM Cliente WHERE Id = $id";
        cmd.Parameters.AddWithValue("$id", id);

        using var r = cmd.ExecuteReader();
        if (!r.Read()) return null;

        return new Cliente
        {
            Id = r.GetInt32(0),
            Nombre = r.GetString(1),
            Email = r.GetString(2),
            TipoCliente = Enum.Parse<TipoCliente>(r.GetString(3)),
            SancionesActivas = r.GetInt32(4)
        };
    }
}
