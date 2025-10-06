namespace MiniOrdersApi.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public required string Cliente { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Cantidad { get; set; } = 1;
        public decimal Total { get; set; }
    }
}
