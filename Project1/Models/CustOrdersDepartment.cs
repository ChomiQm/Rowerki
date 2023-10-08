using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
    
namespace Project1.Models;

public partial class CustOrdersDepartment
{
    [Key]
    public Guid OrderId { get; set; }
    [Required]
    public int Total { get; set; }
    
    public Guid CustomerId { get; set; }

    public int BikeId { get; set; }
    public Guid ShipmentId { get; set; }

    public string? OStatus { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [ForeignKey("BikeId")]
    public virtual BikeDepot? Bike { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [ForeignKey("CustomerId")]
    public virtual CustomerDatum? Customer { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [ForeignKey("ShipmentId")]
    public virtual Shipment? Shipment { get; set; }
}
