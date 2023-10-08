using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project1.Models;
using System.Text.Json;

namespace Project1.Controllers
{

    [Route("[Controller]")]
    public class PaintRoomDepartmentController : ControllerBase
    {

        private readonly ModelContext db;

        public PaintRoomDepartmentController(ModelContext dbContext)
        {
            db = dbContext;
        }

        // METHODS
        private bool PaintRoomDepartmentExists(int id)
        {
            return db.PaintRoomDepartments.Any(p => p.SeriesPtnId == id);
        }

        // METHODS

        // GET METHOD TO ALL RECORDS
        [HttpGet]
        public async Task<ActionResult<List<PaintRoomDepartment>>> GetAllPaintRoomDepartment()
        {
            var paint = await db.PaintRoomDepartments.ToListAsync();
            return Ok(paint);
        }

        // GET METHOD TO SINGLE RECORD
        [HttpGet("{id}")]
        public async Task<ActionResult<PaintRoomDepartment>> GetPaintRoomDepartment(int id)
        {
            var paint = await db.PaintRoomDepartments.FindAsync(id);
            if (paint == null)
            {
                return NotFound();
            }
            return Ok(paint);
        }

        // POST TO NEW RECORD
        [HttpPost]
        public async Task<ActionResult> AddPaintRoomDepartment([FromBody] PaintRoomDepartment paint)
        {
            if (paint == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.PaintRoomDepartments.AnyAsync(b => b.SeriesPtnId == paint.SeriesPtnId))
                    {
                        return Conflict();
                    }
                    else
                    {
                        await db.PaintRoomDepartments.AddAsync(paint);
                        await db.SaveChangesAsync();
                        return CreatedAtAction(nameof(GetPaintRoomDepartment), new { id = paint.SeriesPtnId }, paint);
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
        public async Task<IActionResult> UpdatePaintRoomDepartment(int id, [FromBody] JsonElement json)
        {
            // Get EngineRoomDeps from db
            var paintRoomDepartment = await db.PaintRoomDepartments.FindAsync(id);

            if (paintRoomDepartment == null)
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
                    paintRoomDepartment.SeriesName = dict[key].GetString();
                }
                else if (key == "SeriesDescription")
                {
                    paintRoomDepartment.SeriesDescription = dict[key].GetString();
                }
                else if (key == "Quantity")
                {
                    paintRoomDepartment.Quantity = dict[key].GetInt32();
                }
                else if (key == "DateOfProduction")
                {
                    paintRoomDepartment.DateOfProduction = DateTime.Parse(dict[key].GetString());
                }
            }

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaintRoomDepartmentExists(id))
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
        public async Task<ActionResult> DeletePaintRoomDepartment(int id)
        {
            var paint = await db.PaintRoomDepartments.FindAsync(id);

            if (paint == null)
                {
                return BadRequest();
            }

            db.PaintRoomDepartments.Remove(paint);
            await db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteAllPaintRoomDepartment()
        {
            foreach (var c in await db.PaintRoomDepartments.ToListAsync())
            {
                await DeletePaintRoomDepartment(c.SeriesPtnId);
            }
            return NoContent();
        }

    }
}