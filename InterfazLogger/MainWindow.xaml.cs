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

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly ObservableCollection<Habito> _registros = new();

    public MainWindow()
    {
        InitializeComponent();

        // Inicializar base de datos al cargar
        Loaded += MainWindow_Loaded;

        // Asignar la lista observable al ListView
        lvRegistros.ItemsSource = _registros;

        // Establecer fecha por defecto a hoy
        dpFecha.SelectedDate = DateTime.Today;
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            // Inicializar base de datos
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
            // Validar tipo de hábito
            string tipo = txtTipoHabito.Text.Trim();
            if (string.IsNullOrWhiteSpace(tipo))
            {
                MessageBox.Show("Debe escribir un tipo de hábito.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtTipoHabito.Focus();
                return;
            }

            // Validar fecha
            if (dpFecha.SelectedDate == null)
            {
                MessageBox.Show("Debe seleccionar una fecha.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                dpFecha.Focus();
                return;
            }
            DateTime fecha = dpFecha.SelectedDate.Value;

            // Validar cantidad
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

            // Insertar en base de datos
            bool correcto = HabitoRepositorio.Insertar(tipo, fecha, cantidad);

            if (correcto)
            {
                string mensaje = $"Registro insertado: {tipo} - {fecha:dd/MM/yyyy} - {cantidad}";
                LogUsuario.Registrar(mensaje);

                // Limpiar formulario
                txtTipoHabito.Clear();
                txtCantidad.Clear();
                dpFecha.SelectedDate = DateTime.Today;
                txtTipoHabito.Focus();

                // Recargar registros
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

    // Solo permitir números en el TextBox de cantidad
    private void txtCantidad_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        Regex regex = new Regex("[^0-9]+");
        e.Handled = regex.IsMatch(e.Text);
    }

    protected override void OnClosed(EventArgs e)
    {
        LogUsuario.Registrar("Aplicación cerrada");
        base.OnClosed(e);
    }
}