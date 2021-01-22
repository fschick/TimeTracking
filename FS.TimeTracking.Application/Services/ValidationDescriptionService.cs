using FS.TimeTracking.Shared.Attributes;
using FS.TimeTracking.Shared.Extensions;
using FS.TimeTracking.Shared.Interfaces.Application.Converters;
using FS.TimeTracking.Shared.Interfaces.Application.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FS.TimeTracking.Application.Services
{
    public class ValidationDescriptionService<TModelAssembly, TConverterAssembly> : IValidationDescriptionService
    {
        [SuppressMessage("ReSharper", "StaticMemberInGenericType", Justification = "Instance per generic is intended")]
        internal static readonly List<IValidationDescriptionConverter> ValidationDescriptionConverters = GetValidationDescriptionConverters();

        public Task<JObject> GetValidationDescriptions()
        {
            var typeValidationDescriptions = GetTypesRequireValidationDescription<TModelAssembly>()
                .Select(GetValidationDescriptions)
                .Where(x => x != null)
                .ToList();

            return Task.FromResult(new JObject { typeValidationDescriptions });
        }

        private static List<IValidationDescriptionConverter> GetValidationDescriptionConverters()
        {
            return typeof(TConverterAssembly)
                .Assembly
                .GetTypes()
                .Where(x => x.GetInterface(nameof(IValidationDescriptionConverter)) != null)
                .Select(validationConverterType => (IValidationDescriptionConverter)validationConverterType.GetConstructor(Type.EmptyTypes)?.Invoke(null))
                .Where(x => x != null)
                .ToList();
        }

        private static IEnumerable<Type> GetTypesRequireValidationDescription<TAssembly>()
            => typeof(TAssembly)
                .Assembly
                .GetTypes()
                .Where(type => type.HasAttributeValue<ValidationDescriptionAttribute>(x => x.Enabled));

        private static JToken GetValidationDescriptions(Type type)
        {
            var propertyValidationDescriptions = type
                .GetProperties()
                .Select(GetValidationDescriptions)
                .Where(x => x != null)
                .ToList();

            var validationDescriptions = new JObject { propertyValidationDescriptions };
            return new JProperty(type.Name, validationDescriptions);
        }

        private static JToken GetValidationDescriptions(MemberInfo property)
        {
            var validationAttributes = property
                .CustomAttributes
                .Select(GetValidationDescriptions)
                .Where(x => x != null)
                .ToList();

            var propertyName = property.Name.LowercaseFirstChar();
            var propertyResult = new JObject { validationAttributes };
            return new JProperty(propertyName, propertyResult);
        }

        private static IEnumerable<JToken> GetValidationDescriptions(CustomAttributeData attribute)
        {
            var converter = ValidationDescriptionConverters
                .FirstOrDefault(x => x.SupportedValidationAttributes.Any(a => a.FullName == attribute.AttributeType.FullName));

            var attributeValidation = converter?.Convert(attribute, "");
            return attributeValidation;
        }
    }
}
