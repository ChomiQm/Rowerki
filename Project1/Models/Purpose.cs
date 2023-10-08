using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Project1.Models;

public partial class Purpose
{
    [Key]
    public int PurposeId { get; set; }
    [MaxLength(40)]
    public string? PurposeName { get; set; }
    [JsonIgnore]
    public virtual ICollection<PartsDepot> PartsDepots { get; } = new List<PartsDepot>();
}
