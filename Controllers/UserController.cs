using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OVS.Models;
using System.Linq;
using System.Text.Json;

namespace OVS.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserController(AppDbContext context, UserManager<User> userManager, SignInManager<User> signInManager, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _passwordHasher = passwordHasher;
        }

        public IActionResult UserLogInInterface()
        {
            return View();
        }

        public IActionResult UserPannel()
        {
            if (HttpContext.Request.Cookies.ContainsKey("UserDetails"))
            {
                ViewBag.Name = JsonSerializer.Deserialize<User>(HttpContext.Request.Cookies["UserDetails"])?.FirstName;
                ViewBag.ClosestElection = _context.Elections
                               .Where(e => e.StartDate >= DateTime.Now)
                               .OrderBy(e => e.StartDate)
                               .Select(e => e.StartDate)
                               .FirstOrDefault();
                return View();
            }

            TempData["Message"] = "You must log in first.";
            return View("SignIn");
        }

        public IActionResult SignIn()
        {
            if (HttpContext.Request.Cookies.ContainsKey("UserDetails"))
            {
                return RedirectToAction("UserPannel");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(string Email, string Password)
        {
            var user = await _userManager.FindByEmailAsync(Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, Password))
            { 
                HttpContext.Response.Cookies.Append("UserDetails", JsonSerializer.Serialize(user), new CookieOptions { HttpOnly = true, Secure = true, SameSite = SameSiteMode.Strict, Expires = DateTimeOffset.UtcNow.AddDays(7) });
                ViewBag.Name = user.FirstName;
                return RedirectToAction("UserPannel");
            }
            else
            {
                TempData["Message"] = "Invalid credentials.";
                return View();
            }
        }

        public IActionResult SignOut()
        {
            if (HttpContext.Request.Cookies.ContainsKey("UserDetails"))
                return View();

            TempData["Message"] = "You must log in first.";
            return View("SignIn");
        }

        public IActionResult ConfirmedSignOut()
        {
            HttpContext.Response.Cookies.Delete("UserDetails");
            return RedirectToAction("UserLogInInterface", "Home");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User user, string Password, string ConfirmPassword)
        {
            if (Password != ConfirmPassword)
            {
                TempData["Message"] = "Passwords do not match.";
                return View(user);
            }

            // Don't set the Id manually. Let UserManager handle it.
            var result = await _userManager.CreateAsync(user, Password);

            if (result.Succeeded)
            {
                // Log the user in after successful registration
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("SignIn");
            }
            else
            {
                // Return the model with validation errors
                TempData["Message"] = "Registration failed: " + string.Join(", ", result.Errors.Select(e => e.Description));
                return View(user);
            }
        }



        public IActionResult Profile()
        {
            if (HttpContext.Request.Cookies.ContainsKey("UserDetails"))
            {
                var userDetails = JsonSerializer.Deserialize<User>(HttpContext.Request.Cookies["UserDetails"]);
                return View(userDetails);
            }

            TempData["Message"] = "You must log in first.";
            return View("SignIn");
        }

        public IActionResult EditProfile(string id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound();

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(User updatedUser, IFormFile ProfilePicture)
        {
            var user = await _userManager.FindByIdAsync(updatedUser.Id.ToString());
            if (user == null)
                return NotFound();

            // Update user properties
            user.FirstName = updatedUser.FirstName;
            user.LastName = updatedUser.LastName;
            user.DateOfBirth = updatedUser.DateOfBirth;
            user.Gender = updatedUser.Gender;
            user.PhoneNumber = updatedUser.PhoneNumber;
            user.Street = updatedUser.Street;
            user.City = updatedUser.City;
            user.State = updatedUser.State;
            user.ZipCode = updatedUser.ZipCode;
            user.Country = updatedUser.Country;

            // Handle Profile Picture upload
            if (ProfilePicture != null && ProfilePicture.Length > 0)
            {
                var imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                if (!Directory.Exists(imagesFolder))
                {
                    Directory.CreateDirectory(imagesFolder);
                }

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ProfilePicture.FileName);
                var filePath = Path.Combine(imagesFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    ProfilePicture.CopyTo(stream);
                }

                user.ProfilePicture = fileName;
            }

            var updateResult = await _userManager.UpdateAsync(user);
            if (updateResult.Succeeded)
            {
                // Update the cookie after profile edit
                HttpContext.Response.Cookies.Append("UserDetails", JsonSerializer.Serialize(user));
                return RedirectToAction("Profile");
            }
            else
            {
                TempData["Message"] = "Profile update failed: " + string.Join(", ", updateResult.Errors.Select(e => e.Description));
                return View(updatedUser);
            }

        }
    }
}
