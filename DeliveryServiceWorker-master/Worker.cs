namespace DeliveryServiceWorker;

using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private IConnection _connection;
    private IModel _channel;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
        InitializeRabbitMQ();
    }

    private void InitializeRabbitMQ()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        // Opret kø til at modtage shipping requests
        _channel.QueueDeclare(queue: "shippingQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            _logger.LogInformation($"Modtaget besked: {message}");

            // Deserialiser beskeden og skriv til CSV
            var shippingRequest = JsonSerializer.Deserialize<ShippingRequest>(message);
            WriteToCsv(shippingRequest);
        };

        _channel.BasicConsume(queue: "shippingQueue", autoAck: true, consumer: consumer);

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker kører på: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }
    }

    private void WriteToCsv(ShippingRequest request)
    {
        string csvFilePath = "shipping_requests.csv";  // CSV-fil gemmes her
        bool fileExists = File.Exists(csvFilePath);

        using (var writer = new StreamWriter(csvFilePath, true))
        {
            if (!fileExists)
            {
                writer.WriteLine("PackageId,MemberName,PickupAddress,DeliveryAddress");
            }

            string line = $"{request.PackageId},{request.MemberName},{request.PickupAddress},{request.DeliveryAddress}";
            writer.WriteLine(line);
        }
    }

    public override void Dispose()
    {
        _channel.Close();
        _connection.Close();
        base.Dispose();
    }
}

public class ShippingRequest
{
    public string PackageId { get; set; }
    public string MemberName { get; set; }
    public string PickupAddress { get; set; }
    public string DeliveryAddress { get; set; }
}
