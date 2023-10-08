using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project1.Models;
using System.Text.Json;

namespace Project1.Controllers
{
    [Route("[Controller]")]
    public class ManufacturingDepartmentController : ControllerBase
    {

        private readonly ModelContext db;

        public ManufacturingDepartmentController(ModelContext dbContext)
        {
            db = dbContext;
        }

        // METHODS

        private bool ManufacturingDepartmentExists(int id)
        {
            return db.ManufacturingDepartments.Any(m => m.ManuSeriesId== id);
        }

        // METHODS

        // GET METHOD TO ALL RECORDS
        [HttpGet]
        public async Task<ActionResult<List<ManufacturingDepartment>>> GetAllManufacturingDepartment()
        {
            var manu = await db.ManufacturingDepartments.ToListAsync();
            return Ok(manu);
        }

        // GET METHOD TO SINGLE RECORD
        [HttpGet("{id}")]
        public async Task<ActionResult<ManufacturingDepartment>> GetManufacturingDepartment(int id)
        {
            var manu = await db.ManufacturingDepartments.FindAsync(id);
            if (manu == null)
            {
                return NotFound();
            }
            return Ok(manu);
        }

        // POST TO NEW RECORD
        [HttpPost]
        public async Task<ActionResult> AddManufacturingDepartment([FromBody] ManufacturingDepartment manu)
        {
            if (manu == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.ManufacturingDepartments.AnyAsync(b => b.ManuSeriesId == manu.ManuSeriesId))
                    {
                        return Conflict();
                    }
                    else
                    {
                        await db.ManufacturingDepartments.AddAsync(manu);
                        await db.SaveChangesAsync();
                        return CreatedAtAction(nameof(GetManufacturingDepartment), new { id = manu.ManuSeriesId }, manu);
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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateManufacturingDepartment(int id, [FromBody] JsonElement json)
        {
            // Get manu dep
            var manuDep = await db.ManufacturingDepartments.FindAsync(id);

            if (manuDep == null)
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
                if (key == "Quantity")
                {
                    manuDep.Quantity = dict[key].GetInt32();
                }
                else if (key == "DateOfProduction")
                {
                    manuDep.DateOfProduction = DateTime.Parse(dict[key].GetString());
                }
            }

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ManufacturingDepartmentExists(id))
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
        public async Task<ActionResult> DeleteManufacturingDepartment(int id)
        {
            var manu = await db.ManufacturingDepartments.FindAsync(id);
            if (manu == null)
            {
                return BadRequest();
            }
            db.ManufacturingDepartments.Remove(manu);
            await db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteAllEManufacturingDepartment()
        {
            foreach (var c in await db.ManufacturingDepartments.ToListAsync())
            {
                await DeleteManufacturingDepartment(c.ManuSeriesId);
            }
            return NoContent();
        }

    }
}