using FTShirts.Web.Models.CatalogueModels;
using FTShirts.Web.Models.UserModels;
using FTShirts.Web.Models.UserModels.UserRequest;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FTShirts.Web.Services
{
    public class UserService : IUserService
    {
        private const string FilePath = "./Data/users.csv";

        // The logged-in users are shared by every request, so they live in the
        // IActiveUserStore singleton. ASP.NET creates a new UserService for each request
        // (AddScoped) and gives it that same store through the constructor.
        private readonly IActiveUserStore _activeUsers;

        public UserService(IActiveUserStore activeUsers)
        {
            _activeUsers = activeUsers;
        }

        // Reading users.csv is IO, so it is done asynchronously: while the file is being read,
        // the thread is free to serve other requests instead of waiting.
        // ReadLinesAsync still reads the file line by line (as a stream) and stops at the first match.
        public async Task<bool> ValidateLoginAsync(LogRegRequest request)
        {
            if (!File.Exists(FilePath))
            {
                return false;
            }

            string expectedLine = request.Username + "," + request.Password;
            await foreach (string line in File.ReadLinesAsync(FilePath))
            {
                if (line == expectedLine)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> RegisterUserAsync(LogRegRequest request)
        {
            if (File.Exists(FilePath))
            {
                await foreach (string line in File.ReadLinesAsync(FilePath))
                {
                    if (line.StartsWith(request.Username + ","))
                    {
                        return false;
                    }
                }
            }

            await File.AppendAllTextAsync(FilePath, request.Username + "," + request.Password + "\n");
            return true;
        }

        public void EnsureUserActive(string username)
        {
            _activeUsers.GetOrAdd(username);
        }

        public void LogOutUpdate(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return;
            }

            _activeUsers.Remove(username);
        }
    }
}