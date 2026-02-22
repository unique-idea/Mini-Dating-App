using Mini_Dating_App_BE.Data.Models;

namespace Mini_Dating_App_BE.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
