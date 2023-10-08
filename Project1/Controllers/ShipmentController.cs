using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project1.Models;
using System.Text.Json;

namespace Project1.Controllers
{
    [Route("[Controller]")]
    public class ShipmentController : ControllerBase
    {
        private readonly ModelContext db;

        public ShipmentController(ModelContext dbContext)
        {
            db = dbContext;
        }

        // METHODS
        private bool ShipmentExists(Guid id)
        {
            return db.Shipments.Any(s => s.ShipmentId == id);
        }

        // METHODS

        // GET METHOD TO ALL RECORDS
        [HttpGet]
        public async Task<ActionResult<List<Shipment>>> GetAllShipments()
        {
            var shipment = await db.Shipments.ToListAsync();
            return Ok(shipment);
        }

        // GET METHOD TO SINGLE RECORD
        [HttpGet("{id}")]
        public async Task<ActionResult<Shipment>> GetShipment(Guid id)
        {
            var shipment = await db.Shipments.FindAsync(id);
            if (shipment == null)
            {
                return NotFound();
            }
            return Ok(shipment);
        }

        // POST TO NEW RECORD
        [HttpPost]
        public async Task<ActionResult> AddShipment([FromBody] Shipment shipment)
        {
            if (shipment == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.Shipments.AnyAsync(b => b.ShipmentId == shipment.ShipmentId))
                    {
                        return Conflict();
                    }
                    else
                    {
                        await db.Shipments.AddAsync(shipment);
                        await db.SaveChangesAsync();
                        return CreatedAtAction(nameof(GetShipment), new { id = shipment.ShipmentId }, shipment);
                    }

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

        // PUT UPDATES RECORD
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateShipment(Guid id, [FromBody] JsonElement json)
        {
            // Get BikeDepot from db
            var shipment = await db.Shipments.FindAsync(id);

            if (shipment == null)
            {
                return NotFound();
            }

            if (json.ValueKind == JsonValueKind.Null)
            {
                return BadRequest(ModelState);
            }

            // JSON Serializer.Deserialize
            var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json.ToString());

            // Updating cols
            foreach (var key in dict.Keys)
            {
                if (key == "Destination")
                {
                    shipment.Destination = dict[key].GetString();
                }
                else if (key == "DeliveryType")
                {
                    shipment.DeliveryType = dict[key].GetString();
                }
                else if (key == "DateOfSent")
                {
                    shipment.DateOfSent = DateTime.Parse(dict[key].GetString());
                }
                else if (key == "DateOfReceipt")
                {
                    shipment.DateOfReceipt = DateTime.Parse(dict[key].GetString());
                }
            }

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShipmentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE METHOD
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteShipment(Guid id)
        {
            var shipment = await db.Shipments.FindAsync(id);
            if (shipment == null)
            {
                return BadRequest();
            }
            db.Shipments.Remove(shipment);
            await db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteAllShipments()
        {
            foreach (var c in await db.Shipments.ToListAsync())
            {
                await DeleteShipment(c.ShipmentId);
            }
            return NoContent();
        }

    }
}