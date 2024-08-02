using Compooler.API.Extensions;
using CookieCrumble;
using HotChocolate.Execution;
using Microsoft.Extensions.DependencyInjection;

namespace Compooler.API.Tests;

public class SchemaTests
{
    [Fact]
    public async Task SchemaChanged()
    {
        var schema = await new ServiceCollection()
            .AddGraphQL()
            .AddCompoolerTypes()
            .BuildSchemaAsync();

        schema.MatchSnapshot();
    }
}
