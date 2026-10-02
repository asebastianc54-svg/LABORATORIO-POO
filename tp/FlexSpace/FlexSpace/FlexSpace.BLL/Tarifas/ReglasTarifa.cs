using FlexSpace.DAL.Entities;

namespace FlexSpace.BLL.Tarifas;

/// <summary>Paso 1: Subtotal = horas x tarifa base.</summary>
public class ReglaSubtotalBase : IReglaTarifa
{
    public void Aplicar(ContextoTarifa ctx, DesgloseTarifa d)
    {
        d.Subtotal = ctx.Horas * ctx.TarifaBasePorHora;
    }
}

/// <summary>Paso 2: +15% sobre el subtotal si el rango toca sabado o domingo.</summary>
public class ReglaRecargoFinDeSemana : IReglaTarifa
{
    public void Aplicar(ContextoTarifa ctx, DesgloseTarifa d)
    {
        if (ctx.IncluyeFinDeSemana)
            d.RecargoFinDeSemana = d.Subtotal * ReglasNegocio.RecargoFinDeSemana;
    }
}

/// <summary>Paso 3: -10% sobre el total acumulado si dura 5 horas o mas.</summary>
public class ReglaDescuentoVolumen : IReglaTarifa
{
    public void Aplicar(ContextoTarifa ctx, DesgloseTarifa d)
    {
        if (ctx.Horas >= ReglasNegocio.HorasParaDescuentoVolumen)
            d.DescuentoVolumen = d.TotalAcumulado * ReglasNegocio.DescuentoVolumen;
    }
}

/// <summary>Paso 4: -5% extra final para clientes VIP.</summary>
public class ReglaDescuentoVip : IReglaTarifa
{
    public void Aplicar(ContextoTarifa ctx, DesgloseTarifa d)
    {
        if (ctx.TipoCliente == TipoCliente.VIP)
            d.DescuentoVip = d.TotalAcumulado * ReglasNegocio.DescuentoVip;
    }
}

/// <summary>
/// Paso 5: con sanciones se pierden TODOS los descuentos previos (volumen y VIP)
/// y se agrega un 20% sobre la tarifa base (el subtotal).
/// </summary>
public class ReglaPenalizacionPorSanciones : IReglaTarifa
{
    public void Aplicar(ContextoTarifa ctx, DesgloseTarifa d)
    {
        if (ctx.SancionesActivas <= 0) return;

        d.DescuentoVolumen = 0m;
        d.DescuentoVip = 0m;
        d.RecargoSancion = d.Subtotal * ReglasNegocio.RecargoPorSanciones;
    }
}
