using FTShirts.Web.Models.UserModels.UserRequest;

namespace FTShirts.Web.Services
{
    public interface IUserService
    {
        bool ValidateLogin(LogRegRequest request);
        bool RegisterUser(LogRegRequest request);
        void EnsureUserActive(string username);

        void LogOutUpdate(string username);
    }
}