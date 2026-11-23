using FTShirts.Web.Models.CatalogueModels;
using FTShirts.Web.Models.UserModels;
using FTShirts.Web.Models.UserModels.UserRequest;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FTShirts.Web.Services
{
    public class UserService : IUserService
    {
        private const string FilePath = "./Data/users.csv";

        // Logged-in users and their shopping carts, by username.
        // Every request runs on its own thread and all of them share this collection,
        // so it must be thread-safe: unlike List<T>, ConcurrentDictionary can be read
        // and changed by several threads at the same time.
        public static ConcurrentDictionary<string, User> ActiveUsers { get; } = new ConcurrentDictionary<string, User>();

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
            // GetOrAdd checks and adds in one atomic step, so two logins at the same
            // time can never add the same user twice
            ActiveUsers.GetOrAdd(username, name => new User(name));
        }

        public void LogOutUpdate(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return;
            }

            ActiveUsers.TryRemove(username, out _);
        }
    }
}