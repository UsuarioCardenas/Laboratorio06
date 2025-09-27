using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _esActualizacion = false;

        public MainWindow()
        {
            InitializeComponent();
            
            // Probar conexión primero
            if (!ConexionDatos.ProbarConexion())
            {
                MessageBox.Show("Error: No se pudo conectar a la base de datos. Verifique la conexión.", 
                    "Error de Conexión", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                // Probar stored procedure
                ConexionDatos.ProbarStoredProcedure();
            }
            
            LimpiarFormulario(); // Inicializar el formulario correctamente
            CargarClientes();
        }

        private void CargarClientes()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== INICIANDO CargarClientes en MainWindow ===");
                var clientes = ConexionDatos.ListarClientes();
                System.Diagnostics.Debug.WriteLine($"Clientes obtenidos: {clientes.Count}");
                
                dgClientes.ItemsSource = null; // Limpiar primero
                dgClientes.ItemsSource = clientes;
                
                System.Diagnostics.Debug.WriteLine("DataGrid actualizado");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR en CargarClientes: {ex.Message}");
                MessageBox.Show($"Error al cargar clientes: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnInsertar_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarDatosInsercion()) return;

            try
            {
                var cliente = new Cliente
                {
                    // No asignamos IdCliente aquí - se genera automáticamente
                    NombreCompania = txtNombreCompania.Text.Trim(),
                    NombreContacto = string.IsNullOrWhiteSpace(txtNombreContacto.Text) ? null : txtNombreContacto.Text.Trim(),
                    CargoContacto = string.IsNullOrWhiteSpace(txtCargoContacto.Text) ? null : txtCargoContacto.Text.Trim(),
                    Direccion = string.IsNullOrWhiteSpace(txtDireccion.Text) ? null : txtDireccion.Text.Trim(),
                    Ciudad = string.IsNullOrWhiteSpace(txtCiudad.Text) ? null : txtCiudad.Text.Trim(),
                    Region = string.IsNullOrWhiteSpace(txtRegion.Text) ? null : txtRegion.Text.Trim(),
                    CodPostal = string.IsNullOrWhiteSpace(txtCodPostal.Text) ? null : txtCodPostal.Text.Trim(),
                    Pais = string.IsNullOrWhiteSpace(txtPais.Text) ? null : txtPais.Text.Trim(),
                    Telefono = string.IsNullOrWhiteSpace(txtTelefono.Text) ? null : txtTelefono.Text.Trim(),
                    Fax = null
                };

                if (ConexionDatos.InsertarCliente(cliente))
                {
                    MessageBox.Show($"Cliente insertado correctamente con ID: {cliente.IdCliente}", "Éxito", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    // Mostrar el ID generado en el campo
                    txtIdCliente.Text = cliente.IdCliente;
                    
                    CargarClientes();
                }
                else
                {
                    MessageBox.Show("Error al insertar el cliente.", "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al insertar cliente: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnActualizar_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarDatos()) return;

            if (string.IsNullOrWhiteSpace(txtIdCliente.Text))
            {
                MessageBox.Show("Debe seleccionar un cliente para actualizar", "Advertencia", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var cliente = new Cliente
                {
                    IdCliente = txtIdCliente.Text.Trim(),
                    NombreCompania = txtNombreCompania.Text.Trim(),
                    NombreContacto = string.IsNullOrWhiteSpace(txtNombreContacto.Text) ? null : txtNombreContacto.Text.Trim(),
                    CargoContacto = string.IsNullOrWhiteSpace(txtCargoContacto.Text) ? null : txtCargoContacto.Text.Trim(),
                    Direccion = string.IsNullOrWhiteSpace(txtDireccion.Text) ? null : txtDireccion.Text.Trim(),
                    Ciudad = string.IsNullOrWhiteSpace(txtCiudad.Text) ? null : txtCiudad.Text.Trim(),
                    Region = string.IsNullOrWhiteSpace(txtRegion.Text) ? null : txtRegion.Text.Trim(),
                    CodPostal = string.IsNullOrWhiteSpace(txtCodPostal.Text) ? null : txtCodPostal.Text.Trim(),
                    Pais = string.IsNullOrWhiteSpace(txtPais.Text) ? null : txtPais.Text.Trim(),
                    Telefono = string.IsNullOrWhiteSpace(txtTelefono.Text) ? null : txtTelefono.Text.Trim(),
                    Fax = null
                };

                if (ConexionDatos.ActualizarCliente(cliente))
                {
                    MessageBox.Show("Cliente actualizado correctamente", "Éxito", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    LimpiarFormulario();
                    CargarClientes();
                    _esActualizacion = false;
                }
                else
                {
                    MessageBox.Show("Error al actualizar el cliente", "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar cliente: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtIdCliente.Text))
            {
                MessageBox.Show("Debe seleccionar un cliente para eliminar", "Advertencia", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var resultado = MessageBox.Show(
                $"¿Está seguro de eliminar el cliente {txtIdCliente.Text}?", 
                "Confirmar eliminación", 
                MessageBoxButton.YesNo, 
                MessageBoxImage.Question);

            if (resultado == MessageBoxResult.Yes)
            {
                try
                {
                    if (ConexionDatos.EliminarCliente(txtIdCliente.Text.Trim()))
                    {
                        MessageBox.Show("Cliente eliminado correctamente", "Éxito", 
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        LimpiarFormulario();
                        CargarClientes();
                    }
                    else
                    {
                        MessageBox.Show("Error al eliminar el cliente", "Error", 
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al eliminar cliente: {ex.Message}", "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            LimpiarFormulario();
        }

        private void BtnDepurar_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Iniciando depuración... Revisa la ventana de Output/Debug en Visual Studio", 
                "Debug", MessageBoxButton.OK, MessageBoxImage.Information);
            
            // Verificar estructura de la tabla primero
            ConexionDatos.VerificarEstructuraTabla();
            
            ConexionDatos.ProbarConexion();
            ConexionDatos.ProbarStoredProcedure();
            
            // Probar inserción simple primero
            ConexionDatos.ProbarInsercionSimple();
            
            // Luego la inserción completa
            ConexionDatos.ProbarInsercion();
            
            var clientes = ConexionDatos.ListarClientes();
            MessageBox.Show($"Clientes encontrados: {clientes.Count}\n" +
                           $"Revisa la ventana de Debug para más detalles", 
                "Resultado Debug", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnCargar_Click(object sender, RoutedEventArgs e)
        {
            CargarClientes();
        }

        private void DgClientes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgClientes.SelectedItem is Cliente cliente)
            {
                // Habilitar el campo ID para actualización/eliminación
                txtIdCliente.IsReadOnly = false;
                txtIdCliente.Background = System.Windows.Media.Brushes.White;
                
                txtIdCliente.Text = cliente.IdCliente;
                txtNombreCompania.Text = cliente.NombreCompania;
                txtNombreContacto.Text = cliente.NombreContacto ?? "";
                txtCargoContacto.Text = cliente.CargoContacto ?? "";
                txtDireccion.Text = cliente.Direccion ?? "";
                txtCiudad.Text = cliente.Ciudad ?? "";
                txtRegion.Text = cliente.Region ?? "";
                txtCodPostal.Text = cliente.CodPostal ?? "";
                txtPais.Text = cliente.Pais ?? "";
                txtTelefono.Text = cliente.Telefono ?? "";
                
                _esActualizacion = true;
            }
        }

        private bool ValidarDatos()
        {
            if (string.IsNullOrWhiteSpace(txtIdCliente.Text))
            {
                MessageBox.Show("Debe seleccionar un cliente para actualizar/eliminar", "Validación", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtNombreCompania.Text))
            {
                MessageBox.Show("El nombre de la compañía es obligatorio", "Validación", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtNombreCompania.Focus();
                return false;
            }

            return true;
        }

        private bool ValidarDatosInsercion()
        {
            if (string.IsNullOrWhiteSpace(txtNombreCompania.Text))
            {
                MessageBox.Show("El nombre de la compañía es obligatorio", "Validación", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtNombreCompania.Focus();
                return false;
            }

            return true;
        }

        private void LimpiarFormulario()
        {
            // Deshabilitar el campo ID para inserción
            txtIdCliente.IsReadOnly = true;
            txtIdCliente.Background = System.Windows.Media.Brushes.LightGray;
            
            txtIdCliente.Text = "[Se genera automáticamente]";
            txtNombreCompania.Text = "";
            txtNombreContacto.Text = "";
            txtCargoContacto.Text = "";
            txtDireccion.Text = "";
            txtCiudad.Text = "";
            txtRegion.Text = "";
            txtCodPostal.Text = "";
            txtPais.Text = "";
            txtTelefono.Text = "";
            
            dgClientes.SelectedItem = null;
            _esActualizacion = false;
            txtNombreCompania.Focus(); // Cambiar foco al primer campo requerido
        }
    }
}