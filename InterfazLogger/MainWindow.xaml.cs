using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace InterfazLogger;

public partial class MainWindow : Window
{
    private readonly ObservableCollection<Habito> _registros = new();

    public MainWindow()
    {
        InitializeComponent();

        Loaded += MainWindow_Loaded;

        lvRegistros.ItemsSource = _registros;

        dpFecha.SelectedDate = DateTime.Today;
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            if (BaseDatos.Inicializar())
            {
                LogUsuario.Registrar("Aplicación iniciada");
                CargarRegistros();
            }
            else
            {
                MessageBox.Show("No se pudo iniciar la base de datos. La aplicación se cerrará.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }
        catch (Exception ex)
        {
            RegistroErrores.Registrar("MainWindow.Loaded", ex);
            MessageBox.Show("Error inesperado al iniciar la aplicación.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Application.Current.Shutdown();
        }
    }

    private void CargarRegistros()
    {
        try
        {
            var registros = HabitoRepositorio.ObtenerTodos();
            _registros.Clear();
            foreach (var registro in registros)
            {
                _registros.Add(registro);
            }
            LogUsuario.Registrar("Consulta de registros");
        }
        catch (Exception ex)
        {
            RegistroErrores.Registrar("MainWindow.CargarRegistros", ex);
            MessageBox.Show("Error al cargar los registros.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void btnInsertar_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            string tipo = txtTipoHabito.Text.Trim();
            if (string.IsNullOrWhiteSpace(tipo))
            {
                MessageBox.Show("Debe escribir un tipo de hábito.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtTipoHabito.Focus();
                return;
            }

            if (dpFecha.SelectedDate == null)
            {
                MessageBox.Show("Debe seleccionar una fecha.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                dpFecha.Focus();
                return;
            }
            DateTime fecha = dpFecha.SelectedDate.Value;

            if (!int.TryParse(txtCantidad.Text.Trim(), out int cantidad))
            {
                MessageBox.Show("La cantidad debe ser un número.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtCantidad.Focus();
                return;
            }

            if (cantidad < Configuracion.CantidadMinima || cantidad > Configuracion.CantidadMaxima)
            {
                MessageBox.Show($"La cantidad debe estar entre {Configuracion.CantidadMinima} y {Configuracion.CantidadMaxima}.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtCantidad.Focus();
                return;
            }

            bool correcto = HabitoRepositorio.Insertar(tipo, fecha, cantidad);

            if (correcto)
            {
                string mensaje = $"Registro insertado: {tipo} - {fecha:dd/MM/yyyy} - {cantidad}";
                LogUsuario.Registrar(mensaje);

                txtTipoHabito.Clear();
                txtCantidad.Clear();
                dpFecha.SelectedDate = DateTime.Today;
                txtTipoHabito.Focus();

                CargarRegistros();
            }
            else
            {
                MessageBox.Show("No se pudo insertar el registro.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        catch (Exception ex)
        {
            RegistroErrores.Registrar("MainWindow.btnInsertar_Click", ex);
            MessageBox.Show("Ha ocurrido un error inesperado.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void txtCantidad_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        Regex regex = new Regex("[^0-9]+");
        e.Handled = regex.IsMatch(e.Text);
    }

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            Close();
        }
    }

    private void lvRegistros_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (lvRegistros.SelectedItem is Habito habito)
        {
            var detailWindow = new DetailWindow(habito);
            detailWindow.Show();
        }
    }

    protected override void OnClosed(EventArgs e)
    {
        LogUsuario.Registrar("Aplicación cerrada");
        base.OnClosed(e);
    }
}