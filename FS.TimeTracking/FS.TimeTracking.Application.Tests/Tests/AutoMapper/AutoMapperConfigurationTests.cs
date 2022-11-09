using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using FS.TimeTracking.Application.AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Application.Tests.Tests.AutoMapper;

[TestClass, ExcludeFromCodeCoverage]
public class AutoMapperConfigurationTests
{
    [TestMethod]
    public void AllMappingConfigurations_ShouldBeValid()
    {
        var mapperConfiguration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<TimeTrackingAutoMapper>();
            cfg.AddExpressionMapping();
        });

        mapperConfiguration
            .CreateMapper()
            .ConfigurationProvider
            .AssertConfigurationIsValid();
    }
}