using BankAccountService.Dtos;

namespace BankAccountService.Services.Interfaces;

public interface IAuthorizationService
{
    Task<OperationResult<AuthClientDto>> RegisterClientAsync(string requestFullName, string requestPhoneNumber, string requestPassword, CancellationToken token);
    Task<OperationResult<AuthClientDto>> LoginClientAsync(string requestPhoneNumber, string requestPassword, CancellationToken token);
}