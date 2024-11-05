using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace OrderServiceAPI.Controllers;

[ApiController]
[Route("order")]
public class OrderServiceController : ControllerBase
{

    private readonly ILogger<OrderServiceController> _logger;

    public OrderServiceController(ILogger<OrderServiceController> logger)
    {
        _logger = logger;
    }

    // GET /orderservice
    [HttpGet("version")]
    public async Task<Dictionary<string, string>> GetVersion()
    {
        var properties = new Dictionary<string, string>();
        var assembly = typeof(Program).Assembly;

        properties.Add("service", "OrderService");
        var ver = FileVersionInfo.GetVersionInfo(
            typeof(Program).Assembly.Location).ProductVersion ?? "N/A";
        properties.Add("version", ver);

        var hostName = System.Net.Dns.GetHostName();
        var ips = await System.Net.Dns.GetHostAddressesAsync(hostName);
        var ipa = ips.First().MapToIPv4().ToString() ?? "N/A";
        properties.Add("ip-address", ipa);
        
        return properties;
    }


}
