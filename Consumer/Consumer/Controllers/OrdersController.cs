using System.Text;
using AutoMapper;
using Consumer.Domain;
using Consumer.Infra.Context;
using Consumer.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Consumer.Controllers;

[Controller]
[Route("api/Orders")]
public class OrdersController : ControllerBase
{
    private MyDbContext _dbContext;
    private IMapper _mapper;
    private IConnection _rabbitConnection;

    public OrdersController(MyDbContext dbContext, IMapper mapper, IConnectionFactory rabbitConnection)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _rabbitConnection = rabbitConnection.CreateConnection();
    }
    
    /// <summary>
    /// Inclui um pedido no sistema
    /// </summary>
    /// <param name="viewModel">Dados necessário para inserção do pedido</param>
    /// <returns>Id do pedido incluido</returns>
    [HttpPost]
    public IActionResult PlaceOrder([FromBody] PlaceOrderViewModel viewModel)
    {
        var order = new Orders(Guid.NewGuid(), viewModel.Customername, DateTimeOffset.UtcNow, "Awaiting Payment");

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

    [HttpPost]
    [Route("ProcessOrders")]
    public IActionResult ProcessOrders()
    {
        using var channel = _rabbitConnection.CreateChannel();
        
        channel.QueueDeclare(queue: "PaymentProcessedQueue", durable: false, exclusive: false, autoDelete: false,
            arguments: null);

        var consumer = new EventingBasicConsumer(channel);

        var message = string.Empty;
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            message = Encoding.UTF8.GetString(body);
            Console.WriteLine(message);
        };

        channel.BasicConsume(queue: "PaymentProcessedQueue", autoAck: true, consumer: consumer);
        
        channel.Close();

        return Ok();
    }
    
    
}