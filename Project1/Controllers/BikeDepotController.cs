using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Project1.Models;
using System.Text.Json;

namespace Project1.Controllers
{
    [Route("[Controller]")] //Deletes "controller" pharse from model
    [ApiController]
    public class BikeDepotController : ControllerBase
    {

        private readonly ModelContext db;

        public BikeDepotController(ModelContext dbContext)
        {
            db = dbContext;
        }

        // METHODS
        private bool BikeDepotExists(int id)
        {
            return db.BikeDepots.Any(b => b.BikeId == id);
        }

        // METHODS

        // GET METHOD TO ALL RECORDS
        [HttpGet]
        public async Task<ActionResult<List<BikeDepot>>> GetAllBikeDepots()
        {
            var bike_depots = await db.BikeDepots.ToListAsync();
            return Ok(bike_depots);
        }

        // GET METHOD TO SINGLE RECORD
        [HttpGet("{id}")]
        public async Task<ActionResult<BikeDepot>> GetBikeDepot(int id)
        {
            var bike_depots = await db.BikeDepots.FindAsync(id);
            if (bike_depots == null)
            {
                return NotFound();
            }
            return Ok(bike_depots);
        }

        // POST TO NEW RECORD
        [HttpPost]
        public async Task<ActionResult> AddBikeDepot([FromBody] BikeDepot bikeDepot)
        {
            if (bikeDepot== null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.BikeDepots.AnyAsync(b => b.BikeName == bikeDepot.BikeName))
                    {
                        return Conflict();
                    }
                    else
                    {
                        await db.BikeDepots.AddAsync(bikeDepot);
                        await db.SaveChangesAsync();
                        return CreatedAtAction(nameof(GetBikeDepot), new { id = bikeDepot.BikeId }, await db.BikeDepots.FindAsync(bikeDepot.BikeId));
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
        public async Task<IActionResult> UpdateBikeDepot(int id, [FromBody] JsonElement json)
        {
            // Get BikeDepot from db
            var bikeDepot = await db.BikeDepots.FindAsync(id);

            if (bikeDepot == null)
            {
                return NotFound();
            }

            if(json.ValueKind == JsonValueKind.Null)
            {
                return BadRequest(ModelState);  
            }


            // JSON Serializer.Deserialize
            var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json.ToString());

            // Updating cols
            foreach (var key in dict.Keys)
            {
                if (key == "BikeName")
                {
                    bikeDepot.BikeName = dict[key].GetString();
                }
                else if (key == "BikeDescription")
                {
                    bikeDepot.BikeDescription = dict[key].GetString();
                }
                else if (key == "Quantity")
                {
                    bikeDepot.Quantity = dict[key].GetInt32();
                }
                else if (key == "DateOfStore")
                {
                    bikeDepot.DateOfStore = DateTime.Parse(dict[key].GetString());
                }
                else if (key == "ManuDepSeriesId")
                {
                    bikeDepot.ManuDepSeriesId = dict[key].GetInt32();
                }
                else if (key == "Price")
                {
                    bikeDepot.Price = dict[key].GetInt32();
                }
            }

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BikeDepotExists(id))
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
        public async Task<ActionResult> DeleteBikeDepot(int id)
        {
            var bike_depots = await db.BikeDepots.FindAsync(id);
            if (bike_depots == null)
            {
                return BadRequest();
            }
            db.BikeDepots.Remove(bike_depots);
            await db.SaveChangesAsync();
            return NoContent();

        }

        [HttpDelete]
        public async Task<ActionResult> DeleteAllBikeDepots()
        {
            foreach (var c in await db.BikeDepots.ToListAsync())
            {
                await DeleteBikeDepot(c.BikeId);
            }
            return NoContent();
        }

    }
}
