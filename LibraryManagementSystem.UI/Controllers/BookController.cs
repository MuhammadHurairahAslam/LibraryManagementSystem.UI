using System.Diagnostics;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;
using LibraryManagementSystem.UI.Models;
using Microsoft.AspNetCore.Mvc;
using static System.Reflection.Metadata.BlobBuilder;


namespace LibraryManagementSystem.UI.Controllers
{
    public class BookController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public BookController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            using (var client = _httpClientFactory.CreateClient("WithApiKey"))
            {
                client.Timeout = TimeSpan.FromSeconds(30);

                try
                {
                    var response = await client.GetAsync("Book/GetAllBooks");

                    if (!response.IsSuccessStatusCode)
                    {
                        return StatusCode((int)response.StatusCode);
                    }

                    var json = await response.Content.ReadAsStringAsync();
                    var books = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Book>>(json);

                    ViewBag.GridData = books;
                    return View();
                }
                catch (TaskCanceledException ex)
                {

                    return BadRequest("Request timed out or was canceled.");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }

        public async Task<IActionResult> SaveBook(Book book)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", book);
            }

            try
            {
                using (var client = _httpClientFactory.CreateClient("WithApiKey"))
                {
                    client.Timeout = TimeSpan.FromSeconds(30);

                    var json = JsonSerializer.Serialize(book);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync("Book/AddBook", content);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Error calling the API.");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Exception: {ex.Message}");
            }

            return View("Index", book);
        }


        public async Task<IActionResult> View(int id)
        {

            using (var client = _httpClientFactory.CreateClient("WithApiKey"))
            {
                var response = await client.GetAsync($"Book/GetBookById/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();

                    var book = JsonSerializer.Deserialize<Book>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return PartialView("_ViewModal", book);
                }
            }
            return PartialView("_ViewModal", new Book());
        }
        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> EditAsync(int id)
        {
            using (var client = _httpClientFactory.CreateClient("WithApiKey"))
            {
                var response = await client.GetAsync($"Book/GetBookById/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();

                    var book = JsonSerializer.Deserialize<Book>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return PartialView("_EditModal", book);
                }
            }
            return PartialView("_EditModal", new Book());
        }

        public async Task<IActionResult> Update(Book book)
        {
            using (var client = _httpClientFactory.CreateClient("WithApiKey"))
            {
                var json = JsonSerializer.Serialize(book);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PutAsync("Book/UpdateBook", content);               
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> GetForDelete(int id)
        {
            using (var client = _httpClientFactory.CreateClient("WithApiKey"))
            {
                var response = await client.GetAsync($"Book/GetBookById/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();

                    var book = JsonSerializer.Deserialize<Book>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return PartialView("_DeleteModal", book);
                }
            }
            return PartialView("_DeleteModal", new Book());
        }
        public async Task<IActionResult> Delete(int id)
        {
            using (var client = _httpClientFactory.CreateClient("WithApiKey"))
            {
                client.Timeout = TimeSpan.FromSeconds(30);

                try
                {
                    var response = await client.DeleteAsync($"Book/DeleteBook/{id}");

                    if (!response.IsSuccessStatusCode)
                    {
                        return StatusCode((int)response.StatusCode);
                    }

                    return RedirectToAction("Index");
                }
                catch (TaskCanceledException ex)
                {

                    return BadRequest("Request timed out or was canceled.");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
