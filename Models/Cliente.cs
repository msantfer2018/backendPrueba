namespace ClientesApi.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        public string? nombre { get; set; }
        public string? telefono { get; set; }
		public string? pais { get; set; }
    }
}