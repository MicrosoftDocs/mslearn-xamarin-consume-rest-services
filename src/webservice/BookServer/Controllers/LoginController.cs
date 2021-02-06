using System;
using Microsoft.AspNetCore.Mvc;
using BookService.Models;

namespace BookService.Controllers
{
    [ApiController]
    [Route("api/books/[controller]")]
    public class LoginController : ControllerBase
    {
        
        [HttpGet]
        public ActionResult Get()
        {
            try
            {
                var authorizationToken = Guid.NewGuid().ToString();

                BookFactory.Initialize(authorizationToken);
                
                return new JsonResult(authorizationToken);
            }
            catch (Exception ex)
            {
                return new JsonResult(ex.Message);
            }
        }
    }
}
