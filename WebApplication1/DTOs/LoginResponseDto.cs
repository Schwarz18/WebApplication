namespace WebApplication1.DTOs
{
    public class LoginResponseDto
    {
        public string JwtToken { get; set; }
        public string UserId { get; set; } // Add this property to fix CS0117  
        public string Role { get; set; }
    }
}
