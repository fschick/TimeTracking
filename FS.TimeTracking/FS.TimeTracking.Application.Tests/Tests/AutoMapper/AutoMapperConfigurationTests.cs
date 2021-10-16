using AutoMapper;
using FS.TimeTracking.Application.AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FS.TimeTracking.Application.Tests.Tests.AutoMapper
{
    [TestClass]
    public class AutoMapperConfigurationTests
    {
        [TestMethod]
        public void AllMappingConfigurations_ShouldBeValid()
        {
            new MapperConfiguration(cfg => cfg.AddProfile<TimeTrackingAutoMapper>())
                .CreateMapper()
                .ConfigurationProvider
                .AssertConfigurationIsValid();
        }
    }
}
