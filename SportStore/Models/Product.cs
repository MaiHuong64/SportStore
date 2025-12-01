using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SportStore.Models;

[Table("Product")]
[Index("ProductCode", Name = "UQ__Product__2F4E024F40D40EC1", IsUnique = true)]
public partial class Product
{
    [Key]
    [Column("ProductID")]
    public int ProductId { get; set; }

    [StringLength(30)]
    [Unicode(false)]
    [Display(Name = "Mã sản phẩm")]
    public string ProductCode { get; set; } = null!;

    [StringLength(255)]
    [Display(Name = "Tên sản phẩm")]
    public string FullName { get; set; } = null!;

    [StringLength(500)]
    [Display(Name = "Mô tả")]
    public string? Description { get; set; }

    [StringLength(50)]
    [Display(Name = "Nhãn hàng")]
    public string? Brand { get; set; }

    [Column("CategoryID")]
    public int CategoryId { get; set; }

    [Column("SupplierID")]
    public int? SupplierId { get; set; }
    
    [MaxLength(255)]
    [Unicode(false)]
    [Display(Name = "Hình ảnh")]
    public string? Img { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("Products")]
    public virtual Category Category { get; set; } = null!;

    [InverseProperty("Product")]
    public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();

    [InverseProperty("Product")]
    public virtual ICollection<ProductDetail> ProductDetails { get; set; } = new List<ProductDetail>();

    [ForeignKey("SupplierId")]
    [InverseProperty("Products")]
    public virtual Supplier? Supplier { get; set; }
}
