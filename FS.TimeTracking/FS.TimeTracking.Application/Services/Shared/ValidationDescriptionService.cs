using FS.TimeTracking.Abstractions.Attributes;
using FS.TimeTracking.Core.Extensions;
using FS.TimeTracking.Core.Interfaces.Application.Services.Shared;
using FS.TimeTracking.Core.Interfaces.Application.ValidationConverters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FS.TimeTracking.Application.Services.Shared;

/// <inheritdoc />
public class ValidationDescriptionService<TModelAssembly, TConverterAssembly> : IValidationDescriptionApiService
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

        AddPropertyValidations(property, propertyResult);
        AddNestedValidations(property.PropertyType, propertyResult);

        var propertyName = property.Name.LowercaseFirstChar();
        return new JProperty(propertyName, propertyResult);
    }

    private void AddPropertyValidations(PropertyInfo property, JArray propertyResult)
    {
        var validationDescriptions = property
                    .CustomAttributes
                    .Select(GetAttributeValidationDescriptions)
                    .Where(x => x != null)
                    .ToList();

        propertyResult.Add(validationDescriptions);
    }

    private static void AddNestedValidations(Type propertyType, JArray propertyResult)
    {
        var isNestedValidationType = HasValidationDescriptionAttribute(propertyType);
        if (isNestedValidationType)
        {
            propertyResult.Add(new JObject
            {
                new JProperty("type", "nested"),
                new JProperty("class", propertyType.Name)
            });
        }

        var isNestedList = propertyType.IsGenericType && typeof(IEnumerable<object>).IsAssignableFrom(propertyType);
        if (isNestedList)
        {
            var nestedListType = propertyType.GetGenericArguments()[0];
            var isNestedValidationList = HasValidationDescriptionAttribute(nestedListType);
            if (isNestedValidationList)
            {
                propertyResult.Add(new JObject
                {
                    new JProperty("type", "list"),
                    new JProperty("class", nestedListType.Name)
                });
            }
        }
    }

    private JObject GetAttributeValidationDescriptions(CustomAttributeData attribute)
    {
        var converter = ValidationDescriptionConverters
            .FirstOrDefault(x => x.SupportedValidationAttributes.Any(a => a.FullName == attribute.AttributeType.FullName));

        var attributeValidation = converter?.Convert(attribute, "Validations.");
        return attributeValidation;
    }
}