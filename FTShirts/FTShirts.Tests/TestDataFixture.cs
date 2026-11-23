// The application keeps static state or shared resources that may not be
// thread-safe, so the tests run within a single collection per assembly.
[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly)]

namespace FTShirts.Tests
{
    public sealed class TestDataFixture : IDisposable
    {
        public void Dispose()
        {
        }
    }
}