using System.Globalization;

namespace FlexSpace.UI;

/// <summary>Helpers para leer datos de la consola. Solo valida el FORMATO de lo tipeado, no reglas de negocio.</summary>
public static class EntradaConsola
{
    public const string FormatoFecha = "dd/MM/yyyy HH:mm";

    public static int LeerEntero(string mensaje)
    {
        while (true)
        {
            Console.Write(mensaje);
            if (int.TryParse(Console.ReadLine(), out var valor))
                return valor;
            Console.WriteLine("  Ingrese un numero entero valido.");
        }
    }

    public static DateTime LeerFechaHora(string mensaje)
    {
        while (true)
        {
            Console.Write($"{mensaje} ({FormatoFecha}): ");
            var texto = Console.ReadLine();
            if (DateTime.TryParseExact(texto, FormatoFecha, CultureInfo.InvariantCulture, DateTimeStyles.None, out var fecha))
                return fecha;
            Console.WriteLine($"  Formato invalido. Ejemplo: {DateTime.Now:dd/MM/yyyy} 14:30");
        }
    }

    public static bool Confirmar(string mensaje)
    {
        while (true)
        {
            Console.Write($"{mensaje} (S/N): ");
            var r = Console.ReadLine()?.Trim().ToUpperInvariant();
            if (r == "S") return true;
            if (r == "N") return false;
        }
    }
}
