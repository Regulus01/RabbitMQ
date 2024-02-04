using AutoMapper;
using Publisher.Domain;
using Publisher.Infra.Context;
using Publisher.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Publisher.Controllers;

[Controller]
[Route("api/Orders")]
public class OrdersController : ControllerBase
{
    private MyDbContext _dbContext;
    private IMapper _mapper;

    public OrdersController(MyDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    
    /// <summary>
    /// Inclui um pedido no sistema
    /// </summary>
    /// <param name="viewModel">Dados necessário para inserção do pedido</param>
    /// <returns>Id do pedido incluido</returns>
    [HttpPost]
    public IActionResult PlaceOrder([FromBody] PlaceOrderViewModel viewModel)
    {
        var order = new Orders(Guid.NewGuid(), viewModel.Customername, DateTimeOffset.UtcNow, "Pending");

        var produtos = _dbContext.Products.Where(x => viewModel.ProductsIds.Contains(x.Id)).ToList();

        if (produtos.Count == 0)
            return BadRequest("Produtos não encontrados");

        order.Product = produtos;

        _dbContext.Orders.Add(order);

        _dbContext.SaveChanges();
        
        return Ok(order.Id);
    }
    
    /// <summary>
    /// Obtém os pedidos a partir do nome do cliente
    /// </summary>
    /// <param name="customerName"></param>
    /// <returns>Pedidos do cliente</returns>
    [HttpGet]
    [Route("{customerName}")]
    public IActionResult GetOrders([FromRoute] string customerName)
    {
        var orders = _dbContext.Orders.Where(x => x.Customername.ToUpper().Equals(customerName.ToUpper()))
                                      .Include(x => x.Product) 
                                      .OrderByDescending(x => x.Orderdate)
                                      .ToList();
        
        if (orders.Count == 0)
            return BadRequest("Pedidos não encontrados");

        var orderViewModel = _mapper.Map<List<GetOrderViewModel>>(orders);
        
        return Ok(orderViewModel);
    }
}