namespace UNITEE_BACKEND.Models.Request
{
    public class UpdateCustomerRequest
    {
        public string? firstName { get; set; }
        public string? lastName { get; set; }
        public string? email { get; set; }
        public string? gender { get; set; }
        public string? phoneNumber { get; set; }
        public IFormFile? Image { get; set; }
    }
}
