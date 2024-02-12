using System.ComponentModel.DataAnnotations;

namespace BankAccountService.Entity;

public class Account
{
    [Key]
    public int Id { get; set; }

    public long AccountNumber { get; set; }
    public AccountType AccountType { get; set; }

    public int ClientId { get; set; }
    public virtual Client Client { get; set; }
}