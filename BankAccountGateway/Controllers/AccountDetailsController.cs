using BankAccountGateway.Extensions;
using BankAccountService;
using BankAccountService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankAccountGateway.Controllers;

[ApiController]
[Route("api/v1/accountDetails")]
[Authorize]
public class AccountDetailsController : ControllerBase
{
    private readonly string _clientIdKey;
    private readonly AccountDetailsGrpc.AccountDetailsGrpcClient _accountDetailsClient;


    public AccountDetailsController(AccountDetailsGrpc.AccountDetailsGrpcClient accountDetailsClient,
        IConfiguration configuration)
    {
        _accountDetailsClient = accountDetailsClient;
        _clientIdKey = configuration.GetSection("ClientIdKey").Value;
    }

    [HttpGet]
    public async Task<ActionResult<GetAccountDetailsDto>> GetAccountDetailsAsync(CancellationToken token)
    {
        var jwtToken = HttpContext.ReadJwtToken();
        if (jwtToken == null)
        {
            return Unauthorized("Jwt token not found");
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

        var response = await _accountDetailsClient.GetAccountDetailsAsync(request, cancellationToken: token);


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
}