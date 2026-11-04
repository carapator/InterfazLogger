using Microsoft.Data.Sqlite;
using System.Windows;

namespace InterfazLogger;

public static class BaseDatos
{
    private static readonly string cadenaConexion = $"Data Source={Configuracion.RutaBaseDatos}";

    public static SqliteConnection CrearConexion()
    {
        var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();
        return conexion;
    }

    public static bool Inicializar()
    {
        try
        {
            using var conexion = CrearConexion();
            using var comando = conexion.CreateCommand();

            comando.CommandText = """
                CREATE TABLE IF NOT EXISTS habitos (
                    Id       INTEGER PRIMARY KEY AUTOINCREMENT,
                    Tipo     TEXT    NOT NULL,
                    Fecha    TEXT    NOT NULL,
                    Cantidad INTEGER NOT NULL
                );
                CREATE TABLE IF NOT EXISTS errores (
                    Id      INTEGER PRIMARY KEY AUTOINCREMENT,
                    Fecha   TEXT NOT NULL,
                    Origen  TEXT NOT NULL,
                    Mensaje TEXT NOT NULL
                );
                """;
            comando.ExecuteNonQuery();

            return true;
        }
        catch (Exception ex)
        {
            ManejarError("BaseDatos.Inicializar", ex);
            return false;
        }
    }

    // Devuelve las filas afectadas, o -1 si hubo un error
    public static int EjecutarComando(string sql, params (string Nombre, object Valor)[] parametros)
    {
        try
        {
            using var conexion = CrearConexion();
            using var comando = conexion.CreateCommand();

            comando.CommandText = sql;
            AgregarParametros(comando, parametros);

            return comando.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            ManejarError("BaseDatos.EjecutarComando", ex);
            return -1;
        }
    }

    // "convertir" transforma cada fila en un objeto T. Si hay error devuelve una lista vacía
    public static List<T> EjecutarConsulta<T>(string sql, Func<SqliteDataReader, T> convertir,
                                              params (string Nombre, object Valor)[] parametros)
    {
        var resultado = new List<T>();

        try
        {
            using var conexion = CrearConexion();
            using var comando = conexion.CreateCommand();

            comando.CommandText = sql;
            AgregarParametros(comando, parametros);

            using var lector = comando.ExecuteReader();
            while (lector.Read())
            {
                resultado.Add(convertir(lector));
            }
        }
        catch (Exception ex)
        {
            ManejarError("BaseDatos.EjecutarConsulta", ex);
        }

        return resultado;
    }

    private static void AgregarParametros(SqliteCommand comando, (string Nombre, object Valor)[] parametros)
    {
        foreach (var (nombre, valor) in parametros)
        {
            comando.Parameters.AddWithValue(nombre, valor);
        }
    }

    private static void ManejarError(string origen, Exception ex)
    {
        RegistroErrores.Registrar(origen, ex);
        // En WPF usamos MessageBox en lugar de Consola
        System.Windows.MessageBox.Show($"Error con la base de datos (los detalles están en la tabla 'errores').\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}