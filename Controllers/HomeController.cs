using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OVS.Models;

namespace OVS.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        public HomeController(AppDbContext con)
        {
            _context = con;
        }
        public IActionResult UserLogInInterface()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpPost]
        public IActionResult SubmitFeedback(string Message)
        {
            if (Message != null) 
            {
                FeedbackModel model = new FeedbackModel();
                model.Name= JsonSerializer.Deserialize<User>(HttpContext.Request.Cookies["UserDetails"])?.FirstName;
                model.Feedback = Message;
                _context.Feedbacks.Add(model);
                _context.SaveChanges();
            }
            return RedirectToAction("UserPannel", "User");
        }
    }
}
