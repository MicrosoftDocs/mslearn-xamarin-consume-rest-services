using System;
using Microsoft.AspNetCore.Mvc;
using BookService.Models;

namespace BookService.Controllers
{
    [Route("api/books/[controller]")]
    public class LoginController : Controller
    {
        
        [HttpGet]
        public ActionResult Get()
        {
            try
            {
                var authorizationToken = Guid.NewGuid().ToString();

                BookFactory.Initialize(authorizationToken);
                
                return Json(authorizationToken);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }
    }
}
