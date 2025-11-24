using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SportStore.Models;

[Table("Branch")]
[Index("BranchCode", Name = "UQ__Branch__1C61B8889790C753", IsUnique = true)]
public partial class Branch
{
    [Key]
    [Column("BranchID")]
    public int BranchId { get; set; }

    [StringLength(30)]
    public string? BranchCode { get; set; }

    [StringLength(255)]
    public string? FullName { get; set; }

    [StringLength(255)]
    public string? Adress { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string? Phonenumber { get; set; }

    [InverseProperty("Branch")]
    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
