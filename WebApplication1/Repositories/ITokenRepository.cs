using WebApplication1.Models;

namespace WebApplication1.Repositories
{
    public interface ITokenRepository
    {
        string CreateJwtToken(ApplicationUser user, List<string> roles);
    }
}
