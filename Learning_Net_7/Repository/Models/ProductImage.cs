using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Learning_Net_7.Repository.Models;

[Table("ProductImage")]
public partial class ProductImage
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? ProductCode { get; set; }

    [Column("ProductImage", TypeName = "image")]
    public byte[]? ProductImg { get; set; }
}
