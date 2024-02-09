using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BankAccountService.DbContext;
using BankAccountService.Dtos;
using BankAccountService.Entity;
using BankAccountService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BankAccountService.Services.Implementations;

public class AuthorizationService : IAuthorizationService
{
    private readonly BankAccountContext _accountContext;
    private readonly ILogger<AuthorizationService> _logger;
    private readonly string _jwtToken;
    private readonly string _clientIdKey;

    public AuthorizationService(BankAccountContext accountContext, IConfiguration configuration,
        ILogger<AuthorizationService> logger)
    {
        _accountContext = accountContext;
        _logger = logger;
        _jwtToken = configuration.GetSection("JwtToken").Value;
        _clientIdKey = configuration.GetSection("ClientIdKey").Value;
    }

    public async Task<OperationResult<AuthClientDto>> RegisterClientAsync(string fullName, string phoneNumber,
        string password,
        CancellationToken token)
    {
        try
        {
            if (await _accountContext.Clients.AnyAsync(c => c.PhoneNumber == phoneNumber, cancellationToken: token))
            {
                return OperationResult<AuthClientDto>.Fail("Client with this phone number already exists.");
            }

            CreatePasswordHash(password, out var passwordHash, out var passwordSalt);

            var client = new Client
            {
                FullName = fullName,
                PhoneNumber = phoneNumber,
                PasswordHash = Convert.ToBase64String(passwordHash),
                PasswordSalt = Convert.ToBase64String(passwordSalt)
            };

            _accountContext.Clients.Add(client);
            await _accountContext.SaveChangesAsync(token);

            var jwtToken = GenerateJwtToken(client.Id);
            var authClientDto = new AuthClientDto(client.Id, jwtToken);

            return OperationResult<AuthClientDto>.Ok(authClientDto, "Registration successful.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Registration failed for {phoneNumber}. Error: {ex.Message}");
            return OperationResult<AuthClientDto>.Fail(
                "An error occurred during registration. Please try again later.");
        }
    }

    public async Task<OperationResult<AuthClientDto>> LoginClientAsync(string phoneNumber, string password,
        CancellationToken token)
    {
        try
        {
            var client = await _accountContext.Clients
                .FirstOrDefaultAsync(c => c.PhoneNumber == phoneNumber, cancellationToken: token);

            if (client == null || !VerifyPasswordHash(password, Convert.FromBase64String(client.PasswordHash),
                    Convert.FromBase64String(client.PasswordSalt)))
            {
                return OperationResult<AuthClientDto>.Fail("Phone number or password is incorrect.");
            }

            var jwtToken = GenerateJwtToken(client.Id);
            var authClientDto = new AuthClientDto(client.Id, jwtToken);

            return OperationResult<AuthClientDto>.Ok(authClientDto, "Authentication successful.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Login attempt failed for {phoneNumber}. Error: {ex.Message}");
            return OperationResult<AuthClientDto>.Fail("An error occurred during login. Please try again later.");
        }
    }

    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using var hmac = new HMACSHA512();
        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    }

    private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
    {
        using var hmac = new HMACSHA512(storedSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return computedHash.SequenceEqual(storedHash);
    }

    private string GenerateJwtToken(int clientId)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtToken));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var encryptedClientId = EncryptClientId(clientId);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, encryptedClientId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string EncryptClientId(int clientId)
    {
        return ClientIdEncryptor.EncryptClientId(clientId, _clientIdKey);
    }
}