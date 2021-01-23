using Autofac.Extras.FakeItEasy;
using FluentAssertions;
using FS.TimeTracking.Application.Services;
using FS.TimeTracking.Application.ValidationConverters;
using FS.TimeTracking.Shared.Interfaces.Application.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FS.TimeTracking.Application.Tests.Services
{
    [TestClass]
    public class ValidationDescriptionServiceTests
    {
        [TestMethod]
        public async Task WhenValidationDescriptionsAreGenerated_ResultIsNotEmpty()
        {
            // Prepare
            using var autoFake = new AutoFake();
            autoFake.Provide<IValidationDescriptionService, ValidationDescriptionService<TestDto, ValidationDescriptionConverter>>();
            var validationDescriptionService = autoFake.Resolve<IValidationDescriptionService>();

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
            var validationDescriptionService = autoFake.Resolve<ValidationDescriptionService<TestDto, ValidationDescriptionConverter>>();

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
            { nameof(TestDto.Required), new[] { new { type = "required", message = "" } } },

            { nameof(TestDto.MinLength), new[] { new { type = "length", min = 5, message = "" } } },
            { nameof(TestDto.MaxLength), new[] { new { type = "length", max = 5, message = "" } } },
            { nameof(TestDto.StringLength), new[] { new { type = "length", min = 1, max = 5, message = "" } } },
            { nameof(TestDto.StringLengthMax), new[] { new { type = "length", max = 5, message = "" } } },

            { nameof(TestDto.RangeInt), new[] { new { type = "range", min = 1, max = 5, message = "" } } },
            { nameof(TestDto.RangeIntMin), new[] { new { type = "range", min = 1, message = "" } } },
            { nameof(TestDto.RangeIntMax), new[] { new { type = "range", max = 5, message = "" } } },
            { nameof(TestDto.RangeDouble), new[] { new { type = "range", min = 1.0, max = 5.0, message = "" } } },
            { nameof(TestDto.RangeDoubleMin), new[] { new { type = "range", min = 1.0, message = "" } } },
            { nameof(TestDto.RangeDoubleMax), new[] { new { type = "range", max = 5.0, message = "" } } },
            { nameof(TestDto.RangeDate), new[] { new { type = "range", min = "2020-01-01T00:00:00", max = "2020-01-31T00:00:00", message = "" } } },
            { nameof(TestDto.RangeDateMin), new[] { new { type = "range", min = "2020-01-01T00:00:00", message = "" } } },
            { nameof(TestDto.RangeDateMax), new[] { new { type = "range", max = "2020-01-31T00:00:00", message = "" } } },

            { nameof(TestDto.Compare), new[] { new { type = "compare", otherProperty = "required", message = "" } } },

            { nameof(TestDto.MultiValidation), new object[] { new { type = "required", message = "" }, new { type = "length", min = 5, message = "" }, new { type = "length", min = 1, max = 5, message = "" } } },

            { nameof(TestDto.NestedOuter), new[] { new { type = "nested", @class = "TestNestedInnerDto" } } },
            { nameof(TestDto.NestedInner), new[] { new { type = "nested", @class = "TestNestedOuterDto" } } },
        };

        public class PropertyValidationDescriptionConverterTestSource : Attribute, ITestDataSource
        {
            public IEnumerable<object[]> GetData(MethodInfo methodInfo)
                => typeof(TestDto)
                    .GetProperties()
                    .Select(property => new[] { property })
                    .ToArray();

            public string GetDisplayName(MethodInfo methodInfo, object[] data)
                => ((PropertyInfo)data[0]).Name;
        }
    }
}
