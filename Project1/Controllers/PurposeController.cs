using Azure.Core.GeoJson;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project1.Models;
using System.Text.Json;

namespace Project1.Controllers
{
    [Route("[Controller]")]
    public class PurposeController : ControllerBase
    {

        private readonly ModelContext db;

        public PurposeController(ModelContext dbContext)
        {
            db = dbContext;
        }

        // METHODS
        private bool PurposeExists(int id)
        {
            return db.Purposes.Any(b => b.PurposeId == id);
        }

        // METHODS


        // GET METHOD TO ALL RECORDS
        [HttpGet]
        public async Task<ActionResult<List<Purpose>>> GetAllPurpose()
        {
            var purp = await db.Purposes.ToListAsync();
            return Ok(purp);
        }

        // GET METHOD TO SINGLE RECORD
        [HttpGet("{id}")]
        public async Task<ActionResult<Purpose>> GetPurpose(int id)
        {
            var purp = await db.Purposes.FindAsync(id);
            if (purp == null)
            {
                return NotFound();
            }
            return Ok(purp);
        }

        // POST TO NEW RECORD
        [HttpPost]
        public async Task<ActionResult> AddPurpose([FromBody] Purpose purp)
        {
            if (purp == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.Purposes.AnyAsync(b => b.PurposeId == purp.PurposeId))
                    {
                        return Conflict();
                    }
                    else
                    {
                        await db.Purposes.AddAsync(purp);
                        await db.SaveChangesAsync();
                        return CreatedAtAction(nameof(GetPurpose), new { id = purp.PurposeId }, purp);
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
        public async Task<IActionResult> UpdatePurpose(int id, [FromBody] JsonElement json)
        {
            // Get BikeDepot from db
            var purp = await db.Purposes.FindAsync(id);

            if (purp == null)
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
                if (key == "PurposeName")
                {
                    purp.PurposeName = dict[key].GetString();
                }
            }

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PurposeExists(id))
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
        public async Task<ActionResult> DeletePurpose(int id)
        {
            var purp = await db.Purposes.FindAsync(id);

            if (purp == null)
            {
                return BadRequest();
            }

            db.Purposes.Remove(purp);
            await db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteAllPurpose()
        {
            foreach (var c in await db.Purposes.ToListAsync())
            {
                await DeletePurpose(c.PurposeId);
            }
            return NoContent();
        }

    }
}