namespace BankAccountGateway.Dtos;

public record GetAccountDetailsDto(string FullName, string PhoneNumber, List<AccountInfoDto> Accounts);