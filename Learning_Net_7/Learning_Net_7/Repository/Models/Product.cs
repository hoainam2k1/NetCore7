using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Learning_Net_7.Repository.Models;

[Table("Product")]
public partial class Product
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Code { get; set; } = null!;

    [StringLength(150)]
    [Unicode(false)]
    public string? Name { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? Price { get; set; }
}
