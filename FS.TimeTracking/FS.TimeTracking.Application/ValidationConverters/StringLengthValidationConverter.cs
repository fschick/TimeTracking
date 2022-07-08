using FS.TimeTracking.Core.Interfaces.Application.ValidationConverters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace FS.TimeTracking.Application.ValidationConverters;

/// <inheritdoc />
public class StringLengthValidationConverter : IValidationDescriptionConverter
{
    /// <inheritdoc />
    public IEnumerable<Type> SupportedValidationAttributes { get; } = new[] { typeof(StringLengthAttribute) };

    /// <inheritdoc />
    public JObject Convert(CustomAttributeData attribute, string errorI18NPrefix)
    {
        var result = new JObject
        {
            new JProperty("type", "length")
        };

        var minArgument = attribute.NamedArguments!.FirstOrDefault(x => x.MemberName == nameof(StringLengthAttribute.MinimumLength)).TypedValue.Value;
        if (minArgument != null)
            result.Add(new JProperty("min", (int)minArgument));
        result.Add(new JProperty("max", (int)attribute.ConstructorArguments[0].Value!));

        return result;
    }
}