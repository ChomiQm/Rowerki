using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project1.Models;
using System.Text.Json;

namespace Project1.Controllers
{
    [Route("[Controller]")]
    public class EngineRoomDepartmentController : ControllerBase
    {

        private readonly ModelContext db;

        public EngineRoomDepartmentController(ModelContext dbContext)
        {
            db = dbContext;
        }

        // METHODS
        private bool EngineRoomDepartmentExists(int id)
        {
            return db.EngineRoomDepartments.Any(e => e.SeriesEngId == id);
        }

        // METHODS

        // GET METHOD TO ALL RECORDS
        [HttpGet]
        public async Task<ActionResult<List<EngineRoomDepartment>>> GetAllEngineRoomDepartment()
        {
            var engrd = await db.EngineRoomDepartments.ToListAsync();
            return Ok(engrd);
        }

        // GET METHOD TO SINGLE RECORD
        [HttpGet("{id}")]
        public async Task<ActionResult<EngineRoomDepartment>> GetEngineRoomDepartment(int id)
        {
            var engrd = await db.EngineRoomDepartments.FindAsync(id);
            if (engrd == null)
            {
                return NotFound();
            }
            return Ok(engrd);
        }

        // POST TO NEW RECORD
        [HttpPost]
        public async Task<ActionResult> AddEngineRoomDepartment([FromBody] EngineRoomDepartment engrd)
        {
            if (engrd == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.EngineRoomDepartments.AnyAsync(b => b.SeriesEngId == engrd.SeriesEngId))
                    {
                        return Conflict();
                    }
                    else
                    {
                        await db.EngineRoomDepartments.AddAsync(engrd);
                        await db.SaveChangesAsync();
                        return CreatedAtAction(nameof(GetEngineRoomDepartment), new { id = engrd.SeriesEngId }, engrd);
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
        public async Task<IActionResult> UpdateEngineRoomDepartment(int id, [FromBody] JsonElement json)
        {
            // Get EngineRoomDeps from db
            var engineRoomDepartment = await db.EngineRoomDepartments.FindAsync(id);

            if (engineRoomDepartment == null)
            {
                return NotFound();
            }

            if (json.ValueKind == JsonValueKind.Null)
            {
                return BadRequest(ModelState);
            }

            // JSON Serializer.Deserialize
            var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json.ToString());

            // Update cols
            foreach (var key in dict.Keys)
            {
                if (key == "SeriesName")
                {
                    engineRoomDepartment.SeriesName = dict[key].GetString();
                }
                else if (key == "SeriesDescription")
                {
                    engineRoomDepartment.SeriesDescription = dict[key].GetString();
                }
                else if (key == "Quantity")
                {
                    engineRoomDepartment.Quantity = dict[key].GetInt32();
                }
                else if (key == "DateOfProduction")
                {
                    engineRoomDepartment.DateOfProduction = DateTime.Parse(dict[key].GetString());
                }
            }

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EngineRoomDepartmentExists(id))
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
        public async Task<ActionResult> DeleteEngineRoomDepartment(int id)
        {
            var engrd = await db.EngineRoomDepartments.FindAsync(id);
            if (engrd == null)
            {
                return BadRequest();
            }
            db.EngineRoomDepartments.Remove(engrd);
            await db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteAllEngineRoomDepartment()
        {
            foreach (var c in await db.EngineRoomDepartments.ToListAsync())
            {
                await DeleteEngineRoomDepartment(c.SeriesEngId);
            }
            return NoContent();
        }

    }
}