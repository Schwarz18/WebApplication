namespace WebApplication2.Dtos
{
    public class UserProfileDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
    }


    public class UpdateUserProfileDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
    }

    public class ChangePasswordDto
    {
        public Guid Id { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
