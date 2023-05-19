#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
using FluentAssertions;
using FS.TimeTracking.Abstractions.Constants;
using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Api.REST.Controllers.MasterData;
using FS.TimeTracking.Api.REST.Controllers.Shared;
using FS.TimeTracking.Application.Tests.Extensions;
using FS.TimeTracking.Tests.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using TestHostCall = System.Linq.Expressions.Expression<System.Func<FS.TimeTracking.Tests.Services.TestHost, System.Threading.Tasks.Task>>;

namespace FS.TimeTracking.Tests.IntegrationTests;

[TestClass, ExcludeFromCodeCoverage]
[SuppressMessage("IDisposableAnalyzers.Correctness", "IDISP013:Await in using", Justification = "False positive")]
public class AuthorizationTests
{
    [DataTestMethod, AuthorizationServiceCalls]
    [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
    public async Task WhenUserHasNoRightToViewData_HttpForbiddenIsReturned(TestHostCall testHostCallExpression)
    {
        await using var testHost = await TestHost.Create(DefaultPermissions.NoPermissions);
        var testHostCall = () => testHostCallExpression.Compile()(testHost);
        await testHostCall.Should().ThrowForbiddenAsync();
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class AuthorizationServiceCallsAttribute : Attribute, ITestDataSource
    {
        private static readonly TestHostCall[] _testData = {
            testHost => testHost.Get<ActivityController, ActivityDto>(x => x.Get(Guid.Empty, default)),
            testHost => testHost.Get<ActivityController, ActivityDto>(x => x.GetGridFiltered(default, default)),
            testHost => testHost.Get<ActivityController, ActivityDto>(x => x.GetGridItem(Guid.Empty, default)),
            testHost => testHost.Post<ActivityController, ActivityDto>(x => x.Create(default), default),
            testHost => testHost.Put<ActivityController, ActivityDto>(x => x.Update(default), default),
            testHost => testHost.Delete<ActivityController>(x => x.Delete(default)),

            testHost => testHost.Get<CustomerController, CustomerDto>(x => x.Get(Guid.Empty, default)),
            testHost => testHost.Get<CustomerController, CustomerDto>(x => x.GetGridFiltered(default, default)),
            testHost => testHost.Get<CustomerController, CustomerDto>(x => x.GetGridItem(Guid.Empty, default)),
            testHost => testHost.Post<CustomerController, CustomerDto>(x => x.Create(default), default),
            testHost => testHost.Put<CustomerController, CustomerDto>(x => x.Update(default), default),
            testHost => testHost.Delete<CustomerController>(x => x.Delete(default)),

            testHost => testHost.Get<HolidayController, HolidayDto>(x => x.Get(Guid.Empty, default)),
            testHost => testHost.Get<HolidayController, HolidayDto>(x => x.GetGridFiltered(default, default)),
            testHost => testHost.Get<HolidayController, HolidayDto>(x => x.GetGridItem(Guid.Empty, default)),
            testHost => testHost.Post<HolidayController, HolidayDto>(x => x.Create(default), default),
            testHost => testHost.Put<HolidayController, HolidayDto>(x => x.Update(default), default),
            testHost => testHost.Post<HolidayController, IFormFile>(x => x.Import(default, default, default), default),
            testHost => testHost.Delete<HolidayController>(x => x.Delete(default)),

            testHost => testHost.Get<OrderController, OrderDto>(x => x.Get(Guid.Empty, default)),
            testHost => testHost.Get<OrderController, OrderDto>(x => x.GetGridFiltered(default, default)),
            testHost => testHost.Get<OrderController, OrderDto>(x => x.GetGridItem(Guid.Empty, default)),
            testHost => testHost.Post<OrderController, OrderDto>(x => x.Create(default), default),
            testHost => testHost.Put<OrderController, OrderDto>(x => x.Update(default), default),
            testHost => testHost.Delete<OrderController>(x => x.Delete(default)),

            testHost => testHost.Get<ProjectController, ProjectDto>(x => x.Get(Guid.Empty, default)),
            testHost => testHost.Get<ProjectController, ProjectDto>(x => x.GetGridFiltered(default, default)),
            testHost => testHost.Get<ProjectController, ProjectDto>(x => x.GetGridItem(Guid.Empty, default)),
            testHost => testHost.Post<ProjectController, ProjectDto>(x => x.Create(default), default),
            testHost => testHost.Put<ProjectController, ProjectDto>(x => x.Update(default), default),
            testHost => testHost.Delete<ProjectController>(x => x.Delete(default)),

            testHost => testHost.Get<MaintenanceController, JObject>(x => x.ExportData(default)),
            testHost => testHost.Post<MaintenanceController, JObject>(x => x.ImportData(default), default),
            testHost => testHost.Delete<MaintenanceController>(x => x.TruncateData()),
        };

        public IEnumerable<object[]> GetData(MethodInfo methodInfo)
            => _testData.Select(testCall => new object[] { testCall }).Reverse();

        public string GetDisplayName(MethodInfo methodInfo, object[] data)
        {
            var serviceMethodCall = (LambdaExpression)((UnaryExpression)((MethodCallExpression)((LambdaExpression)data[0]).Body).Arguments[0]).Operand;
            var serviceMethodName = ((MethodCallExpression)serviceMethodCall.Body).Method.Name;
            var serviceName = serviceMethodCall.Parameters[0].Type.Name.Replace("Controller", string.Empty);
            return $"{serviceName}.{serviceMethodName}";
        }
    }
}
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
