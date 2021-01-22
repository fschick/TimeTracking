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

        public IEnumerable<JToken> Convert(CustomAttributeData attribute, string errorI18NPrefix)
        {
            var result = new JObject();
            var otherProperty = ((string)attribute.ConstructorArguments[0].Value).LowercaseFirstChar();
            result.Add(new JProperty("otherProperty", otherProperty));
            result.Add(GetErrorArgument(attribute, errorI18NPrefix, "MaxLength"));
            return new JProperty("compare", result);
        }
    }
}
