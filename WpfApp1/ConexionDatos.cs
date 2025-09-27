using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq; // Agregado: usando para LINQ

namespace WpfApp1
{
    public class ConexionDatos
    {
        private const string _connectionString =
            @"Server=localhost\SQLSERVERFIRST;Database=Neptuno;User Id=userTecsup;Password=123456;TrustServerCertificate=True;";

        // Generar un ID único para el cliente
        private static string GenerarIdCliente()
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            
            string nuevoId;
            int intentos = 0;
            const int maxIntentos = 100;
            
            do
            {
                intentos++;
                nuevoId = new string(Enumerable.Repeat(chars, 5)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
                
                if (intentos >= maxIntentos)
                {
                    break;
                }
            }
            while (ExisteCliente(nuevoId));
            
            return nuevoId;
        }

        // Verificar si existe un cliente con el ID dado
        private static bool ExisteCliente(string idCliente)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Verificando si existe cliente con ID: {idCliente}");
                
                using var con = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("SELECT COUNT(1) FROM dbo.Clientes WHERE idCliente = @idCliente", con);
                
                cmd.Parameters.Add(new SqlParameter("@idCliente", SqlDbType.VarChar, 5) { Value = idCliente });
                
                con.Open();
                int count = (int)cmd.ExecuteScalar();
                con.Close();
                
                System.Diagnostics.Debug.WriteLine($"Cliente {idCliente} existe: {count > 0} (count: {count})");
                return count > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR en ExisteCliente: {ex.Message}");
                return true; // En caso de error, asumir que existe para evitar duplicados
            }
        }

        // Insertar Cliente
        public static bool InsertarCliente(Cliente cliente)
        {
            try
            {
                // Generar ID automáticamente si no se proporciona
                if (string.IsNullOrWhiteSpace(cliente.IdCliente))
                {
                    cliente.IdCliente = GenerarIdCliente();
                }

                using var con = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_InsertarCliente", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add(new SqlParameter("@idCliente", SqlDbType.VarChar, 5) { Value = cliente.IdCliente });
                cmd.Parameters.Add(new SqlParameter("@NombreCompania", SqlDbType.VarChar, 100) { Value = cliente.NombreCompania });
                cmd.Parameters.Add(new SqlParameter("@NombreContacto", SqlDbType.VarChar, 100) { Value = (object?)cliente.NombreContacto ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@CargoContacto", SqlDbType.VarChar, 100) { Value = (object?)cliente.CargoContacto ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@Direccion", SqlDbType.VarChar, 100) { Value = (object?)cliente.Direccion ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@Ciudad", SqlDbType.VarChar, 100) { Value = (object?)cliente.Ciudad ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@Region", SqlDbType.VarChar, 100) { Value = (object?)cliente.Region ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@CodPostal", SqlDbType.VarChar, 100) { Value = (object?)cliente.CodPostal ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@Pais", SqlDbType.VarChar, 100) { Value = (object?)cliente.Pais ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@Telefono", SqlDbType.VarChar, 30) { Value = (object?)cliente.Telefono ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@Fax", SqlDbType.VarChar, 30) { Value = (object?)cliente.Fax ?? DBNull.Value });

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                // Tu SP retorna 0 en éxito, -1 en error
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR en InsertarCliente: {ex.Message}");
                return false;
            }
        }

        // Actualizar Cliente
        public static bool ActualizarCliente(Cliente cliente)
        {
            try
            {
                using var con = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_ActualizarCliente", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add(new SqlParameter("@idCliente", SqlDbType.VarChar, 5) { Value = cliente.IdCliente });
                cmd.Parameters.Add(new SqlParameter("@NombreCompania", SqlDbType.VarChar, 100) { Value = cliente.NombreCompania });
                cmd.Parameters.Add(new SqlParameter("@NombreContacto", SqlDbType.VarChar, 100) { Value = (object?)cliente.NombreContacto ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@CargoContacto", SqlDbType.VarChar, 100) { Value = (object?)cliente.CargoContacto ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@Direccion", SqlDbType.VarChar, 100) { Value = (object?)cliente.Direccion ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@Ciudad", SqlDbType.VarChar, 100) { Value = (object?)cliente.Ciudad ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@Region", SqlDbType.VarChar, 100) { Value = (object?)cliente.Region ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@CodPostal", SqlDbType.VarChar, 100) { Value = (object?)cliente.CodPostal ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@Pais", SqlDbType.VarChar, 100) { Value = (object?)cliente.Pais ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@Telefono", SqlDbType.VarChar, 30) { Value = (object?)cliente.Telefono ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@Fax", SqlDbType.VarChar, 30) { Value = (object?)cliente.Fax ?? DBNull.Value });

                con.Open();
                int result = cmd.ExecuteNonQuery();
                con.Close();
                return result > 0;
            }
            catch
            {
                return false;
            }
        }

        // Eliminar Cliente
        public static bool EliminarCliente(string idCliente)
        {
            try
            {
                using var con = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_EliminarCliente", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add(new SqlParameter("@idCliente", SqlDbType.VarChar, 5) { Value = idCliente });

                con.Open();
                int result = cmd.ExecuteNonQuery();
                con.Close();
                return result > 0;
            }
            catch
            {
                return false;
            }
        }

        // Método de prueba para verificar la conexión
        public static bool ProbarConexion()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== PROBANDO CONEXIÓN ===");
                using var con = new SqlConnection(_connectionString);
                con.Open();
                System.Diagnostics.Debug.WriteLine("Conexión exitosa");
                
                // Probar una consulta simple
                using var cmd = new SqlCommand("SELECT COUNT(*) FROM dbo.Clientes", con);
                var count = (int)cmd.ExecuteScalar();
                System.Diagnostics.Debug.WriteLine($"Número de registros en tabla Clientes: {count}");
                
                con.Close();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR de conexión: {ex.Message}");
                return false;
            }
        }

        // Método auxiliar para obtener el índice de una columna de forma segura
        private static int GetColumnIndex(SqlDataReader reader, string columnName)
        {
            try
            {
                return reader.GetOrdinal(columnName);
            }
            catch (IndexOutOfRangeException)
            {
                return -1;
            }
        }

        // Listar Clientes
        public static List<Cliente> ListarClientes(string? idCliente = null, string? ciudad = null, string? pais = null)
        {
            var clientes = new List<Cliente>();
            try
            {
                System.Diagnostics.Debug.WriteLine("=== INICIANDO ListarClientes ===");
                System.Diagnostics.Debug.WriteLine($"Parámetros: idCliente={idCliente}, ciudad={ciudad}, pais={pais}");

                using var con = new SqlConnection(_connectionString);
                System.Diagnostics.Debug.WriteLine($"Connection String: {_connectionString}");

                using var cmd = new SqlCommand("sp_ListarClientes", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add(new SqlParameter("@idCliente", SqlDbType.VarChar, 5) { Value = (object?)idCliente ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@Ciudad", SqlDbType.VarChar, 100) { Value = (object?)ciudad ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@Pais", SqlDbType.VarChar, 100) { Value = (object?)pais ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@OrdenarPor", SqlDbType.VarChar, 20) { Value = "NombreCompania" });

                System.Diagnostics.Debug.WriteLine("Abriendo conexión...");
                con.Open();
                System.Diagnostics.Debug.WriteLine("Conexión abierta exitosamente");

                System.Diagnostics.Debug.WriteLine("Ejecutando stored procedure...");
                using var reader = cmd.ExecuteReader();
                System.Diagnostics.Debug.WriteLine("Reader creado exitosamente");

                // Mostrar información sobre las columnas
                System.Diagnostics.Debug.WriteLine($"Número de columnas: {reader.FieldCount}");
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    System.Diagnostics.Debug.WriteLine($"Columna {i}: {reader.GetName(i)} ({reader.GetDataTypeName(i)})");
                }

                int contador = 0;
                while (reader.Read())
                {
                    contador++;
                    System.Diagnostics.Debug.WriteLine($"=== FILA {contador} ===");
                    
                    // Mostrar todos los valores de la fila
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        var valor = reader.IsDBNull(i) ? "NULL" : reader[i]?.ToString();
                        System.Diagnostics.Debug.WriteLine($"{reader.GetName(i)}: {valor}");
                    }

                    var cliente = new Cliente
                    {
                        IdCliente = reader["idCliente"].ToString() ?? "",
                        NombreCompania = reader["NombreCompañía"].ToString() ?? "",
                        NombreContacto = reader["NombreContacto"] == DBNull.Value ? null : reader["NombreContacto"].ToString(),
                        CargoContacto = reader["CargoContacto"] == DBNull.Value ? null : reader["CargoContacto"].ToString(),
                        Direccion = reader["Direccion"] == DBNull.Value ? null : reader["Direccion"].ToString(),
                        Ciudad = reader["Ciudad"] == DBNull.Value ? null : reader["Ciudad"].ToString(),
                        Region = reader["Region"] == DBNull.Value ? null : reader["Region"].ToString(),
                        CodPostal = reader["CodPostal"] == DBNull.Value ? null : reader["CodPostal"].ToString(),
                        Pais = reader["Pais"] == DBNull.Value ? null : reader["Pais"].ToString(),
                        Telefono = reader["Telefono"] == DBNull.Value ? null : reader["Telefono"].ToString(),
                        Fax = reader["Fax"] == DBNull.Value ? null : reader["Fax"].ToString()
                    };
                    
                    System.Diagnostics.Debug.WriteLine($"Cliente creado: ID={cliente.IdCliente}, Nombre={cliente.NombreCompania}");
                    clientes.Add(cliente);
                }
                
                System.Diagnostics.Debug.WriteLine($"Total de clientes cargados: {clientes.Count}");
                con.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR en ListarClientes: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}")
;                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
            }
            return clientes;
        }

        // Método de prueba específico para inserción
        public static void ProbarInsercion()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== PROBANDO INSERCIÓN ===");
                
                var clientePrueba = new Cliente
                {
                    NombreCompania = "Empresa Test DEBUG",
                    NombreContacto = "Juan Prueba",
                    Ciudad = "Lima",
                    Pais = "Perú"
                };

                System.Diagnostics.Debug.WriteLine("Creando cliente de prueba...");
                bool resultado = InsertarCliente(clientePrueba);
                System.Diagnostics.Debug.WriteLine($"Resultado final de inserción: {resultado}");
                
                if (resultado)
                {
                    System.Diagnostics.Debug.WriteLine($"Cliente insertado con ID: {clientePrueba.IdCliente}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("FALLÓ la inserción del cliente de prueba");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR en ProbarInsercion: {ex.Message}");
            }
        }

        // Método de prueba específico para inserción - VERSIÓN SIMPLIFICADA
        public static void ProbarInsercionSimple()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== PROBANDO INSERCIÓN SIMPLE ===");
                
                using var con = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_InsertarClienteTest", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // Solo parámetros esenciales
                var testId = "TEST1";
                cmd.Parameters.Add(new SqlParameter("@idCliente", SqlDbType.VarChar, 5) { Value = testId });
                cmd.Parameters.Add(new SqlParameter("@NombreCompania", SqlDbType.VarChar, 100) { Value = "Empresa Test Simple" });

                System.Diagnostics.Debug.WriteLine($"Insertando cliente de prueba con ID: {testId}");

                con.Open();
                int result = cmd.ExecuteNonQuery();
                con.Close();

                System.Diagnostics.Debug.WriteLine($"Resultado SP simple: {result}");
                System.Diagnostics.Debug.WriteLine($"¿Éxito? {result == 0}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR en ProbarInsercionSimple: {ex.Message}");
            }
        }

        // Método de prueba para verificar el stored procedure
        public static void ProbarStoredProcedure()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== PROBANDO STORED PROCEDURE ===");
                using var con = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_ListarClientes", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // Agregar parámetros con valores por defecto
                cmd.Parameters.Add(new SqlParameter("@idCliente", SqlDbType.VarChar, 5) { Value = DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@Ciudad", SqlDbType.VarChar, 100) { Value = DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@Pais", SqlDbType.VarChar, 100) { Value = DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@OrdenarPor", SqlDbType.VarChar, 20) { Value = "NombreCompania" });

                con.Open();
                using var reader = cmd.ExecuteReader();
                
                System.Diagnostics.Debug.WriteLine($"¿Tiene filas? {reader.HasRows}");
                
                if (reader.HasRows)
                {
                    int filaCount = 0;
                    while (reader.Read() && filaCount < 3) // Solo mostrar las primeras 3 filas
                    {
                        filaCount++;
                        System.Diagnostics.Debug.WriteLine($"Fila {filaCount}:");
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            System.Diagnostics.Debug.WriteLine($"  {reader.GetName(i)}: {(reader.IsDBNull(i) ? "NULL" : reader[i])}");
                        }
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("El stored procedure no devuelve filas");
                }
                
                con.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR en ProbarStoredProcedure: {ex.Message}");
            }
        }

        // Método para verificar la estructura de la tabla
        public static void VerificarEstructuraTabla()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== VERIFICANDO ESTRUCTURA DE TABLA ===");
                
                using var con = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE
                    FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_NAME = 'Clientes' AND TABLE_SCHEMA = 'dbo'
                    ORDER BY ORDINAL_POSITION", con);

                con.Open();
                using var reader = cmd.ExecuteReader();
                
                System.Diagnostics.Debug.WriteLine("Columnas de la tabla Clientes:");
                while (reader.Read())
                {
                    System.Diagnostics.Debug.WriteLine($"  {reader["COLUMN_NAME"]} | {reader["DATA_TYPE"]} | Max:{reader["CHARACTER_MAXIMUM_LENGTH"]} | Nullable:{reader["IS_NULLABLE"]}");
                }
                
                con.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR en VerificarEstructuraTabla: {ex.Message}");
            }
        }
    }
}