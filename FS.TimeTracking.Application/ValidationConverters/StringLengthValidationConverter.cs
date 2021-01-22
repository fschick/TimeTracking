using FS.TimeTracking.Shared.Interfaces.Application.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace FS.TimeTracking.Application.ValidationConverters
{
    public class StringLengthValidationConverter : ValidationDescriptionConverter, IValidationDescriptionConverter
    {
        public IEnumerable<Type> SupportedValidationAttributes { get; } = new[] { typeof(StringLengthAttribute) };

        public IEnumerable<JToken> Convert(CustomAttributeData attribute, string errorI18NPrefix)
        {
            var result = new JObject();

            var minArgument = attribute.NamedArguments!.FirstOrDefault(x => x.MemberName == nameof(StringLengthAttribute.MinimumLength)).TypedValue.Value;
            if (minArgument != null)
                result.Add(new JProperty("min", (int)minArgument));
            result.Add(new JProperty("max", (int)attribute.ConstructorArguments[0].Value!));
            result.Add(GetErrorArgument(attribute, errorI18NPrefix, "StringLength"));

            return new JProperty("length", result);
        }
    }
}
