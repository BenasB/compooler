using Compooler.Persistence.Tests;

namespace Compooler.Application.Tests;

[CollectionDefinition(Name)]
public class ApplicationTestsCollection : ICollectionFixture<DatabaseFixture>
{
    public const string Name = nameof(ApplicationTestsCollection);
}
