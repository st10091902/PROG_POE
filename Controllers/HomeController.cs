using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using PROG_POE_P3.Models;

namespace PROG_POE_P3.Controllers
{
    public class HomeController : Controller
    {
        // In-memory data storage
        private static List<Claim> _claims = new List<Claim>();
        private static List<SupportingDocument> _documents = new List<SupportingDocument>();

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult SubmitClaim()
        {
            // Render the form
            return View();
        }

        [HttpPost]
        public IActionResult SubmitClaim(int lecturerId, int hoursWorked, decimal hourlyRate, string additionalNotes, IFormFile fileUpload)
        {
            // Process file upload
            string filePath = null;
            if (fileUpload != null && fileUpload.Length > 0)
            {
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                filePath = Path.Combine(uploadPath, fileUpload.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    fileUpload.CopyTo(stream);
                }
            }

            // Create a new claim
            Claim newClaim = new Claim
            {
                ClaimID = _claims.Count + 1,
                LecturerID = lecturerId,
                HoursWorked = hoursWorked,
                HourlyRate = hourlyRate,
                Status = "Pending",
                SubmissionDate = DateTime.Now,
                RejectionReason = null
            };

            // Automatically process the claim
            if (newClaim.HoursWorked > 100)
            {
                newClaim.Status = "Rejected";
                newClaim.RejectionReason = "Exceeded maximum allowed hours.";
            }
            else if (newClaim.HourlyRate < 10)
            {
                newClaim.Status = "Rejected";
                newClaim.RejectionReason = "Hourly rate is below the minimum threshold.";
            }
            else
            {
                newClaim.Status = "Approved";
            }

            // Add the claim to the in-memory list
            _claims.Add(newClaim);

            // Save the supporting document (if provided)
            if (filePath != null)
            {
                SupportingDocument newDocument = new SupportingDocument
                {
                    DocumentID = _documents.Count + 1,
                    ClaimID = newClaim.ClaimID,
                    FilePath = fileUpload.FileName,
                    UploadedDate = DateTime.Now
                };

                _documents.Add(newDocument);
            }

            return RedirectToAction("ClaimStatus", new { lecturerId });
        }





        public IActionResult ClaimStatus(int lecturerId)
        {
            // Filter claims for the given Lecturer ID
            var lecturerClaims = _claims.Where(c => c.LecturerID == lecturerId).ToList();

            // Check if there are claims for the lecturer
            if (!lecturerClaims.Any())
            {
                ViewBag.Message = "No claims found for the provided Lecturer ID.";
            }

            // Pass the claims to the view
            return View(lecturerClaims);
        }





        [HttpPost]
        public IActionResult ProcessClaimApproval(int claimId, string action)
        {
            var claim = _claims.FirstOrDefault(c => c.ClaimID == claimId);

            if (claim != null)
            {
                // Approve or reject the claim based on the action
                if (action == "approve")
                {
                    claim.Status = "Approved";
                }
                else if (action == "reject")
                {
                    claim.Status = "Rejected";
                }
            }

            // Redirect back to the list of pending claims for approval
            return RedirectToAction("ApproveClaims");
        }

        public IActionResult ApproveClaims()
        {
            // Process pending claims for approval/rejection
            foreach (var claim in _claims.Where(c => c.Status == "Pending"))
            {
                if (claim.HoursWorked > 100)
                {
                    claim.Status = "Rejected";
                    claim.RejectionReason = "Exceeded maximum allowed hours.";
                }
                else if (claim.HourlyRate < 10)
                {
                    claim.Status = "Rejected";
                    claim.RejectionReason = "Hourly rate is below the minimum threshold.";
                }
                else
                {
                    claim.Status = "Approved";
                }
            }

            // Fetch processed claims (approved/rejected)
            var processedClaims = _claims.Where(c => c.Status != "Pending").ToList();

            // Pass claims to the view
            return View(processedClaims);
        }


        [HttpPost]
        public IActionResult UploadDocument(int claimId, IFormFile fileUpload)
        {
            if (fileUpload != null && fileUpload.Length > 0)
            {
                // Save file to in-memory list 
                SupportingDocument document = new SupportingDocument
                {
                    DocumentID = _documents.Count + 1,
                    ClaimID = claimId,
                    FilePath = fileUpload.FileName,
                    UploadedDate = DateTime.Now
                };

                _documents.Add(document);
            }

            return RedirectToAction("ClaimStatus");
        }

        public IActionResult GenerateReport()
        {
            // Filter only approved claims
            var approvedClaims = _claims.Where(c => c.Status == "Approved").ToList();

            // Calculate summary data (e.g., total hours, total amount)
            var totalHours = approvedClaims.Sum(c => c.HoursWorked);
            var totalAmount = approvedClaims.Sum(c => c.TotalAmount);

            // Create a ViewModel for the report
            var reportViewModel = new ReportViewModel
            {
                ApprovedClaims = approvedClaims,
                TotalHours = totalHours,
                TotalAmount = totalAmount
            };

            // Pass the ViewModel to the view
            return View(reportViewModel);
        }


    }
}
