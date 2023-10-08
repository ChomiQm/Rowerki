using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project1.Models;
using System.IO;
using System.Text.Json;

namespace Project1.Controllers
{

    [Route("[Controller]")]
    public class PartsDepotController : ControllerBase
    {

        private readonly ModelContext db;

        public PartsDepotController(ModelContext dbContext)
        {
            db = dbContext;
        }

        // METHODS
        private bool PartsDepotExist(int id)
        {
            return db.PartsDepots.Any(b => b.PartId == id);
        }

        // METHODS

        // GET METHOD TO ALL RECORDS
        [HttpGet]
        public async Task<ActionResult<List<PartsDepot>>> GetAllPartsDepots()
        {
            var parts = await db.PartsDepots.ToListAsync();
            return Ok(parts);
        }

        // GET METHOD TO SINGLE RECORD
        [HttpGet("{id}")]
        public async Task<ActionResult<PartsDepot>> GetPartsDepot(int id)
        {
            var parts = await db.PartsDepots.FindAsync(id);
            if (parts == null)
            {
                return NotFound();
            }
            return Ok(parts);
        }

        // POST TO NEW RECORD
        [HttpPost]
        public async Task<ActionResult> AddPartsDepot([FromBody] PartsDepot parts)
        {
            if (parts == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.PartsDepots.AnyAsync(b => b.PartId == parts.PartId))
                    {
                        return Conflict();
                    }
                    else
                    {
                        await db.PartsDepots.AddAsync(parts);
                        await db.SaveChangesAsync();
                        return CreatedAtAction(nameof(GetPartsDepot), new { id = parts.PartId }, parts);
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
        public async Task<IActionResult> UpdatePartsDepot(int id, [FromBody] JsonElement json)
        {
            // Get BikeDepot from db
            var partsDepot = await db.PartsDepots.FindAsync(id);

            if (partsDepot == null)
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
                if (key == "PartName")
                {
                    partsDepot.PartName = dict[key].GetString();
                }
                else if (key == "PartDescription")
                {
                    partsDepot.PartDescription = dict[key].GetString();
                }
                else if (key == "PurposeId")
                {
                    partsDepot.PurposeId = dict[key].GetInt32();
                }
                else if (key == "Quantity")
                {
                    partsDepot.Quantity = dict[key].GetInt32();
                }
                else if (key == "ManuDepSeriesId")
                {
                    partsDepot.ManuDepSeriesId = dict[key].GetInt32();
                }
            }

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PartsDepotExist(id))
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
        public async Task<ActionResult> DeletePartsDepot(int id)
        {
            var parts = await db.PartsDepots.FindAsync(id);
            if (parts == null)
            {
                return BadRequest();
            }
            db.PartsDepots.Remove(parts);
            await db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteAllPartsDepots()
        {
            foreach (var c in await db.PartsDepots.ToListAsync())
            {
                await DeletePartsDepot(c.PartId);
            }
            return NoContent();
        }

    }
}