using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SportStore.Models;

[Table("ProductDetail")]
[Index("Sku", Name = "UQ__ProductD__CA1ECF0DBEF8EB48", IsUnique = true)]
public partial class ProductDetail
{
    [Key]
    [Column("ProductDetailID")]
    public int ProductDetailId { get; set; }

    [Column("ProductID")]
    public int ProductId { get; set; }

    [Column("SKU")]
    [StringLength(50)]
    [Unicode(false)]
    public string? Sku { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Price { get; set; }

    public byte? Size { get; set; }

    [StringLength(30)]
    public string? Color { get; set; }

    public int? Quantity { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("ProductDetails")]
    public virtual Product Product { get; set; } = null!;
}
