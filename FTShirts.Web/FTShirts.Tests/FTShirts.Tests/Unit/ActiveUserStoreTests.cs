using FTShirts.Web.Services;

namespace FTShirts.Tests.Unit
{
    public class ActiveUserStoreTests
    {
        // Every test gets its own store, so tests never share users
        private readonly ActiveUserStore _store = new ActiveUserStore();

        [Fact]
        public void GetOrAdd_NewUser_AddsIt()
        {
            var user = _store.GetOrAdd("ana");

            Assert.Equal("ana", user.Username);
            Assert.Same(user, _store.Find("ana"));
        }

        [Fact]
        public void GetOrAdd_ExistingUser_ReturnsTheSameUserAndCart()
        {
            var first = _store.GetOrAdd("ana");

            var second = _store.GetOrAdd("ana");

            Assert.Same(first, second);
        }

        [Fact]
        public void Find_UserNotLoggedIn_ReturnsNull()
        {
            Assert.Null(_store.Find("nobody"));
        }

        [Fact]
        public void Remove_LoggedInUser_RemovesIt()
        {
            _store.GetOrAdd("ana");

            _store.Remove("ana");

            Assert.Null(_store.Find("ana"));
        }

        [Fact]
        public void GetOrAdd_ManyThreadsAtTheSameTime_CreatesOnlyOneUser()
        {
            var users = new System.Collections.Concurrent.ConcurrentBag<object>();

            Parallel.For(0, 100, _ => users.Add(_store.GetOrAdd("ana")));

            Assert.All(users, user => Assert.Same(users.First(), user));
        }
    }
}