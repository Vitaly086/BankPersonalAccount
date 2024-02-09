namespace BankAccountGateway.Controllers;

public record GetAccountDetailsDto(string FullName, string PhoneNumber, List<AccountInfoDto> Accounts);