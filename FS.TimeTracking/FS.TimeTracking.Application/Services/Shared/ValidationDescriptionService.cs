using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FS.TimeTracking.Shared.Attributes;
using FS.TimeTracking.Shared.Extensions;
using FS.TimeTracking.Shared.Interfaces.Application.Services;
using FS.TimeTracking.Shared.Interfaces.Application.Services.Shared;
using FS.TimeTracking.Shared.Interfaces.Application.ValidationConverters;
using Newtonsoft.Json.Linq;

namespace FS.TimeTracking.Application.Services.Shared
{
    /// <inheritdoc />
    public class ValidationDescriptionService<TModelAssembly, TConverterAssembly> : IValidationDescriptionService
    {
        internal readonly IValidationDescriptionConverter[] ValidationDescriptionConverters = GetValidationDescriptionConverters();

        /// <inheritdoc />
        public Task<JObject> GetValidationDescriptions()
        {
            var typeValidationDescriptions = GetTypesWithValidationDescriptionAttribute<TModelAssembly>()
                .Select(GetValidationDescriptions)
                .Where(x => x != null)
                .ToList();

            return Task.FromResult(new JObject { typeValidationDescriptions });
        }

        private static IValidationDescriptionConverter[] GetValidationDescriptionConverters()
        {
            return typeof(TConverterAssembly)
                .Assembly
                .GetTypes()
                .Where(x => x.GetInterface(nameof(IValidationDescriptionConverter)) != null)
                .Select(Activator.CreateInstance)
                .Cast<IValidationDescriptionConverter>()
                .Where(x => x != null)
                .ToArray();
        }

        private static IEnumerable<Type> GetTypesWithValidationDescriptionAttribute<TAssembly>()
            => typeof(TAssembly)
                .Assembly
                .GetTypes()
                .Where(HasValidationDescriptionAttribute);

        private static bool HasValidationDescriptionAttribute(Type type)
            => type.HasAttributeValue<ValidationDescriptionAttribute>(x => x.Enabled);

        private JToken GetValidationDescriptions(Type type)
        {
            var propertyValidationDescriptions = type
                .GetProperties()
                .Select(GetValidationDescriptions)
                .Where(x => x != null)
                .ToList();

            var validationDescriptions = new JObject { propertyValidationDescriptions };
            return new JProperty(type.Name, validationDescriptions);
        }

        internal JProperty GetValidationDescriptions(PropertyInfo property)
        {
            var propertyResult = new JArray();

            var validationDescriptions = property
                .CustomAttributes
                .Select(GetValidationDescriptions)
                .Where(x => x != null)
                .ToList();

            propertyResult.Add(validationDescriptions);

            var propertyType = property.PropertyType;
            var hasNestedValidationType = HasValidationDescriptionAttribute(propertyType);
            if (hasNestedValidationType)
            {
                propertyResult.Add(new JObject
                {
                    new JProperty("type", "nested"),
                    new JProperty("class", propertyType.Name)
                });
            }

            var propertyName = property.Name.LowercaseFirstChar();
            return new JProperty(propertyName, propertyResult);
        }

        private JObject GetValidationDescriptions(CustomAttributeData attribute)
        {
            var converter = ValidationDescriptionConverters
                .FirstOrDefault(x => x.SupportedValidationAttributes.Any(a => a.FullName == attribute.AttributeType.FullName));

            var attributeValidation = converter?.Convert(attribute, "Validations.");
            return attributeValidation;
        }
    }
}
