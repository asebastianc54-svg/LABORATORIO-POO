using FlexSpace.BLL.Servicios;
using FlexSpace.BLL.Tarifas;
using FlexSpace.DAL.Database;
using FlexSpace.DAL.Repositories;

namespace FlexSpace.BLL;

/// <summary>
/// Arma los objetos de la DAL y de la BLL. Asi la UI solo necesita referenciar la BLL
/// y nunca toca la capa de datos directamente.
/// </summary>
public static class FlexSpaceFactory
{
    public static void InicializarBaseDeDatos(string cadenaConexion)
    {
        DbInitializer.Inicializar(new ConexionDb(cadenaConexion));
    }

    public static ReservaService CrearReservaService(string cadenaConexion)
    {
        var db = new ConexionDb(cadenaConexion);
        return new ReservaService(
            new ClienteRepository(db),
            new PuestoRepository(db),
            new ReservaRepository(db),
            new CalculadorTarifa());
    }
}
