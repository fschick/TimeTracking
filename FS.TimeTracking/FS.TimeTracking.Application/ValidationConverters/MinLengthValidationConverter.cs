using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using FS.TimeTracking.Shared.Interfaces.Application.ValidationConverters;

namespace FS.TimeTracking.Application.ValidationConverters
{
    /// <inheritdoc />
    public class MinLengthValidationConverter : IValidationDescriptionConverter
    {
        /// <inheritdoc />
        public IEnumerable<Type> SupportedValidationAttributes { get; } = new[] { typeof(MinLengthAttribute) };

        /// <inheritdoc />
        public JObject Convert(CustomAttributeData attribute, string errorI18NPrefix)
            => new JObject
            {
                new JProperty("type", "length"),
                new JProperty("min", (int)attribute.ConstructorArguments[0].Value!),
            };
    }
}
