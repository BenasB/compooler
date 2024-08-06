using Compooler.Application;
using Compooler.Domain;
using HotChocolate.Configuration;
using HotChocolate.Types.Descriptors;
using HotChocolate.Types.Descriptors.Definitions;
using JetBrains.Annotations;

namespace Compooler.API.Types.Errors;

/// <summary>
/// Renames the generic EntityNotFoundError in SDL from 'EntityNotFoundErrorOfType' to 'TypeNotFoundError' and transforms the 'Id' field type
/// </summary>
[UsedImplicitly]
public class EntityNotFoundErrorTypeInterceptor : TypeInterceptor
{
    public override void OnBeforeRegisterDependencies(
        ITypeDiscoveryContext discoveryContext,
        DefinitionBase definition
    )
    {
        if (definition is not ObjectTypeDefinition objectTypeDef)
            return;

        var type = objectTypeDef.RuntimeType;

        if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(EntityNotFoundError<>))
            return;

        var entityType = type.GetGenericArguments()[0];
        objectTypeDef
            .ToDescriptor(discoveryContext.DescriptorContext)
            .Name(nameof(EntityNotFoundError<IEntity>).Replace("Entity", entityType.Name));

        var idField = objectTypeDef.Fields.First(field =>
            string.Equals(
                field.Name,
                nameof(EntityNotFoundError<IEntity>.Id),
                StringComparison.OrdinalIgnoreCase
            )
        );

        idField.ToDescriptor(discoveryContext.DescriptorContext).ID(entityType.Name).ToDefinition();
    }
}
