using AutoMapper;
using AutoMapper.Configuration;
using FluentAssertions;
using FluentAssertions.Execution;
using FS.TimeTracking.Application.AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace FS.TimeTracking.Application.Tests.Tests
{
    [TestClass]
    public class AutoMapperConfigurationTests
    {
        [TestMethod]
        public void AllMappingConfigurations_ShouldBeValid()
        {
            var timeTrackingConfiguration = (IProfileConfiguration)new TimeTrackingProfile();
            var mappingValidations = timeTrackingConfiguration
                .TypeMapConfigs
                .Select(x => new MapperConfiguration(cc => cc.CreateMap(x.SourceType, x.DestinationType)))
                .Select(mapping => (Action)mapping.AssertConfigurationIsValid)
                .ToList();

            using (new AssertionScope())
                foreach (var mappingValidation in mappingValidations)
                    mappingValidation.Should().NotThrow();
        }
    }
}
