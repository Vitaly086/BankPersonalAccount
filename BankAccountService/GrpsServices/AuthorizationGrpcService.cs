using BankAccountService.Dtos;
using BankAccountService.Services.Interfaces;
using Grpc.Core;

namespace BankAccountService.GrpsServices;

public class AuthorizationGrpcService : AuthorizationGrpc.AuthorizationGrpcBase
{
    private readonly IAuthorizationService _authorizationService;

    public AuthorizationGrpcService(IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
    }

    public override async Task<AuthResponse> Register(RegisterRequest request, ServerCallContext context)
    {
        var result =
            await _authorizationService.RegisterClientAsync(request.FullName, request.PhoneNumber, request.Password,
                context.CancellationToken);

        return MapToAuthResponse(result);
    }

    public override async Task<AuthResponse> Login(LoginRequest request, ServerCallContext context)
    {
        var result =
            await _authorizationService.LoginClientAsync(request.PhoneNumber, request.Password,
                context.CancellationToken);

        return MapToAuthResponse(result);
    }

    private AuthResponse MapToAuthResponse(OperationResult<AuthClientDto> result)
    {
        return new AuthResponse
        {
            Success = result.Success,
            Message = result.Message,
            UserId = result.Success ? result.Data.UserId : 0,
            JwtToken = result.Success ? result.Data.JwtToken : string.Empty
        };
    }
}