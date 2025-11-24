using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SportStore.Models;

[Table("Employee")]
public partial class Employee
{
    [Key]
    [Column("EmployeeID")]
    public int EmployeeId { get; set; }

    [StringLength(30)]
    [Unicode(false)]
    [Display(Name = "Mã nhân viên")]
    public string? EmployeeCode { get; set; }

    [StringLength(50)]
    [Display(Name = "Tên nhân vien")]
    public string? FullName { get; set; }

    [StringLength(10)]
    [Display(Name = "Giới tính")]
    public string? Sex { get; set; }
    [Display(Name = "Ngày sinh")]
    public DateOnly? BirthDate { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    [Display(Name = "Số điện thoại")]
    public string? PhoneNumber { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? Email { get; set; }

    [StringLength(255)]
    [Display(Name = "Địa chỉ")]
    public string? Address { get; set; }

    [Column("BranchID")]
    public int? BranchId { get; set; }

    [StringLength(100)]
    public string? Position { get; set; }

    public DateOnly? HireDate { get; set; }

    public bool? IsActive { get; set; }

    [InverseProperty("Employee")]
    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    [ForeignKey("BranchId")]
    [InverseProperty("Employees")]
    public virtual Branch? Branch { get; set; }

    [InverseProperty("Employee")]
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
