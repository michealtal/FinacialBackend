using FinacialBackend.Model;

namespace FinacialBackend.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
    }
}
