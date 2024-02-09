using System.ComponentModel.DataAnnotations;

namespace BankAccountService.Entity;

public class Client
{
    [Key]
    public int Id { get; set; }
    public string FullName { get; set; }
    public string PhoneNumber { get; set; }
    public string PasswordHash { get; set; }
    public string PasswordSalt { get; set; }

    public virtual ICollection<Account> Accounts { get; set; }
}