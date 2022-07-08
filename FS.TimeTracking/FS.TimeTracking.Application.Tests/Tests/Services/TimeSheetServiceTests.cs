using Autofac.Extras.FakeItEasy;
using FakeItEasy;
using FluentAssertions;
using FS.TimeTracking.Application.Services.TimeTracking;
using FS.TimeTracking.Application.Tests.Services;
using FS.TimeTracking.Core.Interfaces.Application.Services.TimeTracking;
using FS.TimeTracking.Core.Interfaces.Repository.Services;
using FS.TimeTracking.Core.Models.Application.TimeTracking;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace FS.TimeTracking.Application.Tests.Tests.Services;

[TestClass, ExcludeFromCodeCoverage]
public class TimeSheetServiceTests
{
    [TestMethod]
    public async Task WhenTimeSheetIsStopped_EndDateIsSet()
    {
        // Prepare
        using var autoFake = new AutoFake();

        var timeSheet = FakeEntityFactory.CreateTimeSheet(default, default);
        timeSheet.EndDate = null;

        var repository = autoFake.Resolve<IRepository>();
        A.CallTo(() => repository.FirstOrDefault((TimeSheet x) => x, default, default, default, default, default, default))
            .WithAnyArguments()
            .Returns(timeSheet);
        A.CallTo(() => repository.Update(default(TimeSheet)))
            .WithAnyArguments()
            .ReturnsLazily((TimeSheet updatedTimeSheet) => updatedTimeSheet);

        autoFake.Provide(FakeEntityFactory.Mapper);
        autoFake.Provide(repository);
        autoFake.Provide<ITimeSheetService, TimeSheetService>();
        var timeSheetService = autoFake.Resolve<ITimeSheetService>();

        // Act
        var stoppedTimeSheet = await timeSheetService.StopTimeSheetEntry(default, DateTimeOffset.Now);

        // Check
        stoppedTimeSheet.EndDate.Should().Be(timeSheet.EndDate);
    }

    [TestMethod]
    public async Task WhenAlreadyStoppedTimeSheetIsStoppedAgain_ExceptionIsThrown()
    {
        // Prepare
        using var autoFake = new AutoFake();

        var repository = autoFake.Resolve<IRepository>();
        A.CallTo(() => repository.FirstOrDefault((TimeSheet x) => x, default, default, default, default, default, default))
            .WithAnyArguments()
            .Returns(FakeEntityFactory.CreateTimeSheet(default, default));

        autoFake.Provide(repository);
        autoFake.Provide<ITimeSheetService, TimeSheetService>();
        var timeSheetService = autoFake.Resolve<ITimeSheetService>();

        // Act
        Func<Task> action = async () => await timeSheetService.StopTimeSheetEntry(default, default);

        // Check
        await action.Should().ThrowAsync<InvalidOperationException>().WithMessage("Time sheet with ID * is already stopped.");
    }
}