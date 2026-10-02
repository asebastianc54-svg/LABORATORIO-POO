using FlexSpace.BLL;
using FlexSpace.BLL.Dtos;
using FlexSpace.BLL.Exceptions;
using FlexSpace.BLL.Servicios;

namespace FlexSpace.UI;

public static class Program
{
    private const string CadenaConexion = "Data Source=flexspace.db";

    public static void Main()
    {
        FlexSpaceFactory.InicializarBaseDeDatos(CadenaConexion);
        var servicio = FlexSpaceFactory.CrearReservaService(CadenaConexion);

        var salir = false;
        while (!salir)
        {
            MostrarMenu();
            switch (Console.ReadLine()?.Trim())
            {
                case "1": RegistrarReserva(servicio); break;
                case "2": CancelarReserva(servicio); break;
                case "3": FuncionPendiente("Consultar Reservas Activas por Puesto"); break;
                case "4": FuncionPendiente("Listar Clientes Sancionados"); break;
                case "0": salir = true; break;
                default: Console.WriteLine("Opcion invalida."); break;
            }
        }
    }

    private static void MostrarMenu()
    {
        Console.WriteLine();
        Console.WriteLine("===== FlexSpace - Gestion de Reservas =====");
        Console.WriteLine("1. Registrar nueva reserva");
        Console.WriteLine("2. Cancelar reserva");
        Console.WriteLine("3. Consultar reservas activas por puesto   (pendiente)");
        Console.WriteLine("4. Listar clientes sancionados             (pendiente)");
        Console.WriteLine("0. Salir");
        Console.Write("Opcion: ");
    }

    // ---------------- Opcion 1 ----------------
    private static void RegistrarReserva(ReservaService servicio)
    {
        try
        {
            var clienteId = EntradaConsola.LeerEntero("Id de cliente: ");
            var puestoId = EntradaConsola.LeerEntero("Id de puesto: ");
            var inicio = EntradaConsola.LeerFechaHora("Fecha/hora de inicio");
            var fin = EntradaConsola.LeerFechaHora("Fecha/hora de fin");

            // La BLL valida y calcula; la UI solo muestra
            var resumen = servicio.SimularReserva(clienteId, puestoId, inicio, fin);
            MostrarResumen(resumen);

            if (!EntradaConsola.Confirmar("Confirmar reserva?"))
            {
                Console.WriteLine("Reserva descartada.");
                return;
            }

            var id = servicio.RegistrarReserva(clienteId, puestoId, inicio, fin);
            Console.WriteLine($"Reserva registrada correctamente. Id: {id}");
        }
        catch (Exception ex)
        {
            MostrarError(ex);
        }
    }

    private static void MostrarResumen(ResumenReservaDto r)
    {
        Console.WriteLine();
        Console.WriteLine("----- Resumen de la reserva -----");
        Console.WriteLine($"Cliente : {r.ClienteNombre} ({r.TipoCliente}, sanciones: {r.SancionesActivas})");
        Console.WriteLine($"Puesto  : {r.PuestoCodigo} ({r.TipoPuesto})");
        Console.WriteLine($"Desde   : {r.Inicio:dd/MM/yyyy HH:mm}");
        Console.WriteLine($"Hasta   : {r.Fin:dd/MM/yyyy HH:mm}  ({r.Horas} hs)");
        Console.WriteLine();
        Console.WriteLine($"Tarifa base por hora      : {r.TarifaBasePorHora,12:N2}");
        Console.WriteLine($"Subtotal                  : {r.Subtotal,12:N2}");
        if (r.RecargoFinDeSemana > 0) Console.WriteLine($"Recargo fin de semana     : {r.RecargoFinDeSemana,12:N2}");
        if (r.DescuentoVolumen > 0)   Console.WriteLine($"Descuento por volumen     : {-r.DescuentoVolumen,12:N2}");
        if (r.DescuentoVip > 0)       Console.WriteLine($"Descuento VIP             : {-r.DescuentoVip,12:N2}");
        if (r.RecargoSancion > 0)     Console.WriteLine($"Recargo por sanciones     : {r.RecargoSancion,12:N2}");
        Console.WriteLine($"TOTAL                     : {r.Total,12:N2}");
        Console.WriteLine();
    }

    // ---------------- Opcion 2 ----------------
    private static void CancelarReserva(ReservaService servicio)
    {
        try
        {
            var reservaId = EntradaConsola.LeerEntero("Id de la reserva a cancelar: ");
            var resultado = servicio.CancelarReserva(reservaId);

            Console.WriteLine($"Reserva {resultado.ReservaId} cancelada.");
            if (resultado.SeAplicoSancion)
                Console.WriteLine($"Cancelacion con menos de 2 hs de anticipacion: se sumo una sancion al cliente (total: {resultado.SancionesActuales}).");
        }
        catch (Exception ex)
        {
            MostrarError(ex);
        }
    }

    // ---------------- Utilidades ----------------
    private static void FuncionPendiente(string nombre)
    {
        Console.WriteLine($"'{nombre}' todavia no esta implementada.");
    }

    private static void MostrarError(Exception ex)
    {
        switch (ex)
        {
            case ClienteSancionadoException:
            case PuestoNoDisponibleException:
            case EntidadNoEncontradaException:
            case ReglaNegocioException:
                Console.WriteLine($"No se pudo completar la operacion: {ex.Message}");
                break;
            default:
                Console.WriteLine($"Error inesperado: {ex.Message}");
                break;
        }
    }
}
