using FS.TimeTracking.Core.Interfaces.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace FS.TimeTracking.Application.Tests.Extensions;

public static class IdEntityModelExtensions
{
    /// <summary>
    /// Removes (sub)entities referenced more than once, compared by it's ID.
    /// Prevents EF error 'The instance of entity type '{TEntity}' cannot be tracked because another instance with the same key value'
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity.</typeparam>
    /// <param name="items">The entities to act on.</param>
    /// <param name="knownIds">Should not be used. Required internal for recursion.</param>
    public static List<TEntity> EliminateDuplicateReferences<TEntity>(this List<TEntity> items, ConcurrentDictionary<Type, List<Guid>> knownIds = null)
    {
        if (items?.Any() != true)
            return items;

        knownIds ??= new ConcurrentDictionary<Type, List<Guid>>();

        var nestedProperties = items.First().GetType()
            .GetProperties()
            .Where(property => property.PropertyType.IsAssignableTo(typeof(IIdEntityModel)))
            .ToList();

        foreach (var property in nestedProperties)
        {
            var knownEntityIds = knownIds.GetOrAdd(property.PropertyType, new List<Guid>());

            foreach (var item in items)
            {
                var entity = (IIdEntityModel)property.GetValue(item);
                var entityId = entity?.Id;
                if (entityId == null)
                    continue;

                if (knownEntityIds.Any(knownId => knownId == entityId))
                    property.SetValue(item, null);
                else
                    knownEntityIds.Add(entityId.Value);

                EliminateDuplicateReferences(new List<IIdEntityModel> { entity }, knownIds);
            }
        }

        return items;
    }
}