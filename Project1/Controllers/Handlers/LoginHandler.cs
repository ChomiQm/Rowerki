using Microsoft.AspNetCore.Mvc;
using Project1.Controllers;
using Project1.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Project1.Configurations;

namespace Project1.Controllers.Handlers
{
    [Route("login")]
    public class LoginHandler : ControllerBase
    {
        private readonly ModelContext _db;
        //login
        public LoginHandler(ModelContext db)
        {
            _db = db;
        }

        //login
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] CustomerDatum customer_datas)
        {
            //get all customer data from database
            var customerDatas = await _db.CustomerData.ToListAsync();

            if (customerDatas == null)
            {
                return BadRequest("No customer data found");
            }

            //get the customer data with the matching login
            var loginData = customerDatas.SingleOrDefault(s => s.CustomerLogin == customer_datas.CustomerLogin);
            if (loginData == null)
            {
                return NotFound("No matching login data found");
            }

            //hash the input password
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(customer_datas.CustomerPassword);

            //check if the input password matches the hashed password stored in the database
            if (BCrypt.Net.BCrypt.Verify(customer_datas.CustomerPassword, loginData.CustomerPassword))
            {
                //set session variables for the login and customer id
                HttpContext.Session.SetString("login", customer_datas.CustomerLogin);
                HttpContext.Session.SetString("id", loginData.CustomerId.ToString());

                //create a cookie for the login
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddSeconds(60),
                    Path = "/"
                };
                Response.Cookies.Append("login", customer_datas.CustomerLogin, cookieOptions);

                return Accepted();
            }
            else
            {
                return BadRequest("Incorrect password");
            }
        }

    }
}