using AutoMapper;
using backend.Data.Repository;
using backend.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/order")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly IOrderRepository orderRepository;
    private readonly IMapper mapper;

    public OrderController(IOrderRepository orderRepository, IMapper mapper)
    {
        this.orderRepository = orderRepository;
        this.mapper = mapper;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllOrders()
    {
        var orders = await orderRepository.GetAllOrders();
        return Ok(orders);
    }
    [Authorize(Roles = "Admin")]
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetOrderById(Guid id)
    {
        var order = await orderRepository.GetOrderById(id);
        if (order == null)
        {
            return NotFound();
        }

        return Ok(order);
    }

    [Authorize(Roles = "User")]
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateOrder(OrderCreateDto orderDto)
    {
        var orderId = await orderRepository.CreateOrder(orderDto);
        return Ok(orderId);
    }

    [Authorize(Roles = "User,Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrder(Guid id, OrderUpdateDto orderDto)
    {
        try
        {
            await orderRepository.UpdateOrder(id, orderDto);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [Authorize(Roles = "User")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(Guid id)
    {
        try
        {
            await orderRepository.DeleteOrder(id);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
