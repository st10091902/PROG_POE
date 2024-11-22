namespace PROG_POE_P3.Models
{
    public class ReportViewModel
    {
        public List<Claim> ApprovedClaims { get; set; } = new List<Claim>();
        public int TotalHours { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
