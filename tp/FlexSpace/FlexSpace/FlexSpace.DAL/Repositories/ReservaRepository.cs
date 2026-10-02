using FlexSpace.DAL.Database;
using FlexSpace.DAL.Entities;
using Microsoft.Data.Sqlite;

namespace FlexSpace.DAL.Repositories;

public class ReservaRepository : IReservaRepository
{
    private const string Columnas = "Id, ClienteId, PuestoId, FechaInicio, FechaFin, Estado, CostoTotal";

    private readonly ConexionDb _db;

    public ReservaRepository(ConexionDb db)
    {
        _db = db;
    }

    public Reserva? ObtenerPorId(int id)
    {
        using var con = _db.Abrir();
        using var cmd = con.CreateCommand();
        cmd.CommandText = $"SELECT {Columnas} FROM Reserva WHERE Id = $id";
        cmd.Parameters.AddWithValue("$id", id);

        using var r = cmd.ExecuteReader();
        return r.Read() ? Mapear(r) : null;
    }

    public List<Reserva> ObtenerConfirmadasSolapadas(int puestoId, DateTime inicio, DateTime fin)
    {
        using var con = _db.Abrir();
        using var cmd = con.CreateCommand();
        // Dos rangos se solapan si cada uno empieza antes de que termine el otro
        cmd.CommandText = $@"SELECT {Columnas} FROM Reserva
                             WHERE PuestoId = $puesto
                               AND Estado = 'Confirmada'
                               AND FechaInicio < $fin
                               AND FechaFin > $inicio";
        cmd.Parameters.AddWithValue("$puesto", puestoId);
        cmd.Parameters.AddWithValue("$inicio", FormatoFecha.Guardar(inicio));
        cmd.Parameters.AddWithValue("$fin", FormatoFecha.Guardar(fin));

        var lista = new List<Reserva>();
        using var r = cmd.ExecuteReader();
        while (r.Read()) lista.Add(Mapear(r));
        return lista;
    }

    public int Insertar(Reserva reserva)
    {
        using var con = _db.Abrir();
        using var cmd = con.CreateCommand();
        cmd.CommandText = @"INSERT INTO Reserva (ClienteId, PuestoId, FechaInicio, FechaFin, Estado, CostoTotal)
                            VALUES ($c, $p, $i, $f, $e, $t);
                            SELECT last_insert_rowid();";
        cmd.Parameters.AddWithValue("$c", reserva.ClienteId);
        cmd.Parameters.AddWithValue("$p", reserva.PuestoId);
        cmd.Parameters.AddWithValue("$i", FormatoFecha.Guardar(reserva.FechaInicio));
        cmd.Parameters.AddWithValue("$f", FormatoFecha.Guardar(reserva.FechaFin));
        cmd.Parameters.AddWithValue("$e", reserva.Estado.ToString());
        cmd.Parameters.AddWithValue("$t", (double)reserva.CostoTotal);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public void Cancelar(int reservaId, int? clienteAPenalizarId)
    {
        using var con = _db.Abrir();
        using var tx = con.BeginTransaction();

        using (var cmd = con.CreateCommand())
        {
            cmd.Transaction = tx;
            cmd.CommandText = "UPDATE Reserva SET Estado = 'Cancelada' WHERE Id = $id";
            cmd.Parameters.AddWithValue("$id", reservaId);
            cmd.ExecuteNonQuery();
        }

        if (clienteAPenalizarId.HasValue)
        {
            using var cmd = con.CreateCommand();
            cmd.Transaction = tx;
            cmd.CommandText = "UPDATE Cliente SET SancionesActivas = SancionesActivas + 1 WHERE Id = $id";
            cmd.Parameters.AddWithValue("$id", clienteAPenalizarId.Value);
            cmd.ExecuteNonQuery();
        }

        tx.Commit();
    }

    private static Reserva Mapear(SqliteDataReader r) => new()
    {
        Id = r.GetInt32(0),
        ClienteId = r.GetInt32(1),
        PuestoId = r.GetInt32(2),
        FechaInicio = FormatoFecha.Leer(r.GetString(3)),
        FechaFin = FormatoFecha.Leer(r.GetString(4)),
        Estado = Enum.Parse<EstadoReserva>(r.GetString(5)),
        CostoTotal = Convert.ToDecimal(r.GetDouble(6))
    };
}
