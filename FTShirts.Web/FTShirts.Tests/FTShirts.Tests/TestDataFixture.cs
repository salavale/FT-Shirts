// Created once before the first test and disposed after the last one.
[assembly: AssemblyFixture(typeof(FTShirts.Tests.TestDataFixture))]

// The catalogue is still kept in a static list that is not thread-safe (it will move to the
// database), so the tests must not run in parallel until then.
[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly)]
namespace FTShirts.Tests
{
    // The services read and write ./Data/shirts.csv and ./Data/users.csv relative to the
    // current directory. Before the tests start, they get a fresh copy of those files in a
    // temporary folder, so every run starts from the same data and never changes the real files.
    public sealed class TestDataFixture : IDisposable
    {
        private readonly string _originalDirectory = Directory.GetCurrentDirectory();

        public TestDataFixture()
        {
            WorkDirectory = Path.Combine(Path.GetTempPath(), "ftshirts-tests", Guid.NewGuid().ToString("N"));
            string dataDirectory = Path.Combine(WorkDirectory, "Data");
            Directory.CreateDirectory(dataDirectory);

            foreach (string file in Directory.GetFiles(Path.Combine(AppContext.BaseDirectory, "TestData"), "*.csv"))
            {
                File.Copy(file, Path.Combine(dataDirectory, Path.GetFileName(file)));
            }

            Directory.SetCurrentDirectory(WorkDirectory);
        }

        public string WorkDirectory { get; }

        public void Dispose()
        {
            Directory.SetCurrentDirectory(_originalDirectory);
        }
    }
}