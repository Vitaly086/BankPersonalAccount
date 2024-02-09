using BankAccountService.Entity;

namespace BankAccountService.Services.Interfaces;

public interface IClientAccountRepository
{
    Task<Client?> GetClientWithAccountsAsync(int clientId, CancellationToken contextCancellationToken);
}