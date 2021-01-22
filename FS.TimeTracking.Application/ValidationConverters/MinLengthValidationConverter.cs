using FS.TimeTracking.Shared.Interfaces.Application.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace FS.TimeTracking.Application.ValidationConverters
{
    public class MinLengthValidationConverter : ValidationDescriptionConverter, IValidationDescriptionConverter
    {
        public IEnumerable<Type> SupportedValidationAttributes { get; } = new[] { typeof(MinLengthAttribute) };

        public IEnumerable<JToken> Convert(CustomAttributeData attribute, string errorI18NPrefix)
        {
            var result = new JObject();
            result.Add(new JProperty("min", (int)attribute.ConstructorArguments[0].Value!));
            result.Add(GetErrorArgument(attribute, errorI18NPrefix, "MinLength"));
            return new JProperty("length", result);
        }
    }
}
