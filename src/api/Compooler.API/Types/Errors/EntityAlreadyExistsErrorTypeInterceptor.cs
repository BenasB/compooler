using Compooler.Application;
using Compooler.Domain;
using HotChocolate.Configuration;
using HotChocolate.Types.Descriptors;
using HotChocolate.Types.Descriptors.Definitions;
using JetBrains.Annotations;

namespace Compooler.API.Types.Errors;

/// <summary>
/// Renames the generic EntityAlreadyExistsError in SDL from 'EntityAlreadyExistsErrorOfType' to 'TypeAlreadyExistsError' and transforms the 'Id' field type
/// </summary>
[UsedImplicitly]
public class EntityAlreadyExistsErrorTypeInterceptor : TypeInterceptor
{
    public override void OnBeforeRegisterDependencies(
        ITypeDiscoveryContext discoveryContext,
        DefinitionBase definition
    )
    {
        if (definition is not ObjectTypeDefinition objectTypeDef)
            return;

        var type = objectTypeDef.RuntimeType;

        if (
            !type.IsGenericType
            || type.GetGenericTypeDefinition() != typeof(EntityAlreadyExistsError<,>)
        )
            return;

        var entityType = type.GetGenericArguments()[0];
        objectTypeDef
            .ToDescriptor(discoveryContext.DescriptorContext)
            .Name(
                nameof(EntityAlreadyExistsError<IEntity<object>, object>)
                    .Replace("Entity", entityType.Name)
            );

        var idField = objectTypeDef.Fields.First(field =>
            string.Equals(
                field.Name,
                nameof(EntityAlreadyExistsError<IEntity<object>, object>.Id),
                StringComparison.OrdinalIgnoreCase
            )
        );

        idField.ToDescriptor(discoveryContext.DescriptorContext).ID(entityType.Name).ToDefinition();
    }
}
