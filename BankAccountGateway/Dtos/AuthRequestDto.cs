using System.ComponentModel.DataAnnotations;
using BankAccountService.Attributes;

namespace BankAccountGateway.Dtos;

public record AuthRequestDto([RussianPhoneNumber][Required] string PhoneNumber, 
    [Required] string Password);