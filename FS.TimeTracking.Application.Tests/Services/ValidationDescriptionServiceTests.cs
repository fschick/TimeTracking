using Autofac.Extras.FakeItEasy;
using FluentAssertions;
using FS.TimeTracking.Application.Services;
using FS.TimeTracking.Application.ValidationConverters;
using FS.TimeTracking.Shared.Interfaces.Application.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    }
}
