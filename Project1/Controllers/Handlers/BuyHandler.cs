using Microsoft.AspNetCore.Mvc;
using Project1.Controllers;
using Microsoft.EntityFrameworkCore;
using Project1.Models;
using System;
using System.Web;
using Microsoft.AspNetCore.Http;
using System.Reflection.Metadata.Ecma335;
using System.Xml.Schema;
using Project1.DTO;
using System.Text.Json;

namespace Project1.Controllers.Handlers
{
    [Route("buy")] //localhost:5146/buy
    public class BuyHandler : CustOrdersDepartmentController
    {
        private readonly ModelContext db;

        public BuyHandler(ModelContext dbContext) : base(dbContext)
        {
            db = dbContext;
        }

        [HttpPost]
        public async Task<ActionResult> Buy([FromBody] OrderDTO order)
        {
            var bikeController = new BikeDepotController(db);
            var custDataController = new CustomerDatumController(db);

            // login user check
            var login = HttpContext.Session.GetString("login");

            if (order == null)
            {
                return BadRequest();
            }

            else
            {
                if (string.IsNullOrEmpty(login))
                {
                    return BadRequest("Login not found");
                }

                // catch user using login session
                var customer = await db.CustomerData.FirstOrDefaultAsync(c => c.CustomerLogin == login);
                if (customer == null)
                {
                    return BadRequest("User not found.");
                }

                // catch bike using BikeId 
                var bike = await bikeController.GetBikeDepot(order.BikeId);

                if (bike.Result == NotFound())
                {
                    return BadRequest("Bike not found.");
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        var shipment = new Shipment
                        {
                            ShipmentId = Guid.NewGuid(),
                            Destination = order.ShipmentDestination,
                            DateOfSent = DateTime.Now,
                            DeliveryType = order.ShipmentType
                        };

                        if (await db.Shipments.AnyAsync(b => b.ShipmentId == shipment.ShipmentId))
                        {
                            return Conflict();
                        }

                        db.Shipments.Add(shipment);
                        await db.SaveChangesAsync();

                        // New order to db
                        var custOD = new CustOrdersDepartment
                        {
                            OrderId = Guid.NewGuid(),
                            Total = order.Total,
                            BikeId = bike.Value.BikeId,
                            CustomerId = Guid.Parse(HttpContext.Session.GetString("id")),
                            OStatus = "Pending",
                            ShipmentId = shipment.ShipmentId
                        };

                        db.CustOrdersDepartments.Add(custOD);
                        await db.SaveChangesAsync();
                        return CreatedAtAction(nameof(GetCustOrdersDepartment), new { id = custOD.OrderId }, custOD);
                    }
                    catch (DbUpdateException ex)
                    {
                        return BadRequest($"Failed to save changes to the database. {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        return BadRequest($"An error occurred while processing your request. {ex.Message}");
                    }


                }
                else
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors)
                                        .Select(e => e.ErrorMessage)
                                        .ToList();
                    return BadRequest(new { Errors = errors });
                }

            }
           

        }

        [HttpPut]
        public async Task<ActionResult> UpdateStatus(Guid id, [FromBody] JsonElement json)
        {
            var custOD = await db.CustOrdersDepartments.FindAsync(id);

            if (custOD == null)
            {
                return BadRequest();
            }

            if(json.ValueKind== JsonValueKind.Null)
            {
                return BadRequest();
            }

            var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json.ToString());

            foreach (var key in dict.Keys)
            {
                if (key == "OStatus")
                {
                    custOD.OStatus = "Delivered";
                }
            }

            await db.SaveChangesAsync();
            return Ok();
        }
    }
}