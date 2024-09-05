using Compooler.Tests.Utilities.Fixtures;

namespace Compooler.Persistence.Tests;

[CollectionDefinition(Name)]
public class PersistenceTestsCollection : ICollectionFixture<DatabaseFixture>
{
    public const string Name = nameof(PersistenceTestsCollection);
}
