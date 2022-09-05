using FS.TimeTracking.Abstractions.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace FS.TimeTracking.Abstractions.Attributes;

/// <summary>
/// Provides an attribute that compares two properties
/// Implements the <see cref="ValidationAttribute" />
/// </summary>
/// <seealso cref="ValidationAttribute" />
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class CompareToAttribute : CompareAttribute
{
    private readonly ComparisonType _comparisonType;

    /// <inheritdoc />
    public override object TypeId { get; } = new object();

    /// <summary>
    /// Initializes a new instance of the <see cref="CompareToAttribute"/> class.
    /// </summary>
    /// <param name="comparisonType">Type of the comparison.</param>
    /// <param name="otherProperty">The other property.</param>
    public CompareToAttribute(ComparisonType comparisonType, string otherProperty)
        : base(otherProperty)
        => _comparisonType = comparisonType;

    /// <inheritdoc />
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var otherProperty = validationContext.ObjectType.GetProperty(OtherProperty);
        if (otherProperty == null)
            return new ValidationResult($"No other property with name '{OtherProperty}' could be found.");

        var otherValue = otherProperty.GetValue(validationContext.ObjectInstance);

        if (value?.GetType().IsAssignableTo(typeof(IComparable)) == false || otherValue?.GetType().IsAssignableTo(typeof(IComparable)) == false)
            throw new InvalidOperationException($"The properties to compare must implement interface {nameof(IComparable)}");

        if (value == null || otherValue == null)
            return ValidationResult.Success;

        return ValidateComparison((IComparable)value, (IComparable)otherValue, otherProperty.Name, _comparisonType);
    }

    private static ValidationResult ValidateComparison(IComparable value, IComparable otherValue, string otherPropertyName, ComparisonType comparisonType)
    {
        switch (comparisonType)
        {
            case ComparisonType.Equal:
                if (value.CompareTo(otherValue) != 0)
                    return new ValidationResult($"Value is not equal to property {otherPropertyName}.");
                break;
            case ComparisonType.NotEqual:
                if (value.CompareTo(otherValue) == 0)
                    return new ValidationResult($"Value is equal to property {otherPropertyName}.");
                break;
            case ComparisonType.GreaterThan:
                if (value.CompareTo(otherValue) <= 0)
                    return new ValidationResult($"Value is not grater than property {otherPropertyName}.");
                break;
            case ComparisonType.GreaterThanOrEqual:
                if (value.CompareTo(otherValue) < 0)
                    return new ValidationResult($"Value is not grater or equal than property {otherPropertyName}.");
                break;
            case ComparisonType.LessThan:
                if (value.CompareTo(otherValue) >= 0)
                    return new ValidationResult($"Value is not less than property {otherPropertyName}.");
                break;
            case ComparisonType.LessThanOrEqual:
                if (value.CompareTo(otherValue) > 0)
                    return new ValidationResult($"Value is not less or equal than property {otherPropertyName}.");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(comparisonType), comparisonType, null);
        }

        return ValidationResult.Success;
    }
}