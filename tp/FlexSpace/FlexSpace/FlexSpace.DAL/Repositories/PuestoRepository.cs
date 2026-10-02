using FlexSpace.DAL.Database;
using FlexSpace.DAL.Entities;

namespace FlexSpace.DAL.Repositories;

public class PuestoRepository : IPuestoRepository
{
    private readonly ConexionDb _db;

    public PuestoRepository(ConexionDb db)
    {
        _db = db;
    }

    public Puesto? ObtenerPorId(int id)
    {
        using var con = _db.Abrir();
        using var cmd = con.CreateCommand();
        cmd.CommandText = "SELECT Id, Codigo, TipoPuesto, TarifaBasePorHora FROM Puesto WHERE Id = $id";
        cmd.Parameters.AddWithValue("$id", id);

        using var r = cmd.ExecuteReader();
        if (!r.Read()) return null;

        return new Puesto
        {
            Id = r.GetInt32(0),
            Codigo = r.GetString(1),
            TipoPuesto = Enum.Parse<TipoPuesto>(r.GetString(2)),
            TarifaBasePorHora = Convert.ToDecimal(r.GetDouble(3))
        };
    }
}
