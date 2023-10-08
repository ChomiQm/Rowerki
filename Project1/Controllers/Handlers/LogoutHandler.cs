using Azure;
using Microsoft.AspNetCore.Mvc;
using Project1.Controllers;
using Project1.Models;

namespace Project1.Controllers.Handlers
{
    [Route("logout")]
    public class LogoutHandler : ControllerBase
    {
        //LOGOUT http://localhost:5146/login/logout - ścieżka jeżeli logout znajdzie się w handlerze loginu z dopiskiem "logout" przy HttpGet
        private readonly ModelContext _db;
        public LogoutHandler(ModelContext db)
        {
            _db = db;
        }

        [HttpGet]
        public ActionResult<CustomerDatum> Logout()
        {
            HttpContext.Session.Remove("login");
            Response.Cookies.Delete("login");
            return Accepted();
        }
    }


}
