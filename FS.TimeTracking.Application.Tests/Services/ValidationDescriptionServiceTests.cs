using Autofac.Extras.FakeItEasy;
using FluentAssertions;
using FS.TimeTracking.Application.Services;
using FS.TimeTracking.Application.Tests.DTOs;
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
            autoFake.Provide<IValidationDescriptionService, ValidationDescriptionService<TestDto, RequiredValidationConverter>>();
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
            var validationDescriptionService = autoFake.Resolve<ValidationDescriptionService<TestDto, RequiredValidationConverter>>();

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
            { nameof(TestDto.Required), new[] { new { type = "required" } } },

            { nameof(TestDto.MinLength), new[] { new { type = "length", min = 2 } } },
            { nameof(TestDto.MaxLength), new[] { new { type = "length", max = 4 } } },
            { nameof(TestDto.StringLength), new[] { new { type = "length", min = 2, max = 4 } } },
            { nameof(TestDto.StringLengthMax), new[] { new { type = "length", max = 4 } } },

            { nameof(TestDto.RangeInt), new[] { new { type = "range", min = 2, max = 4 } } },
            { nameof(TestDto.RangeIntMin), new[] { new { type = "range", min = 2 } } },
            { nameof(TestDto.RangeIntMax), new[] { new { type = "range", max = 4 } } },
            { nameof(TestDto.RangeDouble), new[] { new { type = "range", min = 2.0, max = 4.0 } } },
            { nameof(TestDto.RangeDoubleMin), new[] { new { type = "range", min = 2.0 } } },
            { nameof(TestDto.RangeDoubleMax), new[] { new { type = "range", max = 4.0 } } },
            { nameof(TestDto.RangeDate), new[] { new { type = "range", min = "2020-01-01T00:00:00", max = "2020-01-31T00:00:00" } } },
            { nameof(TestDto.RangeDateMin), new[] { new { type = "range", min = "2020-01-01T00:00:00" } } },
            { nameof(TestDto.RangeDateMax), new[] { new { type = "range", max = "2020-01-31T00:00:00" } } },

            { nameof(TestDto.Compare), new[] { new { type = "compare", otherProperty = "required" } } },

            { nameof(TestDto.MultiValidation), new object[] { new { type = "required" }, new { type = "length", min = 4 }, new { type = "length", min = 2, max = 4 } } },

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
