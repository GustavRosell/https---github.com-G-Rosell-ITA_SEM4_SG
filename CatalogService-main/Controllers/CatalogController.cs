using Microsoft.AspNetCore.Mvc;
using Models;
using System.Linq;
using System.Diagnostics;

namespace CatalogService.Controllers;

[ApiController]
[Route("[controller]")]
public class CatalogController : ControllerBase
{
    private static List<Product> _products = new List<Product>() {
new () {
Id = new Guid("7125e019-c469-4dbd-93e5-426de6652523"),
Name = "Salmon Fillet",
Description = "Fresh salmon fillet",
Price = 12.99m,
Brand = "FishmongerX",
Manufacturer = "Fish Supplier",
Model = "Standard",
ImageUrl = "https://example.com/salmon.jpg",
ProductUrl = "https://example.com/salmon",
ReleaseDate = DateTime.Now,
ExpiryDate = DateTime.Now.AddDays(3), // Example expiry date 3 days from now
}
};

    private readonly ILogger<CatalogController> _logger;

    public CatalogController(ILogger<CatalogController> logger)
    {
        _logger = logger;
    }


    [HttpGet("{productId}", Name = "GetProductById")]
    public Product Get(Guid productId)
    {
        return _products.Where(c => c.Id == productId).First();
    }

    /* Get-metode til at hente versionsinfo*/
    [HttpGet("version")]
    public async Task<Dictionary<string, string>> GetVersion()
    {
        var properties = new Dictionary<string, string>();
        var assembly = typeof(Program).Assembly;
        properties.Add("service", "qgt-customer-service");
        var ver = FileVersionInfo.GetVersionInfo(typeof(Program)
        .Assembly.Location).ProductVersion;
        properties.Add("version", ver!);
        try
        {
            var hostName = System.Net.Dns.GetHostName();
            var ips = await System.Net.Dns.GetHostAddressesAsync(hostName);
            var ipa = ips.First().MapToIPv4().ToString();
            properties.Add("hosted-at-address", ipa);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            properties.Add("hosted-at-address", "Could not resolve IP-address");
        }
        return properties;
    }
}
