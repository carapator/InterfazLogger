using System.Globalization;
using Microsoft.Data.Sqlite;

namespace InterfazLogger;

public static class HabitoRepositorio
{
    public static bool Insertar(string tipo, DateTime fecha, int cantidad)
    {
        int filas = BaseDatos.EjecutarComando(
            "INSERT INTO habitos (Tipo, Fecha, Cantidad) VALUES (@tipo, @fecha, @cantidad)",
            ("@tipo", tipo),
            ("@fecha", FechaATexto(fecha)),
            ("@cantidad", cantidad));

        return filas > 0;
    }

    public static List<Habito> ObtenerTodos()
    {
        return BaseDatos.EjecutarConsulta(
            "SELECT Id, Tipo, Fecha, Cantidad FROM habitos ORDER BY Fecha DESC, Id DESC",
            ConvertirAHabito);
    }

    public static Habito? ObtenerPorId(int id)
    {
        var resultado = BaseDatos.EjecutarConsulta(
            "SELECT Id, Tipo, Fecha, Cantidad FROM habitos WHERE Id = @id",
            ConvertirAHabito,
            ("@id", id));

        return resultado.FirstOrDefault();
    }

    private static string FechaATexto(DateTime fecha)
    {
        return fecha.ToString(Configuracion.FormatoFechaBD, CultureInfo.InvariantCulture);
    }

    private static Habito ConvertirAHabito(SqliteDataReader lector)
    {
        int id = lector.GetInt32(0);
        string tipo = lector.GetString(1);
        DateTime fecha = DateTime.ParseExact(lector.GetString(2), Configuracion.FormatoFechaBD, CultureInfo.InvariantCulture);
        int cantidad = lector.GetInt32(3);

        return new Habito(id, tipo, fecha, cantidad);
    }
}