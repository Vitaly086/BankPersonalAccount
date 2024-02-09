using BankAccountService.Services.Interfaces;
using Grpc.Core;

namespace BankAccountService.GrpsServices;

public class AccountDetailsGrpcService : AccountDetailsGrpc.AccountDetailsGrpcBase
{
    private readonly IClientAccountRepository _clientAccountRepository;
    private readonly ILogger<AccountDetailsGrpcService> _logger;

    public AccountDetailsGrpcService(IClientAccountRepository clientAccountRepository,
        ILogger<AccountDetailsGrpcService> logger)
    {
        _clientAccountRepository = clientAccountRepository;
        _logger = logger;
    }

    public override async Task<AccountDetailsResponse> GetAccountDetails(AccountDetailsRequest request,
        ServerCallContext context)
    {
        try
        {
            var client =
                await _clientAccountRepository.GetClientWithAccountsAsync(request.ClientId, context.CancellationToken);

            if (client == null)
            {
                return new AccountDetailsResponse
                {
                    Success = false,
                    Message = "Client not found."
                };
            }

            var accountInfos = client.Accounts.Select(a => new AccountInfo
            {
                AccountType = (int)a.AccountType,
                AccountNumber = a.AccountNumber
            }).ToList();

            return new AccountDetailsResponse
            {
                Success = true,
                FullName = client.FullName,
                PhoneNumber = client.PhoneNumber,
                Accounts = { accountInfos }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while processing GetAccountDetails for client ID {request.ClientId}");
            
            return new AccountDetailsResponse
            {
                Success = false,
                Message = "An error occurred on the server."
            };
        }
    }
}