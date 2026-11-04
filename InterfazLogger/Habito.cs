using System;

namespace InterfazLogger;

public record Habito(int Id, string Tipo, DateTime Fecha, int Cantidad)
{
    public override string ToString()
    {
        return $"{Tipo} - {Fecha:dd/MM/yyyy} - {Cantidad}";
    }
}