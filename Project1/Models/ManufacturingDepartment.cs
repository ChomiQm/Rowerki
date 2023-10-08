using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Project1.Models;

public partial class ManufacturingDepartment
{
    [Key]
    public int ManuSeriesId { get; set; }

    public int? Quantity { get; set; }

    public DateTime? DateOfProduction { get; set; }
    [JsonIgnore]
    public virtual ICollection<BikeDepot> BikeDepots { get; } = new List<BikeDepot>();
    [JsonIgnore]
    public virtual ICollection<PartsDepot> PartsDepots { get; } = new List<PartsDepot>();
}
