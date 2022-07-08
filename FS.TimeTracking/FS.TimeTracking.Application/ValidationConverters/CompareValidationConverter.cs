using FS.TimeTracking.Core.Extensions;
using FS.TimeTracking.Core.Interfaces.Application.ValidationConverters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace FS.TimeTracking.Application.ValidationConverters;

/// <inheritdoc />
public class CompareValidationConverter : IValidationDescriptionConverter
{
    /// <inheritdoc />
    public IEnumerable<Type> SupportedValidationAttributes { get; } = new[] { typeof(CompareAttribute) };

    /// <inheritdoc />
    public JObject Convert(CustomAttributeData attribute, string errorI18NPrefix)
    {
        var otherProperty = ((string)attribute.ConstructorArguments[0].Value).LowercaseFirstChar();

        return new JObject
        {
            new JProperty("type", "compare"),
            new JProperty("otherProperty", otherProperty),
        };
    }
}