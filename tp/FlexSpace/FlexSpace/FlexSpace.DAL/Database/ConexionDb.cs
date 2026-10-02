using Microsoft.Data.Sqlite;

namespace FlexSpace.DAL.Database;

/// <summary>Fabrica conexiones a SQLite. Unico punto donde se conoce la cadena de conexion.</summary>
public class ConexionDb
{
    private readonly string _cadenaConexion;

    public ConexionDb(string cadenaConexion)
    {
        _cadenaConexion = cadenaConexion;
    }

    public SqliteConnection Abrir()
    {
        var con = new SqliteConnection(_cadenaConexion);
        con.Open();
        using var cmd = con.CreateCommand();
        cmd.CommandText = "PRAGMA foreign_keys = ON;";
        cmd.ExecuteNonQuery();
        return con;
    }
}
