using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CMCS_Prototype.Data;
using CMCS_Prototype.Models;

namespace CMCS_Prototype.Controllers
{ //Handles the approval and rejection workflow for coordinators and managers  
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult AdminDashboard()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Coordinator" && userRole != "Manager")
            {
                return RedirectToAction("Index", "Home");
            }

            var pendingClaims = _context.Claims
                .Where(c => c.Status == "Submitted")
                .Include(c => c.User)
                .Include(c => c.Documents)
                .OrderBy(c => c.DateSubmissions)
                .ToList();

            ViewBag.UserRole = userRole;
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            return View(pendingClaims);
        }

        [HttpPost]
        public IActionResult ApproveClaim(int id)
        {
            var claim = _context.Claims.Find(id);
            if (claim != null)
            {
                claim.Status = "Approved";
                _context.SaveChanges();
                TempData["Message"] = "Claim approved successfully!";
            }
            return RedirectToAction("AdminDashboard");
        }

        [HttpPost]
        public IActionResult RejectClaim(int id)
        {
            var claim = _context.Claims.Find(id);
            if (claim != null)
            {
                claim.Status = "Rejected";
                _context.SaveChanges();
                TempData["Message"] = "Claim rejected!";
            }
            return RedirectToAction("AdminDashboard");
        }
    }
}