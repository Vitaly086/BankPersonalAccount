using System.ComponentModel.DataAnnotations;
using BankAccountService.Attributes;

namespace BankAccountService.Entity;

public class Client
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string FullName { get; set; }
    [RussianPhoneNumber]
    [Required]
    public string PhoneNumber { get; set; }
    public string PasswordHash { get; set; }
    public string PasswordSalt { get; set; }

    public virtual ICollection<Account> Accounts { get; set; }
}