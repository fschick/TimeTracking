using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace FS.TimeTracking.Core.Interfaces.Application.ValidationConverters;

/// <summary>
/// Validation description converter
/// </summary>
public interface IValidationDescriptionConverter
{
    /// <summary>
    /// Validation attributes convertible to descriptions.
    /// </summary>
    IEnumerable<Type> SupportedValidationAttributes { get; }

    /// <summary>
    /// Converts the specified attribute to a validation description.
    /// </summary>
    /// <param name="attribute">The attribute to convert.</param>
    /// <param name="errorI18NPrefix">The error i18n prefix.</param>
    JObject Convert(CustomAttributeData attribute, string errorI18NPrefix);
}