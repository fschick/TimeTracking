using FluentAssertions;
using FS.TimeTracking.Application.Services;
using FS.TimeTracking.Application.ValidationConverters;
using FS.TimeTracking.Shared.Interfaces.Application.Converters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FS.TimeTracking.Application.Tests.ValidationConverters
{
    [TestClass]
    public class MaxLengthValidationConverterTests
    {
        [DataTestMethod]
        [ValidationDescriptionConverterTestSource]
        public void WhenAttributeIsConvertedToDescription_ResultMatchesExpected(PropertyInfo property, CustomAttributeData attribute, IValidationDescriptionConverter converter)
        {
            var expected = _expectedValidationDescriptions[property.Name];
            var expectedJson = JsonConvert.SerializeObject(expected, Formatting.Indented);
            if (converter == null)
                throw new InvalidOperationException($"No converter for attribute '{attribute.AttributeType.Name}' found.");

            // Act
            var validationDescription = converter.Convert(attribute, "");
            var validationDescriptionJson = new JObject(validationDescription).ToString(Formatting.Indented);

            // Check
            validationDescriptionJson.Should().BeEquivalentTo(expectedJson);
        }

        private readonly Dictionary<string, object> _expectedValidationDescriptions = new()
        {
            { nameof(TestDto.Required), new { required = new { message = "" } } },

            { nameof(TestDto.MinLength), new { length = new { min = 5, message = "" } } },
            { nameof(TestDto.MaxLength), new { length = new { max = 5, message = "" } } },
            { nameof(TestDto.StringLength), new { length = new { min = 1, max = 5, message = "" } } },
            { nameof(TestDto.StringLengthMax), new { length = new { max = 5, message = "" } } },

            { nameof(TestDto.RangeInt), new { range = new { min = 1, max = 5, message = "" } } },
            { nameof(TestDto.RangeIntMin), new { range = new { min = 1, message = "" } } },
            { nameof(TestDto.RangeIntMax), new { range = new { max = 5, message = "" } } },
            { nameof(TestDto.RangeDouble), new { range = new { min = 1.0, max = 5.0, message = "" } } },
            { nameof(TestDto.RangeDoubleMin), new { range = new { min = 1.0, message = "" } } },
            { nameof(TestDto.RangeDoubleMax), new { range = new { max = 5.0, message = "" } } },
            { nameof(TestDto.RangeDate), new { range = new { min = "2020-01-01T00:00:00", max = "2020-01-31T00:00:00", message = "" } } },
            { nameof(TestDto.RangeDateMin), new { range = new { min = "2020-01-01T00:00:00", message = "" } } },
            { nameof(TestDto.RangeDateMax), new { range = new { max = "2020-01-31T00:00:00", message = "" } } },

            { nameof(TestDto.Compare), new { compare = new { otherProperty = "required", message = "" } } },
        };

        public class ValidationDescriptionConverterTestSourceAttribute : Attribute, ITestDataSource
        {
            public IEnumerable<object[]> GetData(MethodInfo methodInfo)
            {
                var validationDescriptionConverters = ValidationDescriptionService<TestDto, ValidationDescriptionConverter>.ValidationDescriptionConverters;

                var result = typeof(TestDto)
                    .GetProperties()
                    .Select(property =>
                    {
                        var attribute = property.GetCustomAttributesData().Single();
                        var converter = validationDescriptionConverters
                            .FirstOrDefault(x => x.SupportedValidationAttributes.Any(a => a.FullName == attribute.AttributeType.FullName));
                        return new object[] { property, attribute, converter };
                    })
                    .ToArray();

                return result;
            }

            public string GetDisplayName(MethodInfo methodInfo, object[] data)
                => ((PropertyInfo)data[0]).Name;
        }
    }
}
