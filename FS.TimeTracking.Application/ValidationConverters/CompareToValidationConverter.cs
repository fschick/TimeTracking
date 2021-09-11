using FS.TimeTracking.Shared.Attributes;
using FS.TimeTracking.Shared.Extensions;
using FS.TimeTracking.Shared.Models.Shared;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using FS.TimeTracking.Shared.Interfaces.Application.ValidationConverters;

namespace FS.TimeTracking.Application.ValidationConverters
{
    /// <inheritdoc />
    public class CompareToValidationConverter : IValidationDescriptionConverter
    {
        /// <inheritdoc />
        public IEnumerable<Type> SupportedValidationAttributes { get; } = new[] { typeof(CompareToAttribute) };

        /// <inheritdoc />
        public JObject Convert(CustomAttributeData attribute, string errorI18NPrefix)
        {
            var comparisonType = (ComparisonType)attribute.ConstructorArguments[0].Value!;
            var otherProperty = ((string)attribute.ConstructorArguments[1].Value).LowercaseFirstChar();

            return new JObject
            {
                new JProperty("type", "compareTo"),
                new JProperty("comparisonType", comparisonType.ToString().LowercaseFirstChar()),
                new JProperty("otherProperty", otherProperty),
            };
        }
    }
}
