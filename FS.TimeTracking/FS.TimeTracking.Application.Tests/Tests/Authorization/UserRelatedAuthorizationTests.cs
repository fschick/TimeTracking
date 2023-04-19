using AutoMapper;
using FluentAssertions;
using FS.TimeTracking.Abstractions.Constants;
using FS.TimeTracking.Abstractions.DTOs.Administration;
using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Abstractions.DTOs.TimeTracking;
using FS.TimeTracking.Abstractions.Enums;
using FS.TimeTracking.Application.Services.MasterData;
using FS.TimeTracking.Application.Services.Shared;
using FS.TimeTracking.Application.Services.TimeTracking;
using FS.TimeTracking.Application.Tests.Extensions;
using FS.TimeTracking.Application.Tests.Services;
using FS.TimeTracking.Application.Tests.Services.FakeModels;
using FS.TimeTracking.Core.Interfaces.Application.Services.Administration;
using FS.TimeTracking.Core.Interfaces.Application.Services.MasterData;
using FS.TimeTracking.Core.Interfaces.Application.Services.TimeTracking;
using FS.TimeTracking.Core.Interfaces.Models;
using FS.TimeTracking.Core.Interfaces.Repository.Services.Database;
using FS.TimeTracking.Core.Models.Application.MasterData;
using FS.TimeTracking.Core.Models.Application.TimeTracking;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace FS.TimeTracking.Application.Tests.Tests.Authorization;

[TestClass, ExcludeFromCodeCoverage]
public class UserRelatedAuthorizationTests
{
    [TestMethod]
    public async Task WhenUserHasNoRightToViewForeignData_OnlyOwnTimeSheetsAreViewable()
    {
        // Prepare
        using var faker = new Faker();
        var (testData, dbRepository, _, timeSheetService, _, _) = await Initialize(faker, DefaultPermissions.NoPermissions);
        await dbRepository.AddRange(new List<IIdEntityModel> { testData.OwnTimeSheet, testData.ForeignTimeSheet, testData.Activity, testData.Customer });
        await dbRepository.SaveChanges();

        // Act
        var timeSheetsGetOwn = await timeSheetService.Get(testData.OwnTimeSheet.Id);
        var timeSheetsGetForeign = async () => await timeSheetService.Get(testData.ForeignTimeSheet.Id);
        var timeSheetsGetGridItems = await timeSheetService.GetGridFiltered(FakeFilters.Empty());
        var timeSheetsGetOwnGridItem = await timeSheetService.GetGridItem(testData.OwnTimeSheet.Id);
        var timeSheetsGetForeignGridItem = async () => await timeSheetService.GetGridItem(testData.ForeignTimeSheet.Id);

        // Check
        timeSheetsGetOwn.Should().BeEquivalentTo(new { testData.OwnTimeSheet.Id, testData.OwnTimeSheet.UserId });
        await timeSheetsGetForeign.Should().ThrowForbiddenForeignUserDataAsync();
        timeSheetsGetGridItems.Should().OnlyContain(x => x.UserId == testData.OwnUser.Id);
        timeSheetsGetOwnGridItem.Should().BeEquivalentTo(new { testData.OwnTimeSheet.Id, testData.OwnTimeSheet.UserId });
        await timeSheetsGetForeignGridItem.Should().ThrowForbiddenForeignUserDataAsync();
    }

    [TestMethod]
    public async Task WhenUserHasRightToViewForeignData_ForeignTimeSheetsAreViewable()
    {
        // Prepare
        using var faker = new Faker();
        var (testData, dbRepository, _, timeSheetService, _, _) = await Initialize(faker, DefaultPermissions.ReadPermissions);
        await dbRepository.AddRange(new List<IIdEntityModel> { testData.OwnTimeSheet, testData.ForeignTimeSheet, testData.Activity, testData.Customer });
        await dbRepository.SaveChanges();

        // Act
        var timeSheetsGetOwn = await timeSheetService.Get(testData.OwnTimeSheet.Id);
        var timeSheetsGetForeign = await timeSheetService.Get(testData.ForeignTimeSheet.Id);
        var timeSheetsGetGridItems = await timeSheetService.GetGridFiltered(FakeFilters.Empty());
        var timeSheetsGetOwnGridItem = await timeSheetService.GetGridItem(testData.OwnTimeSheet.Id);
        var timeSheetsGetForeignGridItem = await timeSheetService.GetGridItem(testData.ForeignTimeSheet.Id);

        // Check
        timeSheetsGetOwn.Should().BeEquivalentTo(new { testData.OwnTimeSheet.Id, testData.OwnTimeSheet.UserId });
        timeSheetsGetForeign.Should().BeEquivalentTo(new { testData.ForeignTimeSheet.Id, testData.ForeignTimeSheet.UserId });
        timeSheetsGetGridItems.Should().HaveCount(2);
        timeSheetsGetOwnGridItem.Should().BeEquivalentTo(new { testData.OwnTimeSheet.Id, testData.OwnTimeSheet.UserId });
        timeSheetsGetForeignGridItem.Should().BeEquivalentTo(new { testData.ForeignTimeSheet.Id, testData.ForeignTimeSheet.UserId });
    }

    [TestMethod]
    public async Task WhenUserHasNoRightToManageForeignData_OnlyOwnTimeSheetsAreManageable()
    {
        // Prepare
        using var faker = new Faker();

        var (testData, dbRepository, mapper, timeSheetService, _, _) = await Initialize(faker, DefaultPermissions.ReadPermissions);

        var ownTimeSheetToUpdate = faker.TimeSheet.Create(testData.Customer.Id, testData.Activity.Id, userId: testData.OwnUser.Id);
        var ownTimeSheetToUpdateToForeign = mapper.Map<TimeSheetDto>(ownTimeSheetToUpdate) with { UserId = testData.ForeignUser.Id };

        var foreignTimeSheetToUpdate = faker.TimeSheet.Create(testData.Customer.Id, testData.Activity.Id, userId: testData.ForeignUser.Id);
        var foreignTimeSheetToUpdateOwn = mapper.Map<TimeSheetDto>(foreignTimeSheetToUpdate) with { UserId = testData.OwnUser.Id };

        var ownTimeSheetToDelete = faker.TimeSheet.Create(testData.Customer.Id, testData.Activity.Id, userId: testData.OwnUser.Id);
        var foreignTimeSheetToDelete = faker.TimeSheet.Create(testData.Customer.Id, testData.Activity.Id, userId: testData.ForeignUser.Id);

        await dbRepository.AddRange(new List<IIdEntityModel> { ownTimeSheetToUpdate, foreignTimeSheetToUpdate, ownTimeSheetToDelete, foreignTimeSheetToDelete, testData.Activity, testData.Customer });
        await dbRepository.SaveChanges();

        // Act
        var createOwnTimeSheet = await timeSheetService.Create(mapper.Map<TimeSheetDto>(testData.OwnTimeSheet));
        var createForeignTimeSheet = async () => await timeSheetService.Create(mapper.Map<TimeSheetDto>(testData.ForeignTimeSheet));

        var updateOwnTimeSheet = await timeSheetService.Update(mapper.Map<TimeSheetDto>(ownTimeSheetToUpdate));
        var updateOwnTimeSheetToForeign = async () => await timeSheetService.Update(ownTimeSheetToUpdateToForeign);
        var updateForeignTimeSheet = async () => await timeSheetService.Update(mapper.Map<TimeSheetDto>(foreignTimeSheetToUpdate));
        var updateForeignTimeSheetToOwn = async () => await timeSheetService.Update(foreignTimeSheetToUpdateOwn);

        var deleteOwnTimeSheet = await timeSheetService.Delete(ownTimeSheetToDelete.Id);
        var deleteForeignTimeSheet = async () => await timeSheetService.Delete(foreignTimeSheetToDelete.Id);

        // Check
        createOwnTimeSheet.Should().BeEquivalentTo(new { testData.OwnTimeSheet.Id });
        await createForeignTimeSheet.Should().ThrowForbiddenForeignUserDataAsync();

        updateOwnTimeSheet.Should().BeEquivalentTo(new { ownTimeSheetToUpdate.Id, ownTimeSheetToUpdate.UserId });
        await updateOwnTimeSheetToForeign.Should().ThrowForbiddenForeignUserDataAsync();
        await updateForeignTimeSheet.Should().ThrowForbiddenForeignUserDataAsync();
        await updateForeignTimeSheetToOwn.Should().ThrowForbiddenForeignUserDataAsync();

        deleteOwnTimeSheet.Should().Be(1);
        await deleteForeignTimeSheet.Should().ThrowForbiddenForeignUserDataAsync();
    }

    [TestMethod]
    public async Task WhenUserHasRightToManageForeignData_ForeignTimeSheetsAreManageable()
    {
        // Prepare
        using var faker = new Faker();

        var (testData, dbRepository, mapper, timeSheetService, _, _) = await Initialize(faker, DefaultPermissions.WritePermissions);

        var ownTimeSheetToUpdate = faker.TimeSheet.Create(testData.Customer.Id, testData.Activity.Id, userId: testData.OwnUser.Id);
        var ownTimeSheetToUpdateToForeign = mapper.Map<TimeSheetDto>(ownTimeSheetToUpdate) with { UserId = testData.ForeignUser.Id };

        var foreignTimeSheetToUpdate = faker.TimeSheet.Create(testData.Customer.Id, testData.Activity.Id, userId: testData.ForeignUser.Id);
        var foreignTimeSheetToUpdateOwn = mapper.Map<TimeSheetDto>(foreignTimeSheetToUpdate) with { UserId = testData.OwnUser.Id };

        var ownTimeSheetToDelete = faker.TimeSheet.Create(testData.Customer.Id, testData.Activity.Id, userId: testData.OwnUser.Id);
        var foreignTimeSheetToDelete = faker.TimeSheet.Create(testData.Customer.Id, testData.Activity.Id, userId: testData.ForeignUser.Id);

        await dbRepository.AddRange(new List<IIdEntityModel> { ownTimeSheetToUpdate, foreignTimeSheetToUpdate, ownTimeSheetToDelete, foreignTimeSheetToDelete, testData.Activity, testData.Customer });
        await dbRepository.SaveChanges();

        // Act
        var createOwnTimeSheet = await timeSheetService.Create(mapper.Map<TimeSheetDto>(testData.OwnTimeSheet));
        var createForeignTimeSheet = await timeSheetService.Create(mapper.Map<TimeSheetDto>(testData.ForeignTimeSheet));

        var updateOwnTimeSheet = await timeSheetService.Update(mapper.Map<TimeSheetDto>(ownTimeSheetToUpdate));
        var updateOwnTimeSheetToForeign = await timeSheetService.Update(ownTimeSheetToUpdateToForeign);
        var updateForeignTimeSheet = await timeSheetService.Update(mapper.Map<TimeSheetDto>(foreignTimeSheetToUpdate));
        var updateForeignTimeSheetToOwn = await timeSheetService.Update(foreignTimeSheetToUpdateOwn);

        var deleteOwnTimeSheet = await timeSheetService.Delete(ownTimeSheetToDelete.Id);
        var deleteForeignTimeSheet = await timeSheetService.Delete(foreignTimeSheetToDelete.Id);

        // Check
        createOwnTimeSheet.Should().BeEquivalentTo(new { testData.OwnTimeSheet.Id, testData.OwnTimeSheet.UserId });
        createForeignTimeSheet.Should().BeEquivalentTo(new { testData.ForeignTimeSheet.Id, testData.ForeignTimeSheet.UserId });

        updateOwnTimeSheet.Should().BeEquivalentTo(new { ownTimeSheetToUpdate.Id, ownTimeSheetToUpdate.UserId });
        updateOwnTimeSheetToForeign.Should().BeEquivalentTo(new { ownTimeSheetToUpdateToForeign.Id, ownTimeSheetToUpdateToForeign.UserId });
        updateForeignTimeSheet.Should().BeEquivalentTo(new { foreignTimeSheetToUpdate.Id, foreignTimeSheetToUpdate.UserId });
        updateForeignTimeSheetToOwn.Should().BeEquivalentTo(new { foreignTimeSheetToUpdateOwn.Id, foreignTimeSheetToUpdateOwn.UserId });

        deleteOwnTimeSheet.Should().Be(1);
        deleteForeignTimeSheet.Should().Be(1);
    }

    [TestMethod]
    public async Task WhenUserHasNoRightToViewForeignData_OnlyOwnVacationsAreViewable()
    {
        // Prepare
        using var faker = new Faker();
        var (testData, dbRepository, _, _, vacationService, _) = await Initialize(faker, DefaultPermissions.NoPermissions);
        await dbRepository.AddRange(new List<IIdEntityModel> { testData.OwnVacation, testData.ForeignVacation, testData.PublicHoliday });
        await dbRepository.SaveChanges();

        // Act
        var vacationsGetOwn = await vacationService.Get(testData.OwnVacation.Id);
        var vacationsGetPublic = await vacationService.Get(testData.PublicHoliday.Id);
        var vacationsGetForeign = async () => await vacationService.Get(testData.ForeignVacation.Id);
        var vacationsGetGridItems = await vacationService.GetGridFiltered(FakeFilters.Empty());
        var vacationsGetOwnGridItem = await vacationService.GetGridItem(testData.OwnVacation.Id);
        var vacationsGetPublicGridItem = await vacationService.GetGridItem(testData.PublicHoliday.Id);
        var vacationsGetForeignGridItem = async () => await vacationService.GetGridItem(testData.ForeignVacation.Id);

        // Check
        vacationsGetOwn.Should().BeEquivalentTo(new { testData.OwnVacation.Id, testData.OwnVacation.UserId });
        vacationsGetPublic.Should().BeEquivalentTo(new { testData.PublicHoliday.Id, testData.PublicHoliday.UserId });
        await vacationsGetForeign.Should().ThrowForbiddenForeignUserDataAsync();
        vacationsGetGridItems.Should()
            .Contain(x => x.UserId == testData.OwnUser.Id).And
            .Contain(x => x.UserId == AuthorizationService.DefaultUserId).And
            .NotContain(x => x.UserId == testData.ForeignUser.Id);
        vacationsGetOwnGridItem.Should().BeEquivalentTo(new { testData.OwnVacation.Id, testData.OwnVacation.UserId });
        vacationsGetPublicGridItem.Should().BeEquivalentTo(new { testData.PublicHoliday.Id, testData.PublicHoliday.UserId });
        await vacationsGetForeignGridItem.Should().ThrowForbiddenForeignUserDataAsync();
    }

    [TestMethod]
    public async Task WhenUserHasRightToViewForeignData_ForeignVacationsAreViewable()
    {
        // Prepare
        using var faker = new Faker();
        var (testData, dbRepository, _, _, vacationService, _) = await Initialize(faker, DefaultPermissions.ReadPermissions);
        await dbRepository.AddRange(new List<IIdEntityModel> { testData.OwnVacation, testData.ForeignVacation, testData.PublicHoliday });
        await dbRepository.SaveChanges();

        // Act
        var vacationsGetOwn = await vacationService.Get(testData.OwnVacation.Id);
        var vacationsGetPublic = await vacationService.Get(testData.PublicHoliday.Id);
        var vacationsGetForeign = await vacationService.Get(testData.ForeignVacation.Id);
        var vacationsGetGridItems = await vacationService.GetGridFiltered(FakeFilters.Empty());
        var vacationsGetOwnGridItem = await vacationService.GetGridItem(testData.OwnVacation.Id);
        var vacationsGetPublicGridItem = await vacationService.GetGridItem(testData.PublicHoliday.Id);
        var vacationsGetForeignGridItem = await vacationService.GetGridItem(testData.ForeignVacation.Id);

        // Check
        vacationsGetOwn.Should().BeEquivalentTo(new { testData.OwnVacation.Id, testData.OwnVacation.UserId });
        vacationsGetPublic.Should().BeEquivalentTo(new { testData.PublicHoliday.Id, UserId = AuthorizationService.DefaultUserId });
        vacationsGetForeign.Should().BeEquivalentTo(new { testData.ForeignVacation.Id, testData.ForeignVacation.UserId });
        vacationsGetGridItems.Should().HaveCount(3);
        vacationsGetOwnGridItem.Should().BeEquivalentTo(new { testData.OwnVacation.Id, testData.OwnVacation.UserId });
        vacationsGetPublicGridItem.Should().BeEquivalentTo(new { testData.PublicHoliday.Id, testData.PublicHoliday.UserId });
        vacationsGetForeignGridItem.Should().BeEquivalentTo(new { testData.ForeignVacation.Id, testData.ForeignVacation.UserId });
    }

    [TestMethod]
    public async Task WhenUserHasNoRightToManageForeignData_OnlyOwnVacationsAreManageable()
    {
        // Prepare
        using var faker = new Faker();

        var (testData, dbRepository, mapper, _, vacationService, _) = await Initialize(faker, DefaultPermissions.ReadPermissions);

        var ownVacationToUpdate = faker.Holiday.Create("2020-01-01", "2020-01-05", HolidayType.Holiday, userId: testData.OwnUser.Id);
        var ownVacationToUpdateToPublic = mapper.Map<HolidayDto>(ownVacationToUpdate) with { Type = HolidayType.PublicHoliday, UserId = AuthorizationService.DefaultUserId };
        var ownVacationToUpdateToForeign = mapper.Map<HolidayDto>(ownVacationToUpdate) with { UserId = testData.ForeignUser.Id };

        var publicHolidayToUpdate = faker.Holiday.Create("2020-01-01", "2020-01-05", HolidayType.PublicHoliday, userId: AuthorizationService.DefaultUserId);
        var publicHolidayToUpdateToOwn = mapper.Map<HolidayDto>(publicHolidayToUpdate) with { Type = HolidayType.Holiday, UserId = testData.OwnUser.Id };
        var publicHolidayToUpdateToForeign = mapper.Map<HolidayDto>(publicHolidayToUpdate) with { Type = HolidayType.Holiday, UserId = testData.ForeignUser.Id };

        var foreignVacationToUpdate = faker.Holiday.Create("2020-01-01", "2020-01-05", HolidayType.Holiday, userId: testData.ForeignUser.Id);
        var foreignVacationToUpdateOwn = mapper.Map<HolidayDto>(foreignVacationToUpdate) with { UserId = testData.OwnUser.Id };
        var foreignVacationToUpdatePublic = mapper.Map<HolidayDto>(foreignVacationToUpdate) with { Type = HolidayType.PublicHoliday, UserId = AuthorizationService.DefaultUserId };

        var ownVacationToDelete = faker.Holiday.Create("2020-01-01", "2020-01-05", HolidayType.Holiday, userId: testData.OwnUser.Id);
        var publicVacationToDelete = faker.Holiday.Create("2020-01-01", "2020-01-05", HolidayType.PublicHoliday, userId: AuthorizationService.DefaultUserId);
        var foreignVacationToDelete = faker.Holiday.Create("2020-01-01", "2020-01-05", HolidayType.Holiday, userId: testData.ForeignUser.Id);

        await dbRepository.AddRange(new List<IIdEntityModel> { ownVacationToUpdate, publicHolidayToUpdate, foreignVacationToUpdate, ownVacationToDelete, publicVacationToDelete, foreignVacationToDelete });
        await dbRepository.SaveChanges();

        // Act
        var createOwnVacation = await vacationService.Create(mapper.Map<HolidayDto>(testData.OwnVacation));
        var createPublicVacation = await vacationService.Create(mapper.Map<HolidayDto>(testData.PublicHoliday));
        var createForeignVacation = async () => await vacationService.Create(mapper.Map<HolidayDto>(testData.ForeignVacation));

        var updateOwnVacation = await vacationService.Update(mapper.Map<HolidayDto>(ownVacationToUpdate));
        var updateOwnVacationToPublic = await vacationService.Update(ownVacationToUpdateToPublic);
        var updateOwnVacationToForeign = async () => await vacationService.Update(ownVacationToUpdateToForeign);

        var updatePublicHoliday = await vacationService.Update(mapper.Map<HolidayDto>(publicHolidayToUpdate));
        var updatePublicHolidayToOwn = await vacationService.Update(publicHolidayToUpdateToOwn);
        var updatePublicHolidayToForeign = async () => await vacationService.Update(publicHolidayToUpdateToForeign);

        var updateForeignVacation = async () => await vacationService.Update(mapper.Map<HolidayDto>(foreignVacationToUpdate));
        var updateForeignVacationToOwn = async () => await vacationService.Update(foreignVacationToUpdateOwn);
        var updateForeignVacationToPublic = async () => await vacationService.Update(foreignVacationToUpdatePublic);

        var deleteOwnVacation = await vacationService.Delete(ownVacationToDelete.Id);
        var deletePublicHoliday = await vacationService.Delete(publicVacationToDelete.Id);
        var deleteForeignVacation = async () => await vacationService.Delete(foreignVacationToDelete.Id);

        // Check
        createOwnVacation.Should().BeEquivalentTo(new { testData.OwnVacation.Id, testData.OwnVacation.UserId });
        createPublicVacation.Should().BeEquivalentTo(new { testData.PublicHoliday.Id, testData.PublicHoliday.UserId });
        await createForeignVacation.Should().ThrowForbiddenForeignUserDataAsync();

        updateOwnVacation.Should().BeEquivalentTo(new { ownVacationToUpdate.Id, ownVacationToUpdate.UserId });
        updateOwnVacationToPublic.Should().BeEquivalentTo(new { ownVacationToUpdateToPublic.Id, ownVacationToUpdateToPublic.UserId });
        await updateOwnVacationToForeign.Should().ThrowForbiddenForeignUserDataAsync();

        updatePublicHoliday.Should().BeEquivalentTo(new { publicHolidayToUpdate.Id, publicHolidayToUpdate.UserId });
        updatePublicHolidayToOwn.Should().BeEquivalentTo(new { publicHolidayToUpdateToOwn.Id, publicHolidayToUpdateToOwn.UserId });
        await updatePublicHolidayToForeign.Should().ThrowForbiddenForeignUserDataAsync();

        updatePublicHoliday.Should().BeEquivalentTo(new { publicHolidayToUpdate.Id, publicHolidayToUpdate.UserId });
        updatePublicHolidayToOwn.Should().BeEquivalentTo(new { publicHolidayToUpdateToOwn.Id, publicHolidayToUpdateToOwn.UserId });
        await updatePublicHolidayToForeign.Should().ThrowForbiddenForeignUserDataAsync();

        await updateForeignVacation.Should().ThrowForbiddenForeignUserDataAsync();
        await updateForeignVacationToOwn.Should().ThrowForbiddenForeignUserDataAsync();
        await updateForeignVacationToPublic.Should().ThrowForbiddenForeignUserDataAsync();

        deleteOwnVacation.Should().Be(1);
        deletePublicHoliday.Should().Be(1);
        await deleteForeignVacation.Should().ThrowForbiddenForeignUserDataAsync();
    }

    [TestMethod]
    public async Task WhenUserHasRightToManageForeignData_ForeignVacationsAreManageable()
    {
        // Prepare
        using var faker = new Faker();

        var (testData, dbRepository, mapper, _, vacationService, _) = await Initialize(faker, DefaultPermissions.WritePermissions);

        var ownVacationToUpdate = faker.Holiday.Create("2020-01-01", "2020-01-05", HolidayType.Holiday, userId: testData.OwnUser.Id);
        var ownVacationToUpdateToPublic = mapper.Map<HolidayDto>(ownVacationToUpdate) with { Type = HolidayType.PublicHoliday, UserId = AuthorizationService.DefaultUserId };
        var ownVacationToUpdateToForeign = mapper.Map<HolidayDto>(ownVacationToUpdate) with { UserId = testData.ForeignUser.Id };

        var publicHolidayToUpdate = faker.Holiday.Create("2020-01-01", "2020-01-05", HolidayType.PublicHoliday, userId: AuthorizationService.DefaultUserId);
        var publicHolidayToUpdateToOwn = mapper.Map<HolidayDto>(publicHolidayToUpdate) with { Type = HolidayType.Holiday, UserId = testData.OwnUser.Id };
        var publicHolidayToUpdateToForeign = mapper.Map<HolidayDto>(publicHolidayToUpdate) with { Type = HolidayType.Holiday, UserId = testData.ForeignUser.Id };

        var foreignVacationToUpdate = faker.Holiday.Create("2020-01-01", "2020-01-05", HolidayType.Holiday, userId: testData.ForeignUser.Id);
        var foreignVacationToUpdateOwn = mapper.Map<HolidayDto>(foreignVacationToUpdate) with { UserId = testData.OwnUser.Id };
        var foreignVacationToUpdatePublic = mapper.Map<HolidayDto>(foreignVacationToUpdate) with { Type = HolidayType.PublicHoliday, UserId = AuthorizationService.DefaultUserId };

        var ownVacationToDelete = faker.Holiday.Create("2020-01-01", "2020-01-05", HolidayType.Holiday, userId: testData.OwnUser.Id);
        var publicVacationToDelete = faker.Holiday.Create("2020-01-01", "2020-01-05", HolidayType.PublicHoliday, userId: AuthorizationService.DefaultUserId);
        var foreignVacationToDelete = faker.Holiday.Create("2020-01-01", "2020-01-05", HolidayType.Holiday, userId: testData.ForeignUser.Id);

        await dbRepository.AddRange(new List<IIdEntityModel> { ownVacationToUpdate, publicHolidayToUpdate, foreignVacationToUpdate, ownVacationToDelete, publicVacationToDelete, foreignVacationToDelete });
        await dbRepository.SaveChanges();

        // Act
        var createOwnVacation = await vacationService.Create(mapper.Map<HolidayDto>(testData.OwnVacation));
        var createPublicVacation = await vacationService.Create(mapper.Map<HolidayDto>(testData.PublicHoliday));
        var createForeignVacation = await vacationService.Create(mapper.Map<HolidayDto>(testData.ForeignVacation));

        var updateOwnVacation = await vacationService.Update(mapper.Map<HolidayDto>(ownVacationToUpdate));
        var updateOwnVacationToPublic = await vacationService.Update(ownVacationToUpdateToPublic);
        var updateOwnVacationToForeign = await vacationService.Update(ownVacationToUpdateToForeign);

        var updatePublicHoliday = await vacationService.Update(mapper.Map<HolidayDto>(publicHolidayToUpdate));
        var updatePublicHolidayToOwn = await vacationService.Update(publicHolidayToUpdateToOwn);
        var updatePublicHolidayToForeign = await vacationService.Update(publicHolidayToUpdateToForeign);

        var updateForeignVacation = await vacationService.Update(mapper.Map<HolidayDto>(foreignVacationToUpdate));
        var updateForeignVacationToOwn = await vacationService.Update(foreignVacationToUpdateOwn);
        var updateForeignVacationToPublic = await vacationService.Update(foreignVacationToUpdatePublic);

        var deleteOwnVacation = await vacationService.Delete(ownVacationToDelete.Id);
        var deletePublicHoliday = await vacationService.Delete(publicVacationToDelete.Id);
        var deleteForeignVacation = await vacationService.Delete(foreignVacationToDelete.Id);

        // Check
        createOwnVacation.Should().BeEquivalentTo(new { testData.OwnVacation.Id, testData.OwnVacation.UserId });
        createPublicVacation.Should().BeEquivalentTo(new { testData.PublicHoliday.Id, testData.PublicHoliday.UserId });
        createForeignVacation.Should().BeEquivalentTo(new { testData.ForeignVacation.Id, testData.ForeignVacation.UserId });

        updateOwnVacation.Should().BeEquivalentTo(new { ownVacationToUpdate.Id, ownVacationToUpdate.UserId });
        updateOwnVacationToPublic.Should().BeEquivalentTo(new { ownVacationToUpdateToPublic.Id, ownVacationToUpdateToPublic.UserId });
        updateOwnVacationToForeign.Should().BeEquivalentTo(new { ownVacationToUpdateToForeign.Id, ownVacationToUpdateToForeign.UserId });

        updatePublicHoliday.Should().BeEquivalentTo(new { publicHolidayToUpdate.Id, publicHolidayToUpdate.UserId });
        updatePublicHolidayToOwn.Should().BeEquivalentTo(new { publicHolidayToUpdateToOwn.Id, publicHolidayToUpdateToOwn.UserId });
        updatePublicHolidayToForeign.Should().BeEquivalentTo(new { publicHolidayToUpdateToForeign.Id, publicHolidayToUpdateToForeign.UserId });

        updateForeignVacation.Should().BeEquivalentTo(new { foreignVacationToUpdate.Id, foreignVacationToUpdate.UserId });
        updateForeignVacationToOwn.Should().BeEquivalentTo(new { foreignVacationToUpdateOwn.Id, foreignVacationToUpdateOwn.UserId });
        updateForeignVacationToPublic.Should().BeEquivalentTo(new { foreignVacationToUpdatePublic.Id, foreignVacationToUpdatePublic.UserId });

        deleteOwnVacation.Should().Be(1);
        deletePublicHoliday.Should().Be(1);
        deleteForeignVacation.Should().Be(1);
    }

    [TestMethod]
    public async Task WhenUserHasNoRightToViewForeignData_OnlyOwnUserIsViewable()
    {
        // Prepare
        using var faker = new Faker();
        var (testData, _, _, _, _, userService) = await Initialize(faker, DefaultPermissions.NoPermissions);

        // Act
        var usersGetOwn = await userService.Get(testData.OwnUser.Id);
        var usersGetForeign = async () => await userService.Get(testData.ForeignUser.Id);
        var usersGetGridItems = await userService.GetGridFiltered(FakeFilters.Empty());
        var usersGetGridFiltered = await userService.GetFiltered(FakeFilters.Empty());
        var usersGetOwnGridItem = await userService.GetGridItem(testData.OwnUser.Id);
        var usersGetOwnForeignItem = async () => await userService.GetGridItem(testData.ForeignUser.Id);

        // Check
        usersGetOwn.Should().BeEquivalentTo(new { testData.OwnUser.Id });
        await usersGetForeign.Should().ThrowForbiddenForeignUserDataAsync();
        usersGetGridItems.Should().OnlyContain(x => x.Id == testData.OwnUser.Id);
        usersGetGridFiltered.Should().OnlyContain(x => x.Id == testData.OwnUser.Id);
        usersGetOwnGridItem.Should().BeEquivalentTo(new { testData.OwnUser.Id });
        await usersGetOwnForeignItem.Should().ThrowForbiddenForeignUserDataAsync();
    }

    [TestMethod]
    public async Task WhenUserHasRightToViewForeignData_ForeignUserIsViewable()
    {
        // Prepare
        using var faker = new Faker();
        var (testData, _, _, _, _, userService) = await Initialize(faker, DefaultPermissions.ReadPermissions);

        // Act
        var usersGetOwn = await userService.Get(testData.OwnUser.Id);
        var usersGetForeign = await userService.Get(testData.ForeignUser.Id);
        var usersGetGridItems = await userService.GetGridFiltered(FakeFilters.Empty());
        var usersGetGridFiltered = await userService.GetFiltered(FakeFilters.Empty());
        var usersGetOwnGridItem = await userService.GetGridItem(testData.OwnUser.Id);
        var usersGetOwnForeignItem = await userService.GetGridItem(testData.ForeignUser.Id);

        // Check
        usersGetOwn.Should().BeEquivalentTo(new { testData.OwnUser.Id });
        usersGetForeign.Should().BeEquivalentTo(new { testData.ForeignUser.Id });
        usersGetGridItems.Should().HaveCount(2);
        usersGetGridFiltered.Should().HaveCount(2);
        usersGetOwnGridItem.Should().BeEquivalentTo(new { testData.OwnUser.Id });
        usersGetOwnForeignItem.Should().BeEquivalentTo(new { testData.ForeignUser.Id });
    }

    [TestMethod]
    public async Task WhenUserHasNoRightToManageForeignData_OnlyOwnUserIsManageable()
    {
        // Prepare
        using var faker = new Faker();
        var permissions = DefaultPermissions.ReadPermissions.ChangePermission(PermissionName.ADMINISTRATION_USERS, PermissionScope.MANAGE);
        var (testData, _, _, _, _, userService) = await Initialize(faker, permissions);

        // Act
        var updateOwnUser = await userService.Update(testData.OwnUser);
        var updateForeignUser = async () => await userService.Update(testData.ForeignUser);

        var deleteForeignUser = async () => await userService.Delete(testData.ForeignUser.Id);

        // Check
        updateOwnUser.Should().BeEquivalentTo(new { testData.OwnUser.Id });
        await updateForeignUser.Should().ThrowForbiddenForeignUserDataAsync();

        await deleteForeignUser.Should().ThrowForbiddenForeignUserDataAsync();
    }

    [TestMethod]
    public async Task WhenUserHasRightToManageForeignData_ForeignUserIsManageable()
    {
        // Prepare
        using var faker = new Faker();
        var (testData, _, _, _, _, userService) = await Initialize(faker, DefaultPermissions.WritePermissions);

        // Act
        var updateOwnUser = await userService.Update(testData.OwnUser);
        var updateForeignUser = await userService.Update(testData.ForeignUser);

        var deleteForeignUser = await userService.Delete(testData.ForeignUser.Id);

        // Check
        updateOwnUser.Should().BeEquivalentTo(new { testData.OwnUser.Id });
        updateForeignUser.Should().BeEquivalentTo(new { testData.ForeignUser.Id });

        deleteForeignUser.Should().Be(1);
    }

    private static async Task<TestServices> Initialize(Faker faker, IEnumerable<PermissionDto> permissions)
    {
        var testData = new TestData(faker, permissions);
        faker.ConfigureInMemoryDatabase();
        faker.ConfigureAuthorization(true, testData.OwnUser);
        faker.Provide<IHolidayApiService, HolidayService>();
        faker.Provide<ITimeSheetApiService, TimeSheetService>();

        await faker.ProvideUsers(testData.OwnUser, testData.ForeignUser);

        var timeSheetService = faker.GetRequiredService<ITimeSheetApiService>();
        var vacationService = faker.GetRequiredService<IHolidayApiService>();
        var userService = faker.GetRequiredService<IUserService>();
        var dbRepository = faker.GetRequiredService<IDbRepository>();

        return new TestServices
        {
            TestData = testData,
            DbRepository = dbRepository,
            Mapper = faker.AutoMapper,
            TimeSheetService = timeSheetService,
            VacationService = vacationService,
            UserService = userService,
        };
    }

    private class TestServices
    {
        public TestData TestData;
        public IDbRepository DbRepository;
        public IMapper Mapper;
        public ITimeSheetApiService TimeSheetService;
        public IHolidayApiService VacationService;
        public IUserService UserService;

        public void Deconstruct(
            out TestData testData,
            out IDbRepository dbRepository,
            out IMapper mapper,
            out ITimeSheetApiService timeSheetService,
            out IHolidayApiService vacationService,
            out IUserService userService
            )
        {
            testData = TestData;
            dbRepository = DbRepository;
            mapper = Mapper;
            timeSheetService = TimeSheetService;
            vacationService = VacationService;
            userService = UserService;
        }
    }

    private class TestData
    {
        public Activity Activity { get; }
        public Customer Customer { get; }
        public UserDto OwnUser { get; }
        public UserDto ForeignUser { get; }

        public TimeSheet OwnTimeSheet { get; }
        public TimeSheet ForeignTimeSheet { get; }
        public Holiday OwnVacation { get; }
        public Holiday ForeignVacation { get; }
        public Holiday PublicHoliday { get; }

        public TestData(Faker faker, IEnumerable<PermissionDto> permissions)
        {
            Activity = faker.Activity.Create();
            Customer = faker.Customer.Create();
            OwnUser = faker.User.Create("Own", permissions: permissions);
            ForeignUser = faker.User.Create("Foreign");

            OwnTimeSheet = faker.TimeSheet.Create(Customer.Id, Activity.Id, userId: OwnUser.Id);
            ForeignTimeSheet = faker.TimeSheet.Create(Customer.Id, Activity.Id, userId: ForeignUser.Id);
            OwnVacation = faker.Holiday.Create("2020-01-01", "2020-01-05", HolidayType.Holiday, OwnUser.Id, "Own");
            ForeignVacation = faker.Holiday.Create("2020-01-01", "2020-01-05", HolidayType.Holiday, ForeignUser.Id, "Foreign");
            PublicHoliday = faker.Holiday.Create("2020-01-01", "2020-01-05", HolidayType.PublicHoliday, AuthorizationService.DefaultUserId, "Public");
        }
    }
}