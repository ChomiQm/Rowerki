using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project1.Models;

namespace Project1.Controllers
{
    [Route("[Controller]")]
    public class CustOrdersDepartmentController : ControllerBase
    {

        private readonly ModelContext db;

        public CustOrdersDepartmentController(ModelContext dbContext)
        {
            db = dbContext;
        }

        // GET METHOD TO ALL RECORDS
        [HttpGet]
        public async Task<ActionResult<List<CustOrdersDepartment>>> GetAllCustOrdersDepartment()
        {
            var custOD = await db.CustOrdersDepartments.ToListAsync();
            return Ok(custOD);
        }

        // GET METHOD TO SINGLE RECORD
        [HttpGet("{id}")]
        public async Task<ActionResult<CustOrdersDepartment>> GetCustOrdersDepartment(Guid id)
        {
            var custOD = await db.CustOrdersDepartments.FindAsync(id);
            if (custOD == null)
            {
                return NotFound();
            }
            return Ok(custOD);
        }

        // DELETE METHOD
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCustOrdersDepartment(Guid id)
        {
            var custOD = await db.CustOrdersDepartments.FindAsync(id);
            if (custOD == null)
            {
                return BadRequest();
            }
            db.CustOrdersDepartments.Remove(custOD);
            await db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteAllCustOrdersDepartment()
        {
            foreach (var c in await db.CustOrdersDepartments.ToListAsync())
            {
                await DeleteCustOrdersDepartment(c.OrderId);
            }

            return NoContent();
        }


    }
}