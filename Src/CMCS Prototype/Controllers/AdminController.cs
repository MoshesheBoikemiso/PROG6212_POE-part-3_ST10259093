using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CMCS_Prototype.Data;
using CMCS_Prototype.Models;

namespace CMCS_Prototype.Controllers
{ 
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
                .Where(c => c.Status == "Submitted" ||
                           (userRole == "Manager" && c.Status == "Approved by Coordinator"))
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
            var userRole = HttpContext.Session.GetString("UserRole");
            var userName = HttpContext.Session.GetString("UserName");

            var claim = _context.Claims
                .Include(c => c.User)
                .FirstOrDefault(c => c.ClaimID == id);

            if (claim != null)
            {
                if (claim.TotalHours > 160)
                {
                    TempData["Error"] = "Cannot approve: Hours exceed maximum allowed (160 hours)";
                    return RedirectToAction("AdminDashboard");
                }

                if (claim.HourlyRate > 300)
                {
                    TempData["Error"] = "Cannot approve: Hourly rate exceeds maximum allowed (R300/hour)";
                    return RedirectToAction("AdminDashboard");
                }

                if (claim.TotalAmount > 50000)
                {
                    TempData["Error"] = "Cannot approve: Total amount exceeds maximum allowed (R50,000)";
                    return RedirectToAction("AdminDashboard");
                }

                if (userRole == "Coordinator")
                {
                    claim.Status = "Approved by Coordinator";
                    TempData["Message"] = "Claim approved by Coordinator! Waiting for Manager approval.";
                }
                else if (userRole == "Manager")
                {
                    if (claim.Status == "Approved by Coordinator")
                    {
                        claim.Status = "Fully Approved";
                        TempData["Message"] = "Claim fully approved! Ready for HR processing.";
                    }
                    else
                    {
                        claim.Status = "Approved by Manager";
                        TempData["Message"] = "Claim approved by Manager!";
                    }
                }

                _context.SaveChanges();
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

        public IActionResult ViewDocument(int id)
        {
            var document = _context.Documents.Find(id);
            if (document == null || !System.IO.File.Exists(document.FilePath))
            {
                return NotFound();
            }

            var fileBytes = System.IO.File.ReadAllBytes(document.FilePath);
            var contentType = "application/octet-stream";

            var extension = Path.GetExtension(document.FileName).ToLower();
            if (extension == ".pdf") contentType = "application/pdf";
            else if (extension == ".docx") contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            else if (extension == ".xlsx") contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            else if (extension == ".jpg" || extension == ".jpeg") contentType = "image/jpeg";
            else if (extension == ".png") contentType = "image/png";

            return File(fileBytes, contentType, document.FileName);
        }
    }
}