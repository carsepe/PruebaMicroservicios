namespace Inventario.Application.DTOs
{
    public class CompraHistoricoDto
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public DateTime FechaCompra { get; set; }
        public string Origen { get; set; }
    }
}
