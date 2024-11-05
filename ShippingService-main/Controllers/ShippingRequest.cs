using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;  // Tilføjet for JSON-serialisering

namespace ShippingService.Controllers
{
    [ApiController]
    [Route("api/shipping")]
    public class ShippingController : ControllerBase
    {
        private static List<ShippingRequest> shippingRequests = new List<ShippingRequest>();

        // POST: api/shipping/request
        [HttpPost("request")]
        public IActionResult CreateShippingRequest([FromBody] ShippingRequest request)
        {
            // Gem forsendelsesanmodningen i listen
            shippingRequests.Add(request);

            // Publicer til RabbitMQ
            PublishToRabbitMQ(request);

            return Ok("Shipping request received");
        }

        // GET: api/shipping/delivery-plan
        [HttpGet("delivery-plan")]
        public IActionResult GetDeliveryPlan()
        {
            // Returner leveringsplanen (læses fra CSV)
            var deliveryPlan = System.IO.File.ReadAllText("DeliveryPlan.csv");
            return Ok(deliveryPlan);
        }

        // Metode til at publicere ShipmentDelivery til RabbitMQ
        private void PublishToRabbitMQ(ShippingRequest request)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest", // Tilføj brugernavn
                Password = "guest"  // Tilføj adgangskode
            };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            // Deklarer en kø, hvor meddelelserne vil blive sendt
            channel.QueueDeclare(queue: "shippingQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);

            // Serialiser shippingRequest til JSON-format
            var message = JsonSerializer.Serialize(request);
            var body = Encoding.UTF8.GetBytes(message);

            // Send beskeden til køen
            channel.BasicPublish(exchange: "", routingKey: "shippingQueue", basicProperties: null, body: body);
            Console.WriteLine(" [x] Sent {0}", message);
        }
    }
}

public class ShippingRequest
{
    public string MemberName { get; set; }
    public string PickupAddress { get; set; }
    public string PackageId { get; set; }
    public string DeliveryAddress { get; set; }
}
