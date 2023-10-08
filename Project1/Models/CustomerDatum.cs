using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Project1.Models;

public partial class CustomerDatum
{
    [Key]
    public Guid CustomerId { get; set; }
    [Required]
    [MaxLength(30)]
    public string? CustomerName { get; set; }
    [Required]
    [MaxLength(30)]
    public string? CustomerSurrname { get; set; }
    [Required]
    [MaxLength(40)]
    public string? Town { get; set; }
    [Required]
    [MaxLength(50)]
    public string? Street { get; set; }
    [Required]
    [MaxLength(60)]
    public string? Estabilishment { get; set; }
    [Required]
    [MaxLength(60)]
    public string? Mail { get; set; }

    public DateTime? DateOfFirstBuy { get; set; }
    [Required]
    [MaxLength(20)]
    public string? CustomerLogin { get; set; }
    [Required]
    [MaxLength(64)]
    public string? CustomerPassword { get; set; }
    [JsonIgnore]
    public virtual ICollection<CustOrdersDepartment> CustOrdersDepartments { get; } = new List<CustOrdersDepartment>();
}
