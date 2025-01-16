using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OVS.Models;

namespace OVS.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        public AdminController(AppDbContext con)
        {
            _context = con;
        }
        public IActionResult AdminPannel()
        {
            if (HttpContext.Request.Cookies.ContainsKey("Admin"))
            {
                var electionResults = _context.ElectionResults.ToList();
                return View(electionResults);
            }

            TempData["Message"] = "You must log in first.";
            return View("AdminSignIn");
        }
        public IActionResult AdminSignIn()
        {
            if (HttpContext.Request.Cookies.ContainsKey("Admin"))
            {
                return RedirectToAction("AdminPannel");
            }

            return View();
        }
        [HttpPost]
        public IActionResult AdminSignIn(string Email, string Password)
        {
            if(Email == "admin@gmail.com" && Password == "12345")
            {
                
                HttpContext.Response.Cookies.Append("Admin", "Admin", new CookieOptions { HttpOnly = true, Secure = true, SameSite = SameSiteMode.Strict, Expires = DateTimeOffset.UtcNow.AddDays(7) });
                return RedirectToAction("AdminPannel");
            }
            TempData["Message"] = "Email or Password maybe incorrect";
            return View();

        }
        public IActionResult AdminSignOut()
        {
            if(HttpContext.Request.Cookies.ContainsKey("Admin"))
            {
                return View();
            }

            TempData["Message"] = "You must log in first.";
            return View("AdminSignIn");
        }
        [HttpPost]
        public IActionResult AdminSignOutConfirmed()
        {
            HttpContext.Response.Cookies.Delete("Admin");
            return RedirectToAction("UserLogInInterface", "Home");
        }
        public IActionResult ShowResultForAdmin()
        {
            var electionResults = _context.ElectionResults.ToList();
            return View(electionResults);
        }

    }
}
