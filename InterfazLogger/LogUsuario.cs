using System.IO;

namespace InterfazLogger;

public static class LogUsuario
{
    public static void Registrar(string mensaje)
    {
        try
        {
            Directory.CreateDirectory(Configuracion.CarpetaLogs);

            string nombreFichero = $"{DateTime.Now:dd_MM_yyyy}.log";
            string ruta = Path.Combine(Configuracion.CarpetaLogs, nombreFichero);
            string linea = $"[{DateTime.Now:HH:mm:ss}] {mensaje}{Environment.NewLine}";

            File.AppendAllText(ruta, linea);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"No se pudo escribir en el log: {ex.Message}");
        }
    }
}