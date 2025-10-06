using Microsoft.AspNetCore.Mvc;
using MiniOrdersApi.Models;


namespace MiniOrdersApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private static readonly List<Order> orders = new List<Order>
        {
            //new Order { Id = Guid.NewGuid(), Cliente = "Juan Pérez", Fecha = DateTime.Now.AddDays(-1), Cantidad = 1, Total = 1199.99M },
            //new Order { Id = Guid.NewGuid(), Cliente = "María López", Fecha = DateTime.Now, Cantidad = 1, Total = 849.99M }
        };

        [HttpGet]
        public IActionResult GetOrders() => Ok(orders);

        [HttpGet("{id}")]
        public IActionResult GetOrder(Guid id)
        {
            var order = orders.FirstOrDefault(o => o.Id == id);
            return order == null ? NotFound() : Ok(order);
        }

        [HttpPost]
        public IActionResult CreateOrder([FromBody] Order newOrder)
        {
            newOrder.Id = Guid.NewGuid();
            orders.Add(newOrder);
            return CreatedAtAction(nameof(GetOrder), new { id = newOrder.Id }, newOrder);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateOrder(Guid id, [FromBody] Order updatedOrder)
        {
            var order = orders.FirstOrDefault(o => o.Id == id);
            if (order == null) return NotFound();

            order.Cliente = updatedOrder.Cliente;
            order.Fecha = updatedOrder.Fecha;
            order.Total = updatedOrder.Total;

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteOrder(Guid id)
        {
            var order = orders.FirstOrDefault(o => o.Id == id);
            if (order == null) return NotFound();

            orders.Remove(order);
            return NoContent();
        }
    }
}
