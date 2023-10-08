using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
namespace Project1.Models;

public partial class Employee
{
    [Key]
    public int EmployeeId { get; set; }
    [Required]
    [MaxLength(20)]
    public string EmployeeName { get; set; } = null!;
    [MaxLength(20)]
    public string EmployeeSurrname { get; set; } = null!;
    [MaxLength(40)]
    public string? Town { get; set; }
    [MaxLength(50)]
    public string? Street { get; set; }

    public int? Phone { get; set; }
    [Required]
    public int PositionId { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [ForeignKey("PositionId")]
    public virtual Position? Position { get; set; }
}
