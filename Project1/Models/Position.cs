using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Project1.Models;

public partial class Position
{
    [Key]
    public int PositionId { get; set; }
    [Required]
    [MaxLength(20)]
    public string PositionName { get; set; } = null!;

    public decimal? Salary { get; set; }
    [MaxLength(80)]
    public string? PositionDescription { get; set; }
    [JsonIgnore]
    public virtual ICollection<Employee> Employees { get; } = new List<Employee>();
}
