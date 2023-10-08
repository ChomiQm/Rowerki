using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project1.Models;
using System.Text.Json;

namespace Project1.Controllers
{
    [Route("[Controller]")]
    public class EmployeeController : ControllerBase
    {

        private readonly ModelContext db;

        public EmployeeController(ModelContext dbContext)
        {
            db = dbContext;
        }

        // METHODS
        private bool EmployeeExists(int id)
        {
            return db.Employees.Any(e => e.EmployeeId == id);
        }

        // METHODS

        // GET METHOD TO ALL RECORDS
        [HttpGet]
        public async Task<ActionResult<List<Employee>>> GetAllEmployees()
        {
            var employees = await db.Employees.ToListAsync();
            return Ok(employees);
        }

        // GET METHOD TO SINGLE RECORD
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var employees = await db.Employees.FindAsync(id);
            if (employees == null)
            {
                return NotFound();
            }
            return Ok(employees);
        }

        // POST TO NEW RECORD
        [HttpPost]
        public async Task<ActionResult> AddEmployee([FromBody] Employee employees)
        {
            if (employees == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.Employees.AnyAsync(b => b.EmployeeId == employees.EmployeeId))
                    {
                        return Conflict();
                    }
                    else
                    {
                        await db.Employees.AddAsync(employees);
                        await db.SaveChangesAsync();
                        return CreatedAtAction(nameof(GetEmployee), new { id = employees.EmployeeId }, employees);
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
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] JsonElement json)
        {
            // get Employees from db
            var employee = await db.Employees.FindAsync(id);

            if (employee == null) 
            {
                return NotFound();
            }

            if (json.ValueKind == JsonValueKind.Null)
            {
                return BadRequest(ModelState);
            }

            //JSON Serialize
            var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json.ToString());

            //Update cols
            foreach (var key in dict.Keys)
            {
                if (key == "EmployeeName")
                {
                    employee.EmployeeName = dict[key].GetString();
                }
                else if (key == "EmployeeSurrname")
                {
                    employee.EmployeeSurrname = dict[key].GetString();
                }
                else if (key == "Town")
                {
                    employee.Town = dict[key].GetString();
                }
                else if (key == "Street")
                {
                    employee.Street = dict[key].GetString();
                }
                else if (key == "Phone")
                {
                    employee.Phone = dict[key].GetInt32();
                }
                else if (key == "PositionId")
                {
                    employee.PositionId = dict[key].GetInt32();
                }
            }

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
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
        public async Task<ActionResult> DeleteEmployee(int id)
        {
            var employees = await db.Employees.FindAsync(id);
            if (employees == null)
            {
                return BadRequest();
            }
            db.Employees.Remove(employees);
            await db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteAllEmployee()
        {
            foreach (var c in await db.Employees.ToListAsync())
            {
                await DeleteEmployee(c.EmployeeId);
            }
            return NoContent();
        }

    }
}
