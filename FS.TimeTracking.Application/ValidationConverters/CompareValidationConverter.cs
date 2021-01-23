using FS.TimeTracking.Shared.Extensions;
using FS.TimeTracking.Shared.Interfaces.Application.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace FS.TimeTracking.Application.ValidationConverters
{
    public class CompareValidationConverter : ValidationDescriptionConverter, IValidationDescriptionConverter
    {
        public IEnumerable<Type> SupportedValidationAttributes { get; } = new[] { typeof(CompareAttribute) };

        public JObject Convert(CustomAttributeData attribute, string errorI18NPrefix)
        {
            var otherProperty = ((string)attribute.ConstructorArguments[0].Value).LowercaseFirstChar();

            return new JObject
            {
                new JProperty("type", "compare"),
                new JProperty("otherProperty", otherProperty),
                GetErrorArgument(attribute, errorI18NPrefix, "MaxLength")
            };
        }
    }
}
