using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SportStore.Models;

[Table("Category")]
public partial class Category
{
    [Key]
    [Column("CategoryID")]
    public int CategoryId { get; set; }

    [StringLength(30)]
    [Unicode(false)]
    [Display(Name = "Mã danh mục")]
    public string? CategoryCode { get; set; }

    [StringLength(255)]
    [Display(Name = "Tên danh mục")]
    public string? FullName { get; set; }

    [StringLength(255)]
    [Display(Name = "Mô tả")]
    public string? Description { get; set; }

    [InverseProperty("Category")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
