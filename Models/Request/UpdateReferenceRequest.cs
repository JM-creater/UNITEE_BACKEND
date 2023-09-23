namespace UNITEE_BACKEND.Models.Request
{
    public class UpdateReferenceRequest
    {
        public int Id { get; set; }
        public string? ReferenceId { get; set; }
        public IFormFile? ProofOfPayment { get; set; }
    }
}
