using System.IO;

namespace InterfazLogger;

public static class Configuracion
{
    public const int CantidadMinima = 1;
    public const int CantidadMaxima = 100;

    // En la BD se guarda como yyyy-MM-dd para que SQLite ordene bien por fecha
    public const string FormatoFechaBD = "yyyy-MM-dd";
    public const string FormatoFechaPantalla = "dd/MM/yyyy";

    public static readonly string RutaBaseDatos = Path.Combine(AppContext.BaseDirectory, "habitos.db");
    public static readonly string CarpetaLogs = Path.Combine(AppContext.BaseDirectory, "logs");
}