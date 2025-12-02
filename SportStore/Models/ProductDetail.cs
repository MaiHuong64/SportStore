using System;
using System.Collections.Generic;

namespace SportStore.Models;

public partial class ProductDetail
{
    public int ProductDetailId { get; set; }

    public int ProductId { get; set; }

    public string? Sku { get; set; }

    public decimal Price { get; set; }

    public byte? Size { get; set; }

    public string? Color { get; set; }

    public int? Quantity { get; set; }
}
