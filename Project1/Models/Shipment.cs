using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Project1.Models;

public partial class Shipment
{
    [Key]
    public Guid ShipmentId { get; set; }
    [MaxLength(40)]
    public string? Destination { get; set; }

    public DateTime? DateOfSent { get; set; }

    public string? DeliveryType { get; set; }

    public DateTime? DateOfReceipt { get; set; }
    [JsonIgnore]
    public virtual ICollection<CustOrdersDepartment> CustOrdersDepartments { get; } = new List<CustOrdersDepartment>();
}
