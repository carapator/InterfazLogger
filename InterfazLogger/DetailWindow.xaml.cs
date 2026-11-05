using System.Windows;
using System.Windows.Input;

namespace InterfazLogger;

public partial class DetailWindow : Window
{
    public DetailWindow(Habito habito)
    {
        InitializeComponent();

        Owner = Application.Current.MainWindow;

        txtId.Text = habito.Id.ToString();
        txtTipo.Text = habito.Tipo;
        txtFecha.Text = habito.Fecha.ToString("dd/MM/yyyy");
        txtCantidad.Text = habito.Cantidad.ToString();
    }

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            Close();
        }
    }
}