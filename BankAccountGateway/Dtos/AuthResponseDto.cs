namespace BankAccountGateway.Controllers;

public record AuthResponseDto(string Message, string? JwtToken);