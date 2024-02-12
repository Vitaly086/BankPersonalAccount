using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BankAccountService.Attributes;

public class RussianPhoneNumberAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value == null) return false;

        var phoneNumber = value.ToString();
        var regex = new Regex(@"^\+7[0-9]{10}$");

        return phoneNumber != null && regex.IsMatch(phoneNumber);
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        return !IsValid(value) ? new ValidationResult("Invalid Russian phone number format. Expected format: +7XXXXXXXXXX.") : ValidationResult.Success;
    }
}