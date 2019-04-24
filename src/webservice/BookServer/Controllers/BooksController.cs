using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http;
using System.Threading;
using BookService.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace BookService.Controllers
{
    [Route("api/[controller]")]
    public class BooksController : BaseController
    {
        [HttpGet]
        public ActionResult Get()
        {
            var authorized = CheckAuthorization();
            if (!authorized)
            {
                return Unauthorized();
            }
            Console.WriteLine("GET /api/books");
            return new JsonResult(UserBooks);
        }

        [HttpGet("{isbn}")]
        public ActionResult Get(string isbn)
        {
            var authorized = CheckAuthorization();
            if (!authorized)
            {
                return Unauthorized();
            }

            if (string.IsNullOrEmpty(isbn))
                return this.BadRequest();

            isbn = isbn.ToUpperInvariant();
            Console.WriteLine($"GET /api/books/{isbn}");
            Book book = UserBooks.SingleOrDefault(x => x.ISBN == isbn);

            if (book == null)
            {
                return this.NotFound();
            }
            else
            {
                return this.Ok(book);
            }
        }

        [HttpPut("{isbn}")]
        public async Task<HttpResponseMessage> Put(string isbn, [FromBody] Book book)
        {
            try
            {
                var authorized = CheckAuthorization();
                if (!authorized)
                {
                    return new HttpResponseMessage(HttpStatusCode.Unauthorized);
                }

                if (!ModelState.IsValid)
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                }

                if (string.IsNullOrEmpty(book.ISBN))
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                }

                Console.WriteLine($"PUT /api/books/{isbn}");
                Console.WriteLine(JsonConvert.SerializeObject(book));

                var existingBook = UserBooks.SingleOrDefault(x => x.ISBN == isbn);
                if (existingBook != null)
                {
                    existingBook.Authors = book.Authors;
                    existingBook.Genre = book.Genre;
                    existingBook.PublishDate = book.PublishDate;
                    existingBook.Title = book.Title;
                }

                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Post([FromBody] Book book)
        {
            try
            {
                var authorized = CheckAuthorization();
                if (!authorized)
                {
                    return new HttpResponseMessage(HttpStatusCode.Unauthorized);
                }

                if (!string.IsNullOrWhiteSpace(book.ISBN))
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                }
                Console.WriteLine($"POST /api/books");
                Console.WriteLine(JsonConvert.SerializeObject(book));

                book.ISBN = BookFactory.CreateISBN();

                if (!ModelState.IsValid)
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                }

                if (UserBooks.Any(x => x.ISBN == book.ISBN))
                {
                    return new HttpResponseMessage(HttpStatusCode.Conflict);
                }

                UserBooks.Add(book);

                var json = JsonConvert.SerializeObject(book);
                
                HttpContext.Response.ContentType = "application/json";
                return new HttpResponseMessage(HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);

            }
        }

        [HttpDelete]
        [Route("{isbn}")]
        public HttpResponseMessage Delete(string isbn)
        {
            try
            {
                var authorized = CheckAuthorization();
                if (!authorized)
                {
                    return new HttpResponseMessage(HttpStatusCode.Unauthorized);
                }

                var existingBook = UserBooks.SingleOrDefault(x => x.ISBN == isbn);

                if (existingBook == null)
                {
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                }
                Console.WriteLine($"POST /api/books/{isbn}");
                UserBooks.RemoveAll(x => x.ISBN == isbn);

                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }
    }
}