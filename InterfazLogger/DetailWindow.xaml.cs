using System.Windows;
using System.Windows.Input;

namespace InterfazLogger;

/// <summary>
/// Ventana para mostrar los detalles de un hábito
/// </summary>
public partial class DetailWindow : Window
{
    public DetailWindow(Habito habito)
    {
        InitializeComponent();

        // Establecer el Owner para que se centre en la ventana principal
        Owner = Application.Current.MainWindow;

        // Cargar los datos del hábito
        txtId.Text = habito.Id.ToString();
        txtTipo.Text = habito.Tipo;
        txtFecha.Text = habito.Fecha.ToString("dd/MM/yyyy");
        txtCantidad.Text = habito.Cantidad.ToString();
    }

    // Manejar tecla Escape para cerrar la ventana
    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            Close();
        }
    }
}