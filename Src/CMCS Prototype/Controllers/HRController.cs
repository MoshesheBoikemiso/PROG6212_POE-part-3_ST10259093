using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CMCS_Prototype.Data;
using CMCS_Prototype.Models;
using System.Text;

namespace CMCS_Prototype.Controllers
{
    public class HRController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HRController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult HRDashboard()
        {
            if (HttpContext.Session.GetString("UserRole") != "HR")
                return RedirectToAction("Index", "Home");

            var stats = new
            {
                TotalClaims = _context.Claims.Count(),
                ApprovedClaims = _context.Claims.Count(c => c.Status == "Fully Approved"),
                PendingClaims = _context.Claims.Count(c => c.Status.Contains("Approved")),
                TotalLecturers = _context.Users.Count(u => u.UserRole == "Lecturer")
            };

            ViewBag.Stats = stats;
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            return View();
        }

        public IActionResult GenerateInvoice(int claimId)
        {
            var claim = _context.Claims
                .Include(c => c.User)
                .FirstOrDefault(c => c.ClaimID == claimId);

            if (claim == null || claim.Status != "Fully Approved")
            {
                TempData["Error"] = "Claim not found or not approved for payment";
                return RedirectToAction("ApprovedClaims");
            }

            var invoiceContent = $@"
                INVOICE FOR PAYMENT PROCESSING
                ==============================
                Claim ID: {claim.ClaimID}
                Lecturer: {claim.User.FirstName} {claim.User.LastName}
                Period: {claim.ClaimMonth:MMMM yyyy}
                Hours Worked: {claim.TotalHours}
                Hourly Rate: R{claim.HourlyRate:N2}
                Total Amount: R{claim.TotalAmount:N2}
                Status: {claim.Status}
                Generated: {DateTime.Now:dd MMM yyyy HH:mm}
                
                Payment Instructions:
                - Process within 5 working days
                - Reference: CLAIM-{claim.ClaimID}
                - Department: Academic Services
            ";

            var bytes = Encoding.UTF8.GetBytes(invoiceContent);
            return File(bytes, "text/plain", $"Invoice_Claim_{claim.ClaimID}.txt");
        }

        public IActionResult ProcessBatchPayments()
        {
            var approvedClaims = _context.Claims
                .Where(c => c.Status == "Fully Approved")
                .Include(c => c.User)
                .ToList();

            if (!approvedClaims.Any())
            {
                TempData["Error"] = "No approved claims for batch processing";
                return RedirectToAction("HRDashboard");
            }

            var batchReport = new StringBuilder();
            batchReport.AppendLine("BATCH PAYMENT PROCESSING REPORT");
            batchReport.AppendLine("===============================");
            batchReport.AppendLine($"Generated: {DateTime.Now:dd MMM yyyy HH:mm}");
            batchReport.AppendLine();

            decimal totalAmount = 0;
            foreach (var claim in approvedClaims)
            {
                batchReport.AppendLine($"Claim #{claim.ClaimID}: {claim.User.FirstName} {claim.User.LastName} - R{claim.TotalAmount:N2}");
                totalAmount += claim.TotalAmount;

                claim.Status = "Processed for Payment";
            }

            batchReport.AppendLine();
            batchReport.AppendLine($"TOTAL AMOUNT: R{totalAmount:N2}");
            batchReport.AppendLine($"NUMBER OF CLAIMS: {approvedClaims.Count}");

            _context.SaveChanges();

            var bytes = Encoding.UTF8.GetBytes(batchReport.ToString());
            return File(bytes, "text/plain", $"Batch_Payment_Report_{DateTime.Now:yyyyMMdd_HHmm}.txt");
        }

        public IActionResult ApprovedClaims()
        {
            if (HttpContext.Session.GetString("UserRole") != "HR")
                return RedirectToAction("Index", "Home");

            var approvedClaims = _context.Claims
                .Where(c => c.Status == "Fully Approved" || c.Status == "Processed for Payment")
                .Include(c => c.User)
                .OrderByDescending(c => c.DateSubmissions)
                .ToList();

            return View(approvedClaims);
        }

        public IActionResult ManageLecturers()
        {
            if (HttpContext.Session.GetString("UserRole") != "HR")
                return RedirectToAction("Index", "Home");

            var lecturers = _context.Users
                .Where(u => u.UserRole == "Lecturer")
                .OrderBy(u => u.LastName)
                .ToList();

            return View(lecturers);
        }

        public IActionResult EditLecturer(int id)
        {
            var lecturer = _context.Users.Find(id);
            if (lecturer == null || lecturer.UserRole != "Lecturer")
            {
                return NotFound();
            }

            return View(lecturer);
        }
        
        [HttpPost]
        public IActionResult EditLecturer(User lecturer)
        {
            if (ModelState.IsValid)
            {
                _context.Users.Update(lecturer);
                _context.SaveChanges();
                TempData["Message"] = "Lecturer information updated successfully!";
                return RedirectToAction("ManageLecturers");
            }

            return View(lecturer);
        }
    }
}