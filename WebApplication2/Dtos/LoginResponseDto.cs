namespace WebApplication2.Dtos
{
    public class LoginResponseDto
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public Guid UserId { get; set; }
        public string JwtToken { get; set; }
        public string Role { get; set; }
    }
}
