using BankAccountGateway.Dtos;
using BankAccountService;
using Microsoft.AspNetCore.Mvc;

namespace BankAccountGateway.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthorizationGrpc.AuthorizationGrpcClient _authorizationClient;

    public AuthController(AuthorizationGrpc.AuthorizationGrpcClient authorizationClient)
    {
        _authorizationClient = authorizationClient;
    }

    [HttpPost]
    [Route("login")]
    public async Task<ActionResult<AuthResponseDto>> LoginAsync(AuthRequestDto requestDto, CancellationToken token)
    {
        var loginRequest = new LoginRequest
        {
            PhoneNumber = requestDto.PhoneNumber,
            Password = requestDto.Password
        };

        var response = await _authorizationClient.LoginAsync(loginRequest, cancellationToken: token);

        if (response.Success)
        {
            return Ok(new AuthResponseDto(response.Message, response.JwtToken));
        }

        return BadRequest(new AuthResponseDto(response.Message, null));
    }

    [HttpPost]
    [Route("register")]
    public async Task<ActionResult<AuthResponseDto>> RegisterAsync(RegisterRequestDto request, CancellationToken token)
    {
        var registerRequest = new RegisterRequest()
        {
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            Password = request.Password
        };

        var response = await _authorizationClient.RegisterAsync(registerRequest, cancellationToken: token);

        if (response.Success)
        {
            return Ok(new AuthResponseDto(response.Message, response.JwtToken));
        }

        return BadRequest(new AuthResponseDto(response.Message, null));
    }
}