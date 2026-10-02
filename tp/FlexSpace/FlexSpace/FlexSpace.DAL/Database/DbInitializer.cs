using Microsoft.Data.Sqlite;

namespace FlexSpace.DAL.Database;

/// <summary>Crea el esquema si no existe y carga datos de prueba la primera vez.</summary>
public static class DbInitializer
{
    public static void Inicializar(ConexionDb db)
    {
        using var con = db.Abrir();

        using (var cmd = con.CreateCommand())
        {
            cmd.CommandText = @"
CREATE TABLE IF NOT EXISTS Cliente (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Nombre TEXT NOT NULL,
    Email TEXT NOT NULL UNIQUE,
    TipoCliente TEXT NOT NULL CHECK (TipoCliente IN ('Estandar','VIP')),
    SancionesActivas INTEGER NOT NULL DEFAULT 0
);
CREATE TABLE IF NOT EXISTS Puesto (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Codigo TEXT NOT NULL UNIQUE,
    TipoPuesto TEXT NOT NULL CHECK (TipoPuesto IN ('EscritorioIndividual','SalaReuniones','CabinaPrivada')),
    TarifaBasePorHora REAL NOT NULL
);
CREATE TABLE IF NOT EXISTS Reserva (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    ClienteId INTEGER NOT NULL REFERENCES Cliente(Id),
    PuestoId INTEGER NOT NULL REFERENCES Puesto(Id),
    FechaInicio TEXT NOT NULL,
    FechaFin TEXT NOT NULL,
    Estado TEXT NOT NULL CHECK (Estado IN ('Confirmada','Cancelada','Finalizada')),
    CostoTotal REAL NOT NULL
);";
            cmd.ExecuteNonQuery();
        }

        if (TablaVacia(con, "Cliente")) CargarClientes(con);
        if (TablaVacia(con, "Puesto")) CargarPuestos(con);
    }

    private static bool TablaVacia(SqliteConnection con, string tabla)
    {
        using var cmd = con.CreateCommand();
        cmd.CommandText = $"SELECT COUNT(*) FROM {tabla}";
        return Convert.ToInt64(cmd.ExecuteScalar()) == 0;
    }

    private static void CargarClientes(SqliteConnection con)
    {
        // Datos de prueba: uno por cada caso de la tarifa
        InsertarCliente(con, "Ana Perez", "ana@mail.com", "Estandar", 0);
        InsertarCliente(con, "Bruno Gomez", "bruno@mail.com", "VIP", 0);
        InsertarCliente(con, "Carla Diaz", "carla@mail.com", "Estandar", 1);   // con sancion
        InsertarCliente(con, "Diego Ruiz", "diego@mail.com", "Estandar", 3);   // bloqueado
        InsertarCliente(con, "Eva Lopez", "eva@mail.com", "VIP", 2);            // VIP con sanciones
    }

    private static void CargarPuestos(SqliteConnection con)
    {
        InsertarPuesto(con, "ESC-01", "EscritorioIndividual", 1000m);
        InsertarPuesto(con, "ESC-02", "EscritorioIndividual", 1000m);
        InsertarPuesto(con, "SALA-01", "SalaReuniones", 3000m);
        InsertarPuesto(con, "CAB-01", "CabinaPrivada", 2000m);
    }

    private static void InsertarCliente(SqliteConnection con, string nombre, string email, string tipo, int sanciones)
    {
        using var cmd = con.CreateCommand();
        cmd.CommandText = "INSERT INTO Cliente (Nombre, Email, TipoCliente, SancionesActivas) VALUES ($n, $e, $t, $s)";
        cmd.Parameters.AddWithValue("$n", nombre);
        cmd.Parameters.AddWithValue("$e", email);
        cmd.Parameters.AddWithValue("$t", tipo);
        cmd.Parameters.AddWithValue("$s", sanciones);
        cmd.ExecuteNonQuery();
    }

    private static void InsertarPuesto(SqliteConnection con, string codigo, string tipo, decimal tarifa)
    {
        using var cmd = con.CreateCommand();
        cmd.CommandText = "INSERT INTO Puesto (Codigo, TipoPuesto, TarifaBasePorHora) VALUES ($c, $t, $f)";
        cmd.Parameters.AddWithValue("$c", codigo);
        cmd.Parameters.AddWithValue("$t", tipo);
        cmd.Parameters.AddWithValue("$f", (double)tarifa);
        cmd.ExecuteNonQuery();
    }
}
