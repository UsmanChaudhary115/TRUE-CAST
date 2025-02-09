using Microsoft.AspNetCore.Mvc;
using OVS.Models;

namespace OVS.Components
{
    public class UserFeedbacks: ViewComponent
    {
        private readonly AppDbContext _context;
        public UserFeedbacks(AppDbContext con)
        {
            _context = con;
        }
        public async Task<IViewComponentResult> InvokeAsync(int count)
        {
            var courses = _context.Feedbacks.ToList();
            return View(courses);
        }
    }
}
