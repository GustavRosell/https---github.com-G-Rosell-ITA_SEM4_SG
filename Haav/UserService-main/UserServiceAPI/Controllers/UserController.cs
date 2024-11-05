using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Model; // Sørg for, at navneområdet til Model klassen er korrekt
using Service; // Sørg for, at navneområdet til Service klassen er korrekt
using Microsoft.Extensions.Logging;

namespace UserAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserDBRepository _userRepository;

    public UserController(ILogger<UserController> logger, IUserDBRepository userRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
    }

    [HttpGet("version")]
    public async Task<Dictionary<string, string>> GetVersion()
    {
        var properties = new Dictionary<string, string>();
        var assembly = typeof(Program).Assembly;
        properties.Add("service", "UserService");
        var ver = FileVersionInfo.GetVersionInfo(
        typeof(Program).Assembly.Location).ProductVersion ?? "N/A";
        properties.Add("version", ver);
        var hostName = System.Net.Dns.GetHostName();
        var ips = await System.Net.Dns.GetHostAddressesAsync(hostName);
        var ipa = ips.First().MapToIPv4().ToString() ?? "N/A";
        properties.Add("ip-address", ipa);
        return properties;
    }

    // CREATE - POST /user
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] User user)
    {
        if (user == null)
        {
            return BadRequest("User cannot be null.");
        }

        var createdUser = await _userRepository.CreateUserAsync(user);
        return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
    }

    // READ - GET /user/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(string id)
    {
        var user = await _userRepository.GetUserByIdAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    // READ ALL - GET /user
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userRepository.GetAllUsersAsync();
        return Ok(users);
    }

    // UPDATE - PUT /user/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] User updatedUser)
    {
        if (updatedUser == null || updatedUser.Id != id)
        {
            return BadRequest("User data is invalid.");
        }

        var existingUser = await _userRepository.GetUserByIdAsync(id);

        if (existingUser == null)
        {
            return NotFound();
        }

        await _userRepository.UpdateUserAsync(id, updatedUser);
        return NoContent();
    }

    // DELETE - DELETE /user/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var existingUser = await _userRepository.GetUserByIdAsync(id);

        if (existingUser == null)
        {
            return NotFound();
        }

        var success = await _userRepository.DeleteUserAsync(id);
        if (success)
        {
            return NoContent();
        }
        else
        {
            return StatusCode(500, "An error occurred while deleting the user.");
        }
    }

    // READ - GET /user/byemail/{email}
    [HttpGet("byemail/{email}")]
    public async Task<IActionResult> GetUserByEmail(string email)
    {
        var user = await _userRepository.GetUserByEmailAsync(email);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

}