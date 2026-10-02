namespace FlexSpace.BLL;

/// <summary>Todos los numeros de las reglas en un solo lugar, para no tener "numeros magicos" por el codigo.</summary>
public static class ReglasNegocio
{
    public const decimal RecargoFinDeSemana = 0.15m;
    public const decimal DescuentoVolumen = 0.10m;
    public const decimal HorasParaDescuentoVolumen = 5m;
    public const decimal DescuentoVip = 0.05m;
    public const decimal RecargoPorSanciones = 0.20m;

    public const int SancionesParaBloqueo = 3;
    public const int HorasMinimasParaCancelarSinSancion = 2;
}
