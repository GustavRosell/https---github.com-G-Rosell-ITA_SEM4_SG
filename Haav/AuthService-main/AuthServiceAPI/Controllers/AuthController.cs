using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Text;
using System;
using System.Diagnostics;
using NLog;
using Model.User;
using System.Text.Json;

/* Adding vault */
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.Commons;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpClientFactory;
        private string _secret = null;
        private string _issuer = null;

        public AuthController(ILogger<AuthController> logger, IConfiguration config, IHttpClientFactory httpClientFactory)
        {
            _config = config;
            _logger = logger;

        }

        /* Adding Vault  */

        private async Task connectTovVault()
        {
            var EndPoint = _config["vaulturl"] ?? "<blank>";
            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => { return true; };
            // Initialize one of the several auth methods.
            IAuthMethodInfo authMethod =
            new TokenAuthMethodInfo("00000000-0000-0000-0000-000000000000");
            // Initialize settings. You can also set proxies, custom delegates etc. here.
            var vaultClientSettings = new VaultClientSettings(EndPoint, authMethod)
            {
                Namespace = "",
                MyHttpClientProviderFunc = handler
                => new HttpClient(httpClientHandler)
                {
                    BaseAddress = new Uri(EndPoint)
                }
            };
            IVaultClient vaultClient = new VaultClient(vaultClientSettings);

            // Read a key-value secret.
            Secret<SecretData> kv2Secret = await vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(path: "authenticators", mountPoint: "secret");
            _secret = kv2Secret.Data.Data["Secret"].ToString()!;
            _issuer = kv2Secret.Data.Data["Issuer"].ToString()!;

            _logger.LogInformation($"Secret: {_secret}");
            _logger.LogInformation($"Issuer: {_issuer}");
        }
        private string GenerateJwtToken(string username)
        {

            _logger.LogInformation($"Secret: {_secret}");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, username)
            };

            var token = new JwtSecurityToken(
                _issuer,
                "http://localhost",
                claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

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

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel login)
        {
            await connectTovVault();
            if (login.Username == "haavy_user" && login.Password == "aaakodeord")
            {
                var token = GenerateJwtToken(login.Username);
                return Ok(new { token });
            }

            return Unauthorized();
        }

        private async Task<User?> GetUserData(LoginModel login)
        {
            var endpointUrl = _config["UserServiceEndpoint"]! + "/" + login.Username;
            _logger.LogInformation("Retrieving user data from: {}", endpointUrl);
            var client = _httpClientFactory.CreateClient();
            HttpResponseMessage response;
            try
            {
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                response = await client.GetAsync(endpointUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    string? userJson = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<User>(userJson);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    return null;
                }
            }
            return null;
        }
    }
}