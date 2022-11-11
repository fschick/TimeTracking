using FS.TimeTracking.Core.Interfaces.Application.ValidationConverters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace FS.TimeTracking.Application.ValidationConverters;

/// <inheritdoc />
public class EmailValidationConverter : IValidationDescriptionConverter
{
    /// <inheritdoc />
    public IEnumerable<Type> SupportedValidationAttributes { get; } = new[] { typeof(EmailAddressAttribute) };

    /// <inheritdoc />
    public JObject Convert(CustomAttributeData attribute, string errorI18NPrefix)
        => new JObject
        {
            new JProperty("type", "email"),
        };
}