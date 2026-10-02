using System.Globalization;

namespace FlexSpace.DAL.Database;

/// <summary>SQLite no tiene tipo fecha: se guarda como texto ISO, que ademas ordena bien al comparar.</summary>
internal static class FormatoFecha
{
    private const string Formato = "yyyy-MM-dd HH:mm:ss";

    public static string Guardar(DateTime fecha) => fecha.ToString(Formato, CultureInfo.InvariantCulture);

    public static DateTime Leer(string texto) =>
        DateTime.ParseExact(texto, Formato, CultureInfo.InvariantCulture);
}
