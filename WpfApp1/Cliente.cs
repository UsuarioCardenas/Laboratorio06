namespace WpfApp1
{
    public class Cliente
    {
        public string IdCliente { get; set; } = string.Empty;
        public string NombreCompania { get; set; } = string.Empty;
        public string? NombreContacto { get; set; }
        public string? CargoContacto { get; set; }
        public string? Direccion { get; set; }
        public string? Ciudad { get; set; }
        public string? Region { get; set; }
        public string? CodPostal { get; set; }
        public string? Pais { get; set; }
        public string? Telefono { get; set; }
        public string? Fax { get; set; }
    }
}