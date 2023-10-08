using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project1.Models;
using Project1.Configurations;
using System.Text.Json;

namespace Project1.Controllers
{

    [Route("[Controller]")]
    public class CustomerDatumController : ControllerBase
    {

        private readonly ModelContext db;

        public CustomerDatumController(ModelContext dbContext)
        {
            db = dbContext;
        }

        // METHODS

        private bool CustomerExists(Guid id)
        {
            return db.CustomerData.Any(e => e.CustomerId == id);
        }

        // METHODS

        // GET METHOD TO ALL RECORDS
        [HttpGet]
        public async Task<ActionResult<List<CustomerDatum>>> GetAllCustomerDatas()
        {
            var customer_datas = await db.CustomerData.ToListAsync();
            return Ok(customer_datas);
        }

        // GET METHOD TO SINGLE RECORD
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerDatum>> GetCustomerData(Guid id)
        {
            var customer_datas = await db.CustomerData.FindAsync(id);
            if (customer_datas == null)
            {
                return NotFound();
            }
            return Ok(customer_datas);
        }

        [HttpGet("Data")]
        public async Task<ActionResult<CustomerDatum>> GetCustomerDataByLogin()
        {
            string? login = HttpContext?.User?.Identity?.Name;
            if (login == null)
            {
                return BadRequest();
            }
            else
            {
                var customerData = await db.CustomerData.FirstOrDefaultAsync(cd => cd.CustomerLogin == login);
                if (customerData == null)
                {
                    return NotFound();
                }
                return RedirectToAction("GetCustomerData{customerData.CustomerId}");
            }
        }

        // PUT UPDATES RECORD
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(Guid id, [FromBody] JsonElement json)
        {
            // Pobierz CustomerDatum z bazy danych
            var customer = await db.CustomerData.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            if (json.ValueKind == JsonValueKind.Null)
            {
                return BadRequest(ModelState);
            }

            // Deserializuj JSON do słownika
            var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json.ToString());

            // Sprawdź, które pola mają być zmienione
            foreach (var key in dict.Keys)
            {
                if (key == "CustomerName")
                {
                    customer.CustomerName = dict[key].GetString();
                }
                else if (key == "CustomerSurrname")
                {
                    customer.CustomerSurrname = dict[key].GetString();
                }
                else if (key == "Town")
                {
                    customer.Town = dict[key].GetString();
                }
                else if (key == "Street")
                {
                    customer.Street = dict[key].GetString();
                }
                else if (key == "Estabilishment")
                {
                    customer.Estabilishment = dict[key].GetString();
                }
                else if (key == "Mail")
                {
                    customer.Mail = dict[key].GetString();
                }
                else if (key == "DateOfFirstBuy")
                {
                    customer.DateOfFirstBuy = DateTime.Parse(dict[key].GetString());
                }
                else if (key == "CustomerLogin")
                {
                    customer.CustomerLogin = dict[key].GetString();
                }
                else if (key == "CustomerPassword")
                {
                    customer.CustomerPassword = dict[key].GetString();
                }
            }

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
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
        public async Task<ActionResult> DeleteCustomerData(Guid id)
        {
            var customer_datas = await db.CustomerData.FindAsync(id);
            if (customer_datas == null)
            {
                return BadRequest();
            }
            db.CustomerData.Remove(customer_datas);
            await db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteAllCustomerData()
        {
            foreach (var c in await db.CustomerData.ToListAsync())
            {
                await DeleteCustomerData(c.CustomerId);
            }
            return NoContent();
        }

    }

}