using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CMCS_Prototype.Data;
using CMCS_Prototype.Models;

namespace CMCS_Prototype.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user != null)
            {
                HttpContext.Session.SetInt32("UserId", user.UserID);
                HttpContext.Session.SetString("UserRole", user.UserRole);
                HttpContext.Session.SetString("UserName", user.FirstName);

                return RedirectToAction("Dashboard");
            }

            ViewBag.Error = "User not found. Try: lecturer@email.com, coordinator@email.com, or manager@email.com";
            return View("Index");
        }

        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index");
            }

            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole == "Lecturer")
                return RedirectToAction("LecturerDashboard", "Claim");
            else if (userRole == "Coordinator" || userRole == "Manager")
                return RedirectToAction("AdminDashboard", "Admin");
            else
                return RedirectToAction("Index");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}