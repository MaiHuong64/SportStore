using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SportStore.Models;

[Table("Supplier")]
[Index("SupplierCode", Name = "UQ__Supplier__44BE981B96D14007", IsUnique = true)]
public partial class Supplier
{
    [Key]
    [Column("SupplierID")]
    public int SupplierId { get; set; }

    [StringLength(30)]
    [Unicode(false)]
    public string SupplierCode { get; set; } = null!;

    [StringLength(255)]
    [Display(Name = "Nhà cung cấp")]
    public string SupplierName { get; set; } = null!;

    [StringLength(100)]
    [Display(Name = "Người kết nối")]
    public string? ContactPerson { get; set; }

    [StringLength(15)]
    [Unicode(false)]
    [Display(Name = "Số điện thoại")]
    public string? PhoneNumber { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [StringLength(255)]
    [Display(Name = "Địa chỉ")]
    public string? Address { get; set; }
    [Display(Name = "Trạng thái")]
    public bool? IsActive { get; set; }

    [InverseProperty("Supplier")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
