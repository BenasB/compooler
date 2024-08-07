namespace Compooler.Application.Tests;

[CollectionDefinition(Name)]
public class ApplicationTestsCollection : ICollectionFixture<ApplicationFixture>
{
    public const string Name = nameof(ApplicationTestsCollection);
}
