using BankAccountService.Entity;

namespace BankAccountGateway.Dtos;

public record AccountInfoDto(int Id, long AccountNumber, AccountType AccountType);