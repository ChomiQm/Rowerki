using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project1.Models;
using System.IO;
using System.Text.Json;

namespace Project1.Controllers
{

    [Route("[Controller]")]
    public class PositionController : ControllerBase
    {

        private readonly ModelContext db;

        public PositionController(ModelContext dbContext)
        {
            db = dbContext;
        }

        // METHODS
        private bool PositionExists(int id)
        {
            return db.Positions.Any(b => b.PositionId == id);
        }

        // METHODS

        // GET METHOD TO ALL RECORDS
        [HttpGet]
        public async Task<ActionResult<List<Position>>> GetAllPosition()
        {
            var pos = await db.Positions.ToListAsync();
            return Ok(pos);
        }

        // GET METHOD TO SINGLE RECORD
        [HttpGet("{id}")]
        public async Task<ActionResult<Position>> GetPosition(int id)
        {
            var pos = await db.Positions.FindAsync(id);
            if (pos == null)
            {
                return NotFound();
            }
            return Ok(pos);
        }

        // POST TO NEW RECORD
        [HttpPost]
        public async Task<ActionResult> AddPosition([FromBody] Position pos)
        {
            if (pos == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.Positions.AnyAsync(b => b.PositionId == pos.PositionId))
                    {
                        return Conflict();
                    }
                    else
                    {
                        await db.Positions.AddAsync(pos);
                        await db.SaveChangesAsync();
                        return CreatedAtAction(nameof(GetPosition), new { id = pos.PositionId }, pos);
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
        public async Task<IActionResult> UpdatePosition(int id, [FromBody] JsonElement json)
        {
            // Get BikeDepot from db
            var positions = await db.Positions.FindAsync(id);

            if (positions == null)
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
                if (key == "PositionName")
                {
                    positions.PositionName = dict[key].GetString();
                }
                else if (key == "Salary")
                {
                    positions.Salary = dict[key].GetDecimal();
                }
                else if (key == "PositionDescription")
                {
                    positions.PositionDescription = dict[key].GetString();
                }
            }

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PositionExists(id))
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
        public async Task<ActionResult> DeletePosition(int id)
        {
            var pos = await db.Positions.FindAsync(id);
            if (pos == null)
            {
                return BadRequest();
            }
            db.Positions.Remove(pos);
            await db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteAllPosition()
        {
            foreach (var c in await db.Positions.ToListAsync())
            {
                await DeletePosition(c.PositionId);
            }
            return NoContent();
        }

    }
}