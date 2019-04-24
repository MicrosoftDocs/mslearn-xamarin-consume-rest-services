using System;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Collections.Generic;
using BookService.Models;

namespace BookService.Controllers
{
    [Route("api/[controller]")]
    public class BaseController : Controller
    {
        protected List<Book> UserBooks
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.AuthorizationToken))
                {
                    return null;
                }
                
                if (!BookFactory.Books.ContainsKey(this.AuthorizationToken))
                {
                    return null;
                }

                var result = BookFactory.Books[this.AuthorizationToken];

                return result.Item2;
            }
        }
        protected bool CheckAuthorization()
        {
            BookFactory.ClearStaleData();

            try
            {
                var ctx = HttpContext;
                if (ctx != null)
                {
                    if (string.IsNullOrWhiteSpace(this.AuthorizationToken))
                    {
                        ctx.Response.StatusCode = (int) HttpStatusCode.Forbidden;
                        return false;
                    }
                }
                else
                {
                    return false;
                }

                if (!BookFactory.Books.ContainsKey(this.AuthorizationToken))
                {
                    return false;
                }

                return true;
            }
            catch
            {
            }

            return false;
        }

        protected string AuthorizationToken
        {
            get
            {
                string authorizationToken = string.Empty;

                var ctx = HttpContext;
                if (ctx != null)
                {
                    authorizationToken = ctx.Request.Headers["Authorization"].ToString();
                }

                return authorizationToken;
            }
        }
    }
}