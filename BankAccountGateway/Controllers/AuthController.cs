using BankAccountService;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;

namespace BankAccountGateway.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly GrpcSettings _grpcSettings;

    public AuthController(GrpcSettings grpcSettings)
    {
        _grpcSettings = grpcSettings;
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

        var authorizationGrpcClient = CreateAuthorizationGrpcClient();
        var response = await authorizationGrpcClient.LoginAsync(loginRequest, cancellationToken: token);

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
        
        var authorizationGrpcClient = CreateAuthorizationGrpcClient();

        var response = await authorizationGrpcClient.RegisterAsync(registerRequest, cancellationToken: token);

        if (response.Success)
        {
            return Ok(new AuthResponseDto(response.Message, response.JwtToken));
        }

        return BadRequest(new AuthResponseDto(response.Message, null));
    }
    
    private AuthorizationGrpc.AuthorizationGrpcClient CreateAuthorizationGrpcClient()
    {
        var handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
        var channel = GrpcChannel.ForAddress(_grpcSettings.ServerAddress, new GrpcChannelOptions { HttpHandler = handler });
        return new AuthorizationGrpc.AuthorizationGrpcClient(channel);
    }
}