using System.IdentityModel.Tokens.Jwt;
using BankAccountGateway.Extensions;
using BankAccountService;
using BankAccountService.Services;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankAccountGateway.Controllers;

[ApiController]
[Route("api/v1/accountDetails")]
[Authorize]
public class AccountDetailsController : ControllerBase
{
    private readonly GrpcSettings _grpcSettings;
    private readonly string _clientIdKey;

    public AccountDetailsController(GrpcSettings grpcSettings, IConfiguration configuration)
    {
        _grpcSettings = grpcSettings;
        _clientIdKey = configuration.GetSection("ClientIdKey").Value;
    }

    [HttpGet]
    public async Task<ActionResult<GetAccountDetailsDto>> GetAccountDetailsAsync(CancellationToken token)
    {
        var jwtToken = HttpContext.ReadJwtToken();
        if (jwtToken == null)
        {
            return Unauthorized("Client ID claim not found in token");
        }
        
        
        var clientIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub");
        if (clientIdClaim == null)
        {
            return Unauthorized("Client ID claim not found in token");
        }

        var clientId = DecryptClientId(clientIdClaim.Value);
        var request = new AccountDetailsRequest()
        {
            ClientId = clientId
        };

        var accountDetailsGrpcClient = CreateAccountDetailsGrpcClient();
        var response = await accountDetailsGrpcClient.GetAccountDetailsAsync(request, cancellationToken: token);


        if (response.Success)
        {
            return Ok(response);
        }

        return BadRequest(response.Message);
    }

    private int DecryptClientId(string encryptedClientId)
    {
        return ClientIdEncryptor.DecryptClientId(encryptedClientId, _clientIdKey);
    }

    private AccountDetailsGrpc.AccountDetailsGrpcClient CreateAccountDetailsGrpcClient()
    {
        var handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
        var channel =
            GrpcChannel.ForAddress(_grpcSettings.ServerAddress, new GrpcChannelOptions { HttpHandler = handler });
        return new AccountDetailsGrpc.AccountDetailsGrpcClient(channel);
    }
}