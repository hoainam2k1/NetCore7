using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Learning_Net_7.Repository.Models;

[Table("RefreshToken")]
public partial class RefreshToken
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string UserId { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string? TokenId { get; set; }

    [Column("RefreshToken")]
    [StringLength(50)]
    [Unicode(false)]
    public string? RefreshTokens { get; set; }
}
