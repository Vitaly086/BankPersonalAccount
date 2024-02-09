using BankAccountService.DbContext;
using BankAccountService.Entity;
using BankAccountService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BankAccountService.Services.Implementations;

public class ClientAccountAccountRepository : IClientAccountRepository
{
    private readonly BankAccountContext _context;

    public ClientAccountAccountRepository(BankAccountContext context)
    {
        _context = context;
    }

    public async Task<Client?> GetClientWithAccountsAsync(int clientId, CancellationToken contextCancellationToken)
    {
        return await _context.Clients
            .Include(c => c.Accounts)
            .FirstOrDefaultAsync(c => c.Id == clientId, cancellationToken: contextCancellationToken);
    }
}