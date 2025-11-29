using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SportStore.Models;

[Table("InvoiceDetail")]
public partial class InvoiceDetail
{
    [Key]
    [Column("InvoiceDetailID")]
    public int InvoiceDetailId { get; set; }

    [Column("InvoiceID")]
    public int InvoiceId { get; set; }

    [Column("ProductID")]
    public int? ProductId { get; set; }
    [Display(Name = "Số lượng")]
    public int? Quantity { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    [Display(Name = "Đơn giá")]
    public decimal? UnitPrice { get; set; }

    [ForeignKey("InvoiceId")]
    [InverseProperty("InvoiceDetails")]
    public virtual Invoice? Invoice { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("InvoiceDetails")]
    public virtual Product? Product { get; set; }
}
