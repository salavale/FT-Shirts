using FTShirts.Web.Models.UserModels.UserRequest;

namespace FTShirts.Web.Services
{
    public interface IUserService
    {
        Task<bool> ValidateLoginAsync(LogRegRequest request);
        Task<bool> RegisterUserAsync(LogRegRequest request);
        void EnsureUserActive(string username);
        int GetActiveUsersCount();

        void LogOutUpdate(string username);
    }
}