using FlexSpace.BLL.Dtos;
using FlexSpace.BLL.Exceptions;
using FlexSpace.BLL.Tarifas;
using FlexSpace.DAL.Entities;
using FlexSpace.DAL.Repositories;

namespace FlexSpace.BLL.Servicios;

public class ReservaService
{
    private readonly IClienteRepository _clientes;
    private readonly IPuestoRepository _puestos;
    private readonly IReservaRepository _reservas;
    private readonly CalculadorTarifa _calculador;
    private readonly Func<DateTime> _ahora;

    public ReservaService(
        IClienteRepository clientes,
        IPuestoRepository puestos,
        IReservaRepository reservas,
        CalculadorTarifa calculador,
        Func<DateTime>? ahora = null)
    {
        _clientes = clientes;
        _puestos = puestos;
        _reservas = reservas;
        _calculador = calculador;
        _ahora = ahora ?? (() => DateTime.Now); // inyectable para poder testear con otra "hora actual"
    }

    /// <summary>Valida todo y calcula el precio, pero NO guarda nada. Es lo que se muestra antes de confirmar.</summary>
    public ResumenReservaDto SimularReserva(int clienteId, int puestoId, DateTime inicio, DateTime fin)
    {
        return Preparar(clienteId, puestoId, inicio, fin).Resumen;
    }

    /// <summary>Vuelve a validar (por si cambio algo desde la simulacion) y persiste la reserva. Devuelve su Id.</summary>
    public int RegistrarReserva(int clienteId, int puestoId, DateTime inicio, DateTime fin)
    {
        var preparada = Preparar(clienteId, puestoId, inicio, fin);

        var reserva = new Reserva
        {
            ClienteId = clienteId,
            PuestoId = puestoId,
            FechaInicio = inicio,
            FechaFin = fin,
            Estado = EstadoReserva.Confirmada,
            CostoTotal = preparada.Resumen.Total
        };
        return _reservas.Insertar(reserva);
    }

    /// <summary>
    /// Cancela una reserva Confirmada. Si faltan menos de 2 horas para el inicio,
    /// se suma +1 sancion al cliente (en la misma transaccion).
    /// </summary>
    public CancelacionResultadoDto CancelarReserva(int reservaId)
    {
        var reserva = _reservas.ObtenerPorId(reservaId)
                      ?? throw new EntidadNoEncontradaException("Reserva", reservaId);

        if (reserva.Estado != EstadoReserva.Confirmada)
            throw new ReglaNegocioException($"Solo se pueden cancelar reservas Confirmadas (esta esta {reserva.Estado}).");

        var ahora = _ahora();
        if (reserva.FechaInicio <= ahora)
            throw new ReglaNegocioException("La reserva ya comenzo, no se puede cancelar.");

        var anticipacion = reserva.FechaInicio - ahora;
        var penalizar = anticipacion < TimeSpan.FromHours(ReglasNegocio.HorasMinimasParaCancelarSinSancion);

        _reservas.Cancelar(reservaId, penalizar ? reserva.ClienteId : null);

        var cliente = _clientes.ObtenerPorId(reserva.ClienteId)!;
        return new CancelacionResultadoDto
        {
            ReservaId = reservaId,
            SeAplicoSancion = penalizar,
            SancionesActuales = cliente.SancionesActivas
        };
    }

    // ------------------------------------------------------------------

    private (Cliente Cliente, Puesto Puesto, ResumenReservaDto Resumen) Preparar(
        int clienteId, int puestoId, DateTime inicio, DateTime fin)
    {
        ValidarRangoFechas(inicio, fin);

        var cliente = _clientes.ObtenerPorId(clienteId)
                      ?? throw new EntidadNoEncontradaException("Cliente", clienteId);

        // Regla C: bloqueo de cliente
        if (cliente.SancionesActivas >= ReglasNegocio.SancionesParaBloqueo)
            throw new ClienteSancionadoException(cliente.Nombre, cliente.SancionesActivas);

        var puesto = _puestos.ObtenerPorId(puestoId)
                     ?? throw new EntidadNoEncontradaException("Puesto", puestoId);

        // Regla A: sin solapamiento
        if (_reservas.ObtenerConfirmadasSolapadas(puestoId, inicio, fin).Count > 0)
            throw new PuestoNoDisponibleException(puesto.Codigo, inicio, fin);

        // Regla B: tarifa dinamica
        var contexto = new ContextoTarifa
        {
            TarifaBasePorHora = puesto.TarifaBasePorHora,
            Inicio = inicio,
            Fin = fin,
            TipoCliente = cliente.TipoCliente,
            SancionesActivas = cliente.SancionesActivas
        };
        var d = _calculador.Calcular(contexto);

        var resumen = new ResumenReservaDto
        {
            ClienteNombre = cliente.Nombre,
            TipoCliente = cliente.TipoCliente.ToString(),
            SancionesActivas = cliente.SancionesActivas,
            PuestoCodigo = puesto.Codigo,
            TipoPuesto = puesto.TipoPuesto.ToString(),
            Inicio = inicio,
            Fin = fin,
            Horas = Math.Round(contexto.Horas, 2),
            TarifaBasePorHora = puesto.TarifaBasePorHora,
            Subtotal = CalculadorTarifa.Redondear(d.Subtotal),
            RecargoFinDeSemana = CalculadorTarifa.Redondear(d.RecargoFinDeSemana),
            DescuentoVolumen = CalculadorTarifa.Redondear(d.DescuentoVolumen),
            DescuentoVip = CalculadorTarifa.Redondear(d.DescuentoVip),
            RecargoSancion = CalculadorTarifa.Redondear(d.RecargoSancion),
            Total = CalculadorTarifa.Redondear(d.TotalAcumulado)
        };

        return (cliente, puesto, resumen);
    }

    private void ValidarRangoFechas(DateTime inicio, DateTime fin)
    {
        if (fin <= inicio)
            throw new ReglaNegocioException("La fecha de fin debe ser posterior a la fecha de inicio.");

        if (inicio < _ahora())
            throw new ReglaNegocioException("No se pueden crear reservas en el pasado.");
    }
}
