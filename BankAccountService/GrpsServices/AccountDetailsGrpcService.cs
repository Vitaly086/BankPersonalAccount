using BankAccountService.Services.Interfaces;
using Grpc.Core;

namespace BankAccountService.GrpsServices;

public class AccountDetailsGrpcService : AccountDetailsGrpc.AccountDetailsGrpcBase
{
    private readonly IClientAccountRepository _clientAccountRepository;

    public AccountDetailsGrpcService(IClientAccountRepository clientAccountRepository)
    {
        _clientAccountRepository = clientAccountRepository;
    }
    
    public override async Task<AccountDetailsResponse> GetAccountDetails(AccountDetailsRequest request,
        ServerCallContext context)
    {
        var client = await _clientAccountRepository.GetClientWithAccountsAsync(request.ClientId, context.CancellationToken);

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
}