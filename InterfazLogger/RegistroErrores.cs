using System.Globalization;

namespace InterfazLogger;

public static class RegistroErrores
{
    public static void Registrar(string origen, Exception ex)
    {
        // No se usa BaseDatos.EjecutarComando: si fallara, llamaría de nuevo a este
        // método y se produciría un bucle infinito
        try
        {
            using var conexion = BaseDatos.CrearConexion();
            using var comando = conexion.CreateCommand();

            comando.CommandText = "INSERT INTO errores (Fecha, Origen, Mensaje) VALUES (@fecha, @origen, @mensaje)";
            comando.Parameters.AddWithValue("@fecha", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
            comando.Parameters.AddWithValue("@origen", origen);
            comando.Parameters.AddWithValue("@mensaje", $"{ex.GetType().Name}: {ex.Message}");

            comando.ExecuteNonQuery();
        }
        catch (Exception)
        {
            System.Diagnostics.Debug.WriteLine($"[No se pudo guardar el error en la BD] {origen}: {ex.Message}");
        }

        LogUsuario.Registrar($"ERROR en {origen}: {ex.Message}");
    }
}