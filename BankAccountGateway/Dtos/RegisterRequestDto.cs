namespace BankAccountGateway.Controllers;

public record RegisterRequestDto(string FullName, string PhoneNumber, string Password);