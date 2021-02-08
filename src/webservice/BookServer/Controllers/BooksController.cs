using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http;
using BookService.Models;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace BookService.Controllers
{
    [ApiController]
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
            var userBooks = UserBooks;
            var book = userBooks.SingleOrDefault(x => x.ISBN == isbn);

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
        public HttpResponseMessage Put(string isbn, [FromBody] Book book)
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
                Console.WriteLine(JsonSerializer.Serialize(book));


                var userBooks = UserBooks;
                var existingBook = userBooks.SingleOrDefault(x => x.ISBN == isbn);
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
        public HttpResponseMessage Post([FromBody] Book book)
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
                Console.WriteLine(JsonSerializer.Serialize(book));

                book.ISBN = BookFactory.CreateISBN();

                if (!ModelState.IsValid)
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                }

                var userBooks = UserBooks;

                if(userBooks.Count >= 10)
                {
                    return new HttpResponseMessage(HttpStatusCode.TooManyRequests);
                }

                if (userBooks.Any(x => x.ISBN == book.ISBN))
                {
                    return new HttpResponseMessage(HttpStatusCode.Conflict);
                }

                userBooks.Add(book);

                var json = JsonSerializer.Serialize(book);
                
                HttpContext.Response.ContentType = "application/json";
                var resp = new HttpResponseMessage(HttpStatusCode.Created)
                {
                    Content = new StringContent(json)
                };

                resp.Headers.Location = new UriBuilder(Request.Scheme, Request.Host.Host, Request.Host.Port ?? -1, book.ISBN).Uri;

                return resp;
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

                var userBooks = UserBooks;
                var existingBook = userBooks.SingleOrDefault(x => x.ISBN == isbn);

                if (existingBook == null)
                {
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                }
                Console.WriteLine($"POST /api/books/{isbn}");
                userBooks.RemoveAll(x => x.ISBN == isbn);

                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }
    }
}