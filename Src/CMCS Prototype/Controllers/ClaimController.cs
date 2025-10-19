using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CMCS_Prototype.Data;
using CMCS_Prototype.Models;

namespace CMCS_Prototype.Controllers
{
    public class ClaimController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClaimController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult LecturerDashboard()
        {
            if (HttpContext.Session.GetString("UserRole") != "Lecturer")
            {
                return RedirectToAction("Index", "Home");
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            var claims = _context.Claims
                .Where(c => c.UserID == userId)
                .Include(c => c.Documents)
                .OrderByDescending(c => c.DateSubmissions)
                .ToList();

            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            return View(claims);
        }

        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("UserRole") != "Lecturer")
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Claim claim, IFormFile supportingDocument)
        {
            // if (ModelState.IsValid)
            {
                try
                {
                    if (supportingDocument != null && supportingDocument.Length > 0)
                    {
                        if (supportingDocument.Length > 5 * 1024 * 1024)
                        {
                            TempData["Error"] = "File size is too big. Maximum size is 5MB.";
                            return View(claim);
                        }

                        var allowedExtensions = new[] { ".pdf", ".docx", ".xlsx", ".jpg", ".png", ".jpeg" };
                        var fileExtension = Path.GetExtension(supportingDocument.FileName).ToLower();

                        if (!allowedExtensions.Contains(fileExtension))
                        {
                            TempData["Error"] = "Invalid file type. Allowed: PDF, Word, Excel, JPG, PNG";
                            return View(claim);
                        }
                    }

                    claim.UserID = HttpContext.Session.GetInt32("UserId") ?? 0;
                    claim.DateSubmissions = DateTime.Now;
                    claim.Status = "Submitted";

                    if (claim.TotalAmount == 0 && claim.TotalHours > 0)
                    {
                        decimal hourlyRate = 180;
                        claim.TotalAmount = claim.TotalHours * hourlyRate;
                    }

                    _context.Claims.Add(claim);
                    await _context.SaveChangesAsync();

                    if (supportingDocument != null && supportingDocument.Length > 0)
                    {
                        var fileName = Path.GetFileName(supportingDocument.FileName);
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                        if (!Directory.Exists(uploadsFolder))
                            Directory.CreateDirectory(uploadsFolder);

                        var filePath = Path.Combine(uploadsFolder, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await supportingDocument.CopyToAsync(stream);
                        }

                        var document = new Document
                        {
                            FileName = fileName,
                            FilePath = filePath,
                            ClaimID = claim.ClaimID
                        };

                        _context.Documents.Add(document);
                        await _context.SaveChangesAsync();
                    }

                    TempData["Success"] = "Claim submitted successfully!";
                    return RedirectToAction("LecturerDashboard");
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"An error occurred: {ex.Message}";
                    return View(claim);
                }
            }

            // return View(claim);
        }
    }
}