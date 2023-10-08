using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Project1.Models;

public partial class PartsDepot
{
    [Key]
    public int PartId { get; set; }
    [MaxLength(40)]
    public string? PartName { get; set; }
    [MaxLength(80)]
    public string? PartDescription { get; set; }
    [Required]
    public int PurposeId { get; set; }

    public int? Quantity { get; set; }

    public int? ManuDepSeriesId { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [ForeignKey("ManuDepSeriesId")]
    public virtual ManufacturingDepartment? ManuDepSeries { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [ForeignKey("PurposeId")]
    public virtual Purpose Purpose { get; set; } = null!;
    [JsonIgnore]
    public virtual ICollection<EngineRoomDepartment> SeriesEngs { get; } = new List<EngineRoomDepartment>();
    [JsonIgnore]
    public virtual ICollection<PaintRoomDepartment> SeriesPtns { get; } = new List<PaintRoomDepartment>();
}
