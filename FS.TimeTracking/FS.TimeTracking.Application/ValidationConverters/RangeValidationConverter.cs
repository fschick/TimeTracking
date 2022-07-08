using FS.TimeTracking.Core.Interfaces.Application.ValidationConverters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace FS.TimeTracking.Application.ValidationConverters;

/// <inheritdoc />
public class RangeValidationConverter : IValidationDescriptionConverter
{
    /// <inheritdoc />
    public IEnumerable<Type> SupportedValidationAttributes { get; } = new[] { typeof(RangeAttribute) };

    /// <inheritdoc />
    public JObject Convert(CustomAttributeData attribute, string errorI18NPrefix)
    {
        var result = new JObject{
            new JProperty("type", "range")
        };

        var (minValue, maxValue) = GetConversionParameter(attribute);
        if (minValue != null)
            result.Add(new JProperty("min", minValue));
        if (maxValue != null)
            result.Add(new JProperty("max", maxValue));

        return result;
    }

    private static (IComparable, IComparable) GetConversionParameter(CustomAttributeData attribute)
    {
        IComparable minValue = null;
        IComparable maxValue = null;

        var firstConstructorArgument = attribute.ConstructorArguments[0];
        if (firstConstructorArgument.ArgumentType == typeof(int))
        {
            var minIntValue = System.Convert.ToInt32(attribute.ConstructorArguments[0].Value, CultureInfo.InvariantCulture);
            var maxIntValue = System.Convert.ToInt32(attribute.ConstructorArguments[1].Value, CultureInfo.InvariantCulture);
            minValue = minIntValue != int.MinValue ? minIntValue : null;
            maxValue = maxIntValue != int.MaxValue ? maxIntValue : null;
        }
        else if (firstConstructorArgument.ArgumentType == typeof(double))
        {
            var minDoubleValue = System.Convert.ToDouble(attribute.ConstructorArguments[0].Value, CultureInfo.InvariantCulture);
            var maxDoubleValue = System.Convert.ToDouble(attribute.ConstructorArguments[1].Value, CultureInfo.InvariantCulture);
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            minValue = (minDoubleValue == double.MinValue || double.IsNegativeInfinity(minDoubleValue)) ? null : minDoubleValue;
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            maxValue = (maxDoubleValue == double.MaxValue || double.IsPositiveInfinity(maxDoubleValue)) ? null : maxDoubleValue;
        }
        else if (typeof(IComparable).IsAssignableFrom((Type)firstConstructorArgument.Value))
        {
            var parseLimitsInInvariantCultureArgument = attribute.NamedArguments?.FirstOrDefault(x => x.MemberName == nameof(RangeAttribute.ParseLimitsInInvariantCulture));
            var parseLimitsInInvariantCulture = (bool?)parseLimitsInInvariantCultureArgument?.TypedValue.Value ?? false;
            var minRawValue = attribute.ConstructorArguments[1].Value;
            var maxRawValue = attribute.ConstructorArguments[2].Value;

            var converter = TypeDescriptor.GetConverter((Type)firstConstructorArgument.Value);

            if (minRawValue != null)
                minValue = (IComparable)(parseLimitsInInvariantCulture
                    ? converter.ConvertFromInvariantString((string)minRawValue)
                    : converter.ConvertFromString((string)minRawValue));

            if (maxRawValue != null)
                maxValue = (IComparable)(parseLimitsInInvariantCulture
                    ? converter.ConvertFromInvariantString((string)maxRawValue)
                    : converter.ConvertFromString((string)maxRawValue));
        }
        else
        {
            throw new InvalidOperationException($"The type of range parameters must implement {nameof(IComparable)}");
        }

        return (minValue, maxValue);
    }
}