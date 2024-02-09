using BankAccountService.Entity;

namespace BankAccountGateway.Controllers;

public record AccountInfoDto(int Id, long AccountNumber, AccountType AccountType);