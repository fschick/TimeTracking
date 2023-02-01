using Autofac.Extras.FakeItEasy;
using FluentAssertions;
using FS.TimeTracking.Application.Services.Shared;
using FS.TimeTracking.Application.Tests.Models;
using FS.TimeTracking.Application.ValidationConverters;
using FS.TimeTracking.Core.Interfaces.Application.Services.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FS.TimeTracking.Application.Tests.Tests.Services;

[TestClass, ExcludeFromCodeCoverage]
public class ValidationDescriptionServiceTests
{
    [TestMethod]
    public async Task WhenValidationDescriptionsAreGenerated_ResultIsNotEmpty()
    {
        // Prepare
        using var autoFake = new AutoFake();
        autoFake.Provide<IValidationDescriptionApiService, ValidationDescriptionService<ValidationTestDto, RequiredValidationConverter>>();
        var validationDescriptionService = autoFake.Resolve<IValidationDescriptionApiService>();

        // Act
        var validationDescriptions = await validationDescriptionService.GetValidationDescriptions();

        // Check
        validationDescriptions.ToString().Should().NotBeNullOrEmpty();
    }

    [DataTestMethod]
    [PropertyValidationDescriptionConverterTestSource]
    public void WhenPropertyIsConvertedToDescription_ResultMatchesExpectedValidationDescription(PropertyInfo property)
    {
        using var autoFake = new AutoFake();
        var validationDescriptionService = autoFake.Resolve<ValidationDescriptionService<ValidationTestDto, RequiredValidationConverter>>();

        // Prepare
        if (!_expectedValidationDescriptions.TryGetValue(property.Name, out var expected))
            Assert.Fail($"No test data found for property {property.Name}");
        var expectedJson = JsonConvert.SerializeObject(expected, Formatting.Indented);

        // Act
        var validationDescriptions = validationDescriptionService.GetValidationDescriptions(property);
        var validationDescriptionJson = validationDescriptions.Value.ToString(Formatting.Indented);

        // Check
        validationDescriptionJson.Should().BeEquivalentTo(expectedJson);
    }

    private readonly Dictionary<string, object> _expectedValidationDescriptions = new()
    {
        { nameof(ValidationTestDto.Required), new[] { new { type = "required" } } },

        { nameof(ValidationTestDto.MinLength), new[] { new { type = "length", min = 2 } } },
        { nameof(ValidationTestDto.MaxLength), new[] { new { type = "length", max = 4 } } },
        { nameof(ValidationTestDto.StringLength), new[] { new { type = "length", min = 2, max = 4 } } },
        { nameof(ValidationTestDto.StringLengthMax), new[] { new { type = "length", max = 4 } } },

        { nameof(ValidationTestDto.RangeInt), new[] { new { type = "range", min = 2, max = 4 } } },
        { nameof(ValidationTestDto.RangeIntMin), new[] { new { type = "range", min = 2 } } },
        { nameof(ValidationTestDto.RangeIntMax), new[] { new { type = "range", max = 4 } } },
        { nameof(ValidationTestDto.RangeDouble), new[] { new { type = "range", min = 2.0, max = 4.0 } } },
        { nameof(ValidationTestDto.RangeDoubleMin), new[] { new { type = "range", min = 2.0 } } },
        { nameof(ValidationTestDto.RangeDoubleMinInfinity), new[] { new { type = "range", min = 2.0 } } },
        { nameof(ValidationTestDto.RangeDoubleMax), new[] { new { type = "range", max = 4.0 } } },
        { nameof(ValidationTestDto.RangeDoubleMaxInfinity), new[] { new { type = "range", max = 4.0 } } },
        { nameof(ValidationTestDto.RangeDate), new[] { new { type = "range", min = "2020-01-01T00:00:00", max = "2020-01-31T00:00:00" } } },
        { nameof(ValidationTestDto.RangeDateMin), new[] { new { type = "range", min = "2020-01-01T00:00:00" } } },
        { nameof(ValidationTestDto.RangeDateMax), new[] { new { type = "range", max = "2020-01-31T00:00:00" } } },

        { nameof(ValidationTestDto.Compare), new[] { new { type = "compare", otherProperty = "required" } } },
        { nameof(ValidationTestDto.CompareToEqual), new[] { new { type = "compareTo", comparisonType = "equal", otherProperty = "required" } } },
        { nameof(ValidationTestDto.CompareToNotEqual), new[] { new { type = "compareTo", comparisonType = "notEqual", otherProperty = "required" } } },
        { nameof(ValidationTestDto.CompareToLessThan), new[] { new { type = "compareTo", comparisonType = "lessThan", otherProperty = "required" } } },
        { nameof(ValidationTestDto.CompareToLessThanOrEqual), new[] { new { type = "compareTo", comparisonType = "lessThanOrEqual", otherProperty = "required" } } },
        { nameof(ValidationTestDto.CompareToGreaterThan), new[] { new { type = "compareTo", comparisonType = "greaterThan", otherProperty = "required" } } },
        { nameof(ValidationTestDto.CompareToGreaterThanOrEqual), new[] { new { type = "compareTo", comparisonType = "greaterThanOrEqual", otherProperty = "required" } } },

        { nameof(ValidationTestDto.MultiValidation), new object[] { new { type = "required" }, new { type = "length", min = 4 }, new { type = "length", min = 2, max = 4 } } },

        { nameof(ValidationTestDto.NestedOuter), new[] { new { type = "nested", @class = "TestNestedInnerDto" } } },
        { nameof(ValidationTestDto.NestedInner), new[] { new { type = "nested", @class = "TestNestedOuterDto" } } },
    };

    public class PropertyValidationDescriptionConverterTestSource : Attribute, ITestDataSource
    {
        public IEnumerable<object[]> GetData(MethodInfo methodInfo)
            => typeof(ValidationTestDto)
                .GetProperties()
                .Select(property => new[] { property })
                .ToArray();

        public string GetDisplayName(MethodInfo methodInfo, object[] data)
            => ((PropertyInfo)data[0]).Name;
    }
}