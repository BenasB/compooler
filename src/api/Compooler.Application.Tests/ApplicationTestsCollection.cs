using Compooler.Tests.Utilities.Fixtures;

namespace Compooler.Application.Tests;

[CollectionDefinition(Name)]
public class ApplicationTestsCollection : ICollectionFixture<DatabaseFixture>
{
    public const string Name = nameof(ApplicationTestsCollection);
}
