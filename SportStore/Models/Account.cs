using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SportStore.Models;

[Table("Account")]
public partial class Account
{
    [Key]
    [Column("AccountID")]
    public int AccountId { get; set; }

    [StringLength(10)]
    [Display(Name = "Số điện thoại")]
    [Unicode(false)]
    public string? PhoneNumber { get; set; }

    [StringLength(255)]
    [Display(Name = "Mật khẩu")]
    public string? Password { get; set; }

    [StringLength(30)]
    [Display(Name = "Vai trò")]
    public string? Role { get; set; }

    [Display(Name = "Trạng thái")]
    public int? Status { get; set; }

    [Column("CustomerID")]
    public int? CustomerId { get; set; }

    [Column("EmployeeID")]
    public int? EmployeeId { get; set; }

    [ForeignKey("CustomerId")]
    [InverseProperty("Accounts")]
    public virtual Customer? Customer { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("Accounts")]
    public virtual Employee? Employee { get; set; }
}
