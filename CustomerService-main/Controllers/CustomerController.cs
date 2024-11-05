using Microsoft.AspNetCore.Mvc;
using Models;
using System.Linq;
using System.Diagnostics;

namespace CustomerService.Controllers;

[ApiController]
[Route("[controller]")]
public class CustomerController : ControllerBase
{
    private static List<Customer> _customers = new List<Customer>() {
        new () {
            Id = new Guid("c9fcbc4b-d2d1-4664-9079-dae78a1de446"),
            Name = "Æ Fiskehandler",
            Address1 = "Søndergade 3",
            City = "Harboøre",
            PostalCode = 7673,
            ContactName = "Jens Peter Olesen",
            TaxNumber = "133466789"
        }
    };
    private readonly ILogger<CustomerController> _logger;

    public CustomerController(ILogger<CustomerController> logger)
    {
        _logger = logger;
    }

    [HttpGet("{customerId}", Name = "GetCustomerById")]
    public Customer Get(Guid customerId)
    {
        _logger.LogInformation("Metode GetCustomerById called at {DT}",
            DateTime.UtcNow.ToLongTimeString());

        return _customers.Where(c => c.Id == customerId).First();
    }

    // POST en customer
    [HttpPost(Name = "CreateCustomer")]
    public IActionResult Post(Customer customer)
    {
        _logger.LogInformation("Metode CreateCustomer called at {DT}",
            DateTime.UtcNow.ToLongTimeString());

        // Tjek om Customer.Id allerede findes
        if (_customers.Any(c => c.Id == customer.Id))
        {
            _logger.LogWarning("Attempt to create a customer with an existing ID: {Id} at {DT}", 
                customer.Id, DateTime.UtcNow.ToLongTimeString());

            // Returner en 409 Conflict
            return Conflict();
        }

        customer.Id = Guid.NewGuid();
        _customers.Add(customer);
        return CreatedAtRoute("GetCustomerById", new { customerId = customer.Id }, customer);
    }
    
[HttpGet("version")]
public async Task<Dictionary<string,string>> GetVersion()
{
var properties = new Dictionary<string, string>();
var assembly = typeof(Program).Assembly;
properties.Add("service", "qgt-customer-service");
var ver = FileVersionInfo.GetVersionInfo(typeof(Program)
.Assembly.Location).ProductVersion;
properties.Add("version", ver!);
try {
var hostName = System.Net.Dns.GetHostName();
var ips = await System.Net.Dns.GetHostAddressesAsync(hostName);
var ipa = ips.First().MapToIPv4().ToString();
properties.Add("hosted-at-address", ipa);
} catch (Exception ex) {
_logger.LogError(ex.Message);
properties.Add("hosted-at-address", "Could not resolve IP-address");
}
return properties;
}

}
