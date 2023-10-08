using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Project1.Models;

public partial class BikeDepot
{
    [Key]
    public int BikeId { get; set; }
    [MaxLength(50)]
    public string? BikeName { get; set; }
    [MaxLength(80)]
    public string? BikeDescription { get; set; }

    public int? Quantity { get; set; }

    public DateTime? DateOfStore { get; set; }
    [Required]
    public int ManuDepSeriesId { get; set; }
    [Required]
    public int Price { get; set; }
    [JsonIgnore]
    public virtual ICollection<CustOrdersDepartment> CustOrdersDepartments { get; } = new List<CustOrdersDepartment>();
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [ForeignKey("ManuDepSeriesId")]
    public virtual ManufacturingDepartment? ManuDepSeries { get; set; }
}
