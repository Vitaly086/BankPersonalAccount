using System.ComponentModel.DataAnnotations;
using BankAccountService.Attributes;

namespace BankAccountGateway.Dtos;

public record RegisterRequestDto(
    [Required] string FullName, 
    [RussianPhoneNumber][Required] string PhoneNumber, 
    [Required] string Password);
