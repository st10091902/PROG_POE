namespace PROG_POE_P3.Models
{
    public class ClaimStatusViewModel
    {
        public List<Claim> Claims { get; set; } = new List<Claim>();
        public List<SupportingDocument> SupportingDocuments { get; set; } = new List<SupportingDocument>();
    }
}
