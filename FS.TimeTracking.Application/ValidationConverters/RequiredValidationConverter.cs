using FS.TimeTracking.Shared.Interfaces.Application.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace FS.TimeTracking.Application.ValidationConverters
{
    /// <inheritdoc />
    public class RequiredValidationConverter : IValidationDescriptionConverter
    {
        /// <inheritdoc />
        public IEnumerable<Type> SupportedValidationAttributes { get; } = new[] { typeof(RequiredAttribute) };

        /// <inheritdoc />
        public JObject Convert(CustomAttributeData attribute, string errorI18NPrefix)
            => new JObject
            {
                new JProperty("type", "required"),
            };
    }
}
