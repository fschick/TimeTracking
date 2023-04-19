using AutoMapper;
using FluentAssertions;
using FS.TimeTracking.Abstractions.Constants;
using FS.TimeTracking.Abstractions.DTOs.Administration;
using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Abstractions.DTOs.TimeTracking;
using FS.TimeTracking.Abstractions.Interfaces.DTOs;
using FS.TimeTracking.Application.Services.MasterData;
using FS.TimeTracking.Application.Services.Shared;
using FS.TimeTracking.Application.Services.TimeTracking;
using FS.TimeTracking.Application.Tests.Extensions;
using FS.TimeTracking.Core.Exceptions;
using FS.TimeTracking.Core.Interfaces.Application.Services.MasterData;
using FS.TimeTracking.Core.Interfaces.Application.Services.Shared;
using FS.TimeTracking.Core.Interfaces.Application.Services.TimeTracking;
using FS.TimeTracking.Core.Interfaces.Models;
using FS.TimeTracking.Core.Interfaces.Repository.Services.Database;
using FS.TimeTracking.Core.Models.Application.MasterData;
using FS.TimeTracking.Core.Models.Application.TimeTracking;
using FS.TimeTracking.Core.Models.Filter;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Faker = FS.TimeTracking.Application.Tests.Services.Faker;

namespace FS.TimeTracking.Application.Tests.Tests.Authorization;

[TestClass, ExcludeFromCodeCoverage]
public class CustomerRelatedAuthorizationTests
{
    [TestMethod]
    public async Task WhenUserIsRestrictedToCustomer_EntitiesBelongToOtherCustomersCannotBeReadOrModified()
    {
        using var faker = new Faker();
        var testData = await Initialize(faker);

        await TestRestrictedCustomerAuthentication<ITimeSheetApiService, TimeSheet, TimeSheetDto, TimeSheetGridDto>(faker, testData);
        await TestRestrictedCustomerAuthentication<IOrderApiService, Order, OrderDto, OrderGridDto>(faker, testData);
        await TestRestrictedCustomerAuthentication<IProjectApiService, Project, ProjectDto, ProjectGridDto>(faker, testData);
        await TestRestrictedCustomerAuthentication<IActivityApiService, Activity, ActivityDto, ActivityGridDto>(faker, testData);
        await TestRestrictedCustomerAuthentication<ICustomerApiService, Customer, CustomerDto, CustomerGridDto>(faker, testData);
    }

    [TestMethod]
    public async Task WhenUserIsRestrictedToCustomer_TypeaheadReturnOnlyRelatedEntities()
    {
        using var faker = new Faker();
        var testData = await Initialize(faker);

        var typeaheadService = faker.GetRequiredService<ITypeaheadApiService>();

        var activities = await typeaheadService.GetActivities(true);
        activities.Should().Contain(activity => activity.Id == testData.GetAllowed<Activity>().Id);
        activities.Should().NotContain(activity => activity.Id == testData.GetForbidden<Activity>().Id);

        var customers = await typeaheadService.GetCustomers(true);
        customers.Should().Contain(activity => activity.Id == testData.GetAllowed<Customer>().Id);
        customers.Should().NotContain(activity => activity.Id == testData.GetForbidden<Customer>().Id);

        var orders = await typeaheadService.GetOrders(true);
        orders.Should().Contain(activity => activity.Id == testData.GetAllowed<Order>().Id);
        orders.Should().NotContain(activity => activity.Id == testData.GetForbidden<Order>().Id);

        var projects = await typeaheadService.GetProjects(true);
        projects.Should().Contain(activity => activity.Id == testData.GetAllowed<Project>().Id);
        projects.Should().NotContain(activity => activity.Id == testData.GetForbidden<Project>().Id);
    }

    private static async Task TestRestrictedCustomerAuthentication<TService, TModel, TDto, TGridDto>(IServiceProvider faker, TestData testData)
        where TService : ICrudModelService<Guid, TDto, TGridDto>
        where TModel : class, IIdEntityModel, new()
        where TDto : class, IManageableDto, IIdEntityDto
        where TGridDto : class, IManageableDto, IIdEntityDto
    {
        var mapper = faker.GetRequiredService<IMapper>();
        var service = faker.GetRequiredService<TService>();
        var timeSheetFilterSet = new TimeSheetFilterSet();

        var modelAllowed = testData.GetAllowed<TModel>();
        var modelForbidden = testData.GetForbidden<TModel>();

        var dtoAllowed = mapper.Map<TDto>(modelAllowed);
        var dtoForbidden = mapper.Map<TDto>(modelForbidden);

        var dtoCreateAllowed = dtoAllowed.JsonClone();
        dtoCreateAllowed.Id = Guid.NewGuid();

        var dtoCreateForbidden = dtoForbidden.JsonClone();
        dtoCreateForbidden.Id = Guid.NewGuid();

        // ICrudModelService.Get, allowed
        var getAllowed = await service.Get(modelAllowed.Id);
        getAllowed.Should().BeEquivalentTo(new { modelAllowed.Id });

        // ICrudModelService.Get, forbidden
        var getForbidden = () => service.Get(modelForbidden.Id);
        await getForbidden.Should().ThrowAsync<ForbiddenException>();

        // ICrudModelService.GetGridFiltered, allowed + forbidden
        var getGridFiltered = await service.GetGridFiltered(timeSheetFilterSet);
        getGridFiltered.Should().Contain(result => result.Id == modelAllowed.Id);
        getGridFiltered.Should().NotContain(result => result.Id == modelForbidden.Id);

        // ICrudModelService.GetGridItem, allowed
        var getGridItemAllowed = await service.GetGridItem(modelAllowed.Id);
        getGridItemAllowed.Should().BeEquivalentTo(new { modelAllowed.Id });

        // ICrudModelService.GetGridItem, forbidden
        var getGridItemForbidden = () => service.GetGridItem(modelForbidden.Id);
        await getGridItemForbidden.Should().ThrowAsync<ForbiddenException>();

        // ICrudModelService.Create, allowed
        if (modelAllowed is not Customer)
        {
            var createAllowed = await service.Create(dtoCreateAllowed);
            createAllowed.Should().BeEquivalentTo(new { dtoCreateAllowed.Id });
            await service.Delete(createAllowed.Id);
        }

        // ICrudModelService.Create, forbidden
        var createForbidden = () => service.Create(dtoCreateForbidden);
        await createForbidden.Should().ThrowAsync<ForbiddenException>();

        // ICrudModelService.Update, allowed
        var updateAllowed = await service.Update(dtoAllowed);
        updateAllowed.Should().BeEquivalentTo(new { dtoAllowed.Id });

        // ICrudModelService.Update, forbidden
        var updateForbidden = () => service.Update(dtoForbidden);
        await updateForbidden.Should().ThrowAsync<ForbiddenException>();

        // ICrudModelService.Delete, allowed
        var deleteAllowed = await service.Delete(dtoAllowed.Id);
        deleteAllowed.Should().Be(1);

        // ICrudModelService.Delete, forbidden
        var deleteForbidden = () => service.Delete(dtoForbidden.Id);
        await deleteForbidden.Should().ThrowAsync<ForbiddenException>();
    }

    private static async Task<TestData> Initialize(Faker faker)
    {
        var testData = new TestData(faker);

        faker.ConfigureInMemoryDatabase();
        faker.ConfigureAuthorization(true, testData.OwnUser);
        await faker.ProvideUsers(testData.OwnUser);
        faker.Provide<IActivityApiService, ActivityService>();
        faker.Provide<ICustomerApiService, CustomerService>();
        faker.Provide<IOrderApiService, OrderService>();
        faker.Provide<IProjectApiService, ProjectService>();
        faker.Provide<ITimeSheetApiService, TimeSheetService>();
        faker.Provide<ITypeaheadApiService, TypeaheadService>();

        var dbRepository = faker.GetRequiredService<IDbRepository>();
        await dbRepository.AddRange(testData.Allowed.Values.ToList());
        await dbRepository.AddRange(testData.Forbidden.Values.ToList());
        await dbRepository.SaveChanges();

        return testData;
    }

    private class TestData
    {
        public Dictionary<Type, IEntityModel> Allowed { get; }
        public Dictionary<Type, IEntityModel> Forbidden { get; }

        public UserDto OwnUser { get; }

        public TestData(Faker faker)
        {
            var customerAllowed = faker.Customer.Create("Allowed");
            var customerForbidden = faker.Customer.Create("Forbidden");
            var activityAllowed = faker.Activity.Create(customerAllowed.Id);
            var activityForbidden = faker.Activity.Create(customerForbidden.Id);
            var orderAllowed = faker.Order.Create(customerAllowed.Id);
            var orderForbidden = faker.Order.Create(customerForbidden.Id);
            var projectAllowed = faker.Project.Create(customerAllowed.Id);
            var projectForbidden = faker.Project.Create(customerForbidden.Id);
            var timeSheetAllowed = faker.TimeSheet.Create(customerAllowed.Id, activityAllowed.Id);
            var timeSheetForbidden = faker.TimeSheet.Create(customerForbidden.Id, activityForbidden.Id);
            OwnUser = faker.User.Create("Own", permissions: DefaultPermissions.WritePermissions, restrictToCustomers: new[] { customerAllowed.Id });

            Allowed = new Dictionary<Type, IEntityModel>
            {
                [typeof(Customer)] = customerAllowed,
                [typeof(Activity)] = activityAllowed,
                [typeof(Order)] = orderAllowed,
                [typeof(Project)] = projectAllowed,
                [typeof(TimeSheet)] = timeSheetAllowed,
            };

            Forbidden = new Dictionary<Type, IEntityModel>
            {
                [typeof(Customer)] = customerForbidden,
                [typeof(Activity)] = activityForbidden,
                [typeof(Order)] = orderForbidden,
                [typeof(Project)] = projectForbidden,
                [typeof(TimeSheet)] = timeSheetForbidden,
            };
        }

        public TModel GetAllowed<TModel>()
            => (TModel)Allowed[typeof(TModel)];

        public TModel GetForbidden<TModel>()
            => (TModel)Forbidden[typeof(TModel)];
    }
}