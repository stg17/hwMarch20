using hwMarch20.Data;
using hwMarch20.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

namespace hwMarch20.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string _connectionString;
        private static List<int> _ids;

        public HomeController(IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            _webHostEnvironment = webHostEnvironment;
            _connectionString = configuration.GetConnectionString("Constr");
        }

        public IActionResult Index()
        {
            var repo = new ImageRepository(_connectionString);
            return View(new HomeViewModel()
            {
                Images = repo.GetImages()
            });
        }

        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Upload(string title, IFormFile imageFile)
        {
            var fileName = Guid.NewGuid() + "-" + imageFile.FileName;
            var fullImagePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", fileName);
            using FileStream fs = new(fullImagePath, FileMode.Create);
            imageFile.CopyTo(fs);

            var repo = new ImageRepository(_connectionString);
            repo.AddImage(new Image()
            {
                Title = title,
                ImageName = fileName,
                Date = DateTime.Now,
                Likes = 0
            });

            return Redirect("/");
        }

        public IActionResult ViewImage(int id)
        {
            var repo = new ImageRepository(_connectionString);
            var image = repo.GetById(id);
            return View(new ImageViewModel()
            {
                Image = image
            });
        }

        [HttpPost]
        public void AddLike(int id)
        {
            var repo = new ImageRepository(_connectionString);
            repo.AddLike(id);
            if(HttpContext.Session.Get<List<int>>("ids") == null)
            {
                _ids = new();
            }
            else
            {
                _ids = HttpContext.Session.Get<List<int>>("ids");
            }
            _ids.Add(id);
            HttpContext.Session.Set<List<int>>("ids", _ids);
        }

        public IActionResult UpdateLikes(int id)
        {
            var repo = new ImageRepository(_connectionString);
            return Json(repo.UpdateLikes(id));
        }

        public IActionResult EnableButton(int id)
        {
            List<int> ids = HttpContext.Session.Get<List<int>>("ids");
            bool check = false;
            if (ids != null)
            {
                check = ids.Any(i => i == id);
            }

            return Json(check);
        }
    }

    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            string value = session.GetString(key);

            return value == null ? default(T) :
                JsonSerializer.Deserialize<T>(value);
        }
    }
}
