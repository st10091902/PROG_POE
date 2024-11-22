using System.ComponentModel.DataAnnotations;

namespace PROG_POE_P3.Models
{
    public class Lecturer
    {
        public int LecturerID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public decimal HourlyRate { get; set; }
    }

    public class Claim
    {
        public int ClaimID { get; set; } 
        public int LecturerID { get; set; } 
        public int HoursWorked { get; set; } 
        public decimal HourlyRate { get; set; } 
        public string Status { get; set; } 
        public DateTime SubmissionDate { get; set; }
        public string RejectionReason { get; set; }


        public decimal TotalAmount => HoursWorked * HourlyRate;
    }



    public class SupportingDocument
    {
        public int DocumentID { get; set; }
        public string FilePath { get; set; }
        public DateTime UploadedDate { get; set; }
        public int ClaimID { get; set; }
    }

    public class Approval
    {
        public int ApprovalID { get; set; }
        public int ClaimID { get; set; }
        public bool CoordinatorApproved { get; set; }
        public bool ManagerApproved { get; set; }
        public DateTime ApprovalDate { get; set; }
    }
}
