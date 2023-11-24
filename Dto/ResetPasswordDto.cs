namespace UNITEE_BACKEND.Dto
{
    public class ResetPasswordDto
    {
        public string? Token { get; set; }
        public string? NewPassword { get; set; }
    }
}
