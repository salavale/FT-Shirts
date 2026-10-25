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

        public static List<User> ActiveUsers { get; } = new List<User>();

        public bool ValidateLogin(LogRegRequest request)
        {
            if (!File.Exists(FilePath))
            {
                return false;
            }

            string expectedLine = request.Username + "," + request.Password;
            return File.ReadLines(FilePath).Any(line => line == expectedLine);
        }

        public bool RegisterUser(LogRegRequest request)
        {
            if (File.Exists(FilePath))
            {
                if (File.ReadLines(FilePath).Any(line => line.StartsWith(request.Username + ",")))
                {
                    return false;
                }
            }

            File.AppendAllText(FilePath, request.Username + "," + request.Password + "\n");
            return true;
        }

        public void EnsureUserActive(string username)
        {
            if (!ActiveUsers.Any(u => u.Username == username))
            {
                ActiveUsers.Add(new User(username));
            }
        }
        public void LogOutUpdate(string username)
        {
            ActiveUsers.RemoveAll(i => i.Username == username);
        }
    }
}