using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project1.Controllers;
using Project1.Models;
using Project1.Configurations;

namespace Project1.Controllers.Handlers
{
    [Route("register")] //localhost:5146/register
    public class RegisterHandler : ControllerBase
    {

        private readonly ModelContext db;

        public RegisterHandler(ModelContext dbContext)
        {
            db = dbContext;
        }
        //register
        [HttpPost]
        public async Task<IActionResult> AddCustomerData([FromBody] CustomerDatum customer_datas)
        {

            if (customer_datas == null)
            {
                return BadRequest();
            }
            else
            {
                if (customer_datas.CustomerName == null || customer_datas.CustomerLogin == null || customer_datas.CustomerPassword == null || customer_datas.CustomerSurrname == null || customer_datas.Estabilishment == null || customer_datas.Town == null || customer_datas.Street == null || customer_datas.Mail == null)
                {
                    return BadRequest();
                }
                else
                {
                    if(ModelState.IsValid)
                    {
                        try
                        {
                            if (await db.CustomerData.AnyAsync(b => b.CustomerLogin == customer_datas.CustomerLogin))
                            {
                                return Conflict();
                            }
                            else
                            {
                                customer_datas.CustomerPassword = BCrypt.Net.BCrypt.HashPassword(customer_datas.CustomerPassword);

                                await db.CustomerData.AddAsync(customer_datas);
                                await db.SaveChangesAsync();
                                return CreatedAtAction("GetAllCustomerDatas", customer_datas);
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
            }
            
        }
    }
}