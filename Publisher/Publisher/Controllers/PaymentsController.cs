using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Publisher.Context;
using Publisher.Domain;
using Publisher.ViewModels;
using RabbitMQ.Client;

namespace Publisher.Controllers;

[Controller]
[Route("Payment")]
public class PaymentsController : ControllerBase
{
    private MyDbContext _dbContext;
    private IConnection _rabbitConnection;

    public PaymentsController(MyDbContext dbContext, IConnectionFactory rabbitConnection)
    {
        _dbContext = dbContext;
        _rabbitConnection = rabbitConnection.CreateConnection();
    }

    [HttpPost]
    public IActionResult ProcessPayment([FromBody] PostPayments payments)
    {
        var payment = new Payments(payments.RelatedId, payments.Amount, DateTimeOffset.UtcNow, "Approved");

        _dbContext.Payments.Add(payment);

        if (_dbContext.SaveChanges() == 0)
            return BadRequest();

        using var channel = _rabbitConnection.CreateChannel();

        channel.QueueDeclare(queue: "PaymentProcessedQueue", durable: false, exclusive: false, autoDelete: false,
            arguments: null);
        
        var message = new
        {
            RelatedId = payment.Relatedid,
            Status = payment.Paymentstatus
        };
        
        var messageBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
        
        channel.BasicPublish(string.Empty, "PaymentProcessedQueue", messageBytes);

        channel.Close();
        _rabbitConnection.Close();
        
        return Ok(new { id = payment.Paymentid, Status = payment.Paymentstatus });
     
    }
}