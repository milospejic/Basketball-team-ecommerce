using AutoMapper;
using backend.Data.Repository;
using backend.Models;
using backend.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Route("api/order")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly IOrderRepository orderRepository;
    private readonly IMapper mapper;
    private readonly IUserRepository userRepository;

    public OrderController(IOrderRepository orderRepository, IMapper mapper, IUserRepository userRepository)
    {
        this.orderRepository = orderRepository;
        this.mapper = mapper;
        this.userRepository = userRepository;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllOrders(int page = 1, int pageSize = 10, string sortBy = "", string sortOrder = "")
    {
        var orders = await orderRepository.GetAllOrders(page, pageSize, sortBy, sortOrder);
        if (orders == null || orders.Count() == 0)
        {
            return NoContent();
        }
        return Ok(orders);
    }

    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize(Roles = "Admin")]
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetOrderById(Guid id)
    {
        var order = await orderRepository.GetOrderById(id);
        if (order == null)
        {
            return NotFound("There is no order with id: " + id);
        }

        return Ok(order);
    }

    [Authorize(Roles = "User")]
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Guid>> CreateOrder(OrderCreateDto orderDto)
    {
        try
        {
            var user = userRepository.GetUserByEmail(User.FindFirst(ClaimTypes.Email)?.Value);
            var order = await orderRepository.CreateOrder(orderDto, user.UserId);
            return Ok(order);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = "User,Admin")]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

    [Authorize(Roles = "User")]
    [HttpGet("user/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllOrdersForUser(Guid userId)
    {
        var orders = await orderRepository.GetOrdersByUserId(userId);
        if (orders == null || orders.Count() == 0)
        {
            return NoContent();
        }
        return Ok(orders);
    }

    [Authorize(Roles = "User")]
    [HttpPatch("{orderId}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> PayOrder(Guid orderId)
    {
        var order = await orderRepository.GetOrderById(orderId);
        if (order == null)
        {
            return NotFound();
        }

        await orderRepository.PayOrder(orderId);
        var updatedOrder = await orderRepository.GetOrderById(orderId);
        return Ok(mapper.Map<OrderDto>(updatedOrder));
    }
}