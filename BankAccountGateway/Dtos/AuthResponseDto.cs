namespace BankAccountGateway.Dtos;

public record AuthResponseDto(string Message, string? JwtToken);