using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
namespace Project1.Models;

public partial class EngineRoomDepartment
{
    [Key]
    public int SeriesEngId { get; set; }
    [Required]
    [MaxLength(40)]
    public string SeriesName { get; set; } = null!;
    [MaxLength(80)]
    public string? SeriesDescription { get; set; }
    [Required]
    public int IdEngineroom { get; set; }

    public int? Quantity { get; set; }
    public DateTime? DateOfProduction { get; set; }
    [JsonIgnore]
    public virtual ICollection<PartsDepot> Parts { get; } = new List<PartsDepot>();
}
