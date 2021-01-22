using FS.TimeTracking.Shared.Interfaces.Application.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace FS.TimeTracking.Application.ValidationConverters
{
    public class RequiredValidationConverter : ValidationDescriptionConverter, IValidationDescriptionConverter
    {
        public IEnumerable<Type> SupportedValidationAttributes { get; } = new[] { typeof(RequiredAttribute) };

        public IEnumerable<JToken> Convert(CustomAttributeData attribute, string errorI18NPrefix)
        {
            var result = new JObject();
            result.Add(GetErrorArgument(attribute, errorI18NPrefix, "Required"));
            return new JProperty("required", result);
        }
    }
}
