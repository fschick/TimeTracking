using FS.TimeTracking.Shared.Interfaces.Application.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace FS.TimeTracking.Application.ValidationConverters
{
    public class MaxLengthValidationConverter : ValidationDescriptionConverter, IValidationDescriptionConverter
    {
        public IEnumerable<Type> SupportedValidationAttributes { get; } = new[] { typeof(MaxLengthAttribute) };

        public IEnumerable<JToken> Convert(CustomAttributeData attribute, string errorI18NPrefix)
        {
            var result = new JObject();
            result.Add(new JProperty("max", (int)attribute.ConstructorArguments[0].Value!));
            result.Add(GetErrorArgument(attribute, errorI18NPrefix, "MaxLength"));
            return new JProperty("length", result);
        }
    }
}
