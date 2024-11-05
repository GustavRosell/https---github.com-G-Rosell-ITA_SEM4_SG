using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DeliveryServiceWorker; 

// Opret en host og konfigurer worker
IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        // Tilføj din worker som en hosted service
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();
