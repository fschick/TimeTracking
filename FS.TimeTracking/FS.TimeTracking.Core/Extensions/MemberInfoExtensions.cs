using System;
using System.Linq.Expressions;
using System.Reflection;

namespace FS.TimeTracking.Core.Extensions;

/// <summary>Extensions for the <see cref="T:System.Reflection.MemberInfo"/> class.</summary>
public static class MemberInfoExtensions
{
    /// <summary>
    /// Determines whether an attribute satisfy a condition.
    /// </summary>
    /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
    /// <param name="memberInfo">The member info.</param>
    /// <param name="predicate">A function to test the attribute for a condition.</param>
    /// <returns>true if the attribute passes the test in the specified predicate; otherwise, false</returns>
    public static bool HasAttributeValue<TAttribute>(this MemberInfo memberInfo, Expression<Func<TAttribute, bool>> predicate) where TAttribute : Attribute
    {
        var attribute = memberInfo.GetCustomAttribute<TAttribute>();
        if (attribute == null)
            return false;

        var func = predicate.Compile();
        return func(attribute);
    }
}