using Bogus;
using FS.TimeTracking.Core.Extensions;
using FS.TimeTracking.Core.Interfaces.Application.Services.Shared;
using FS.TimeTracking.Core.Interfaces.Repository.Services.Database;
using FS.TimeTracking.Core.Models.Application.MasterData;
using FS.TimeTracking.Core.Models.Application.TimeTracking;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TimeZoneConverter;

namespace FS.TimeTracking.Application.Services.Shared;

/// <inheritdoc />
public class TestDataService : ITestDataApiService
{
    private readonly IDbRepository _dbRepository;
    private readonly IWorkdayService _workDaysService;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestDataService"/> class.
    /// </summary>
    /// <param name="dbRepository">The repository.</param>
    /// <param name="workDaysService">The work days service.</param>
    public TestDataService(IDbRepository dbRepository, IWorkdayService workDaysService)
    {
        _dbRepository = dbRepository;
        _workDaysService = workDaysService;
    }

    /// <inheritdoc />
    public async Task SeedTestData(int amount = 10, string timeZoneId = null, bool truncateBeforeSeed = false)
    {
        const string locale = "de";
        static string comment(Faker faker) => faker.Lorem.Sentences(faker.Random.Number(0, 3), ".");
        static bool hidden(Faker faker) => faker.Random.WeightedRandom(new[] { true, false }, new[] { .2f, .8f });
        var random = new Random();
        var createdModified = DateTime.UtcNow;

        var customers = new Faker<Customer>(locale)
            .StrictMode(true)
            .RuleFor(x => x.Id, faker => faker.Random.Uuid())
            .RuleFor(x => x.CompanyName, faker => faker.Company.CompanyName())
            .RuleFor(x => x.Title, (_, entity) => Regex.Replace(entity.CompanyName, @"^(\w+).*$", "$1"))
            .RuleFor(x => x.Number, faker => faker.Random.Replace("######"))
            .RuleFor(x => x.Department, faker => faker.Commerce.Department(1))
            .RuleFor(x => x.ContactName, faker => $"{faker.Name.FirstName()} {faker.Name.LastName()}")
            .RuleFor(x => x.HourlyRate, faker => faker.Random.Number(50, 150))
            .RuleFor(x => x.Street, faker => faker.Address.StreetAddress())
            .RuleFor(x => x.ZipCode, faker => faker.Address.ZipCode())
            .RuleFor(x => x.City, faker => faker.Address.City())
            .RuleFor(x => x.Country, new RegionInfo(locale).NativeName)
            .RuleFor(x => x.Comment, comment)
            .RuleFor(x => x.Hidden, hidden)
            .RuleFor(x => x.Created, createdModified)
            .RuleFor(x => x.Modified, createdModified)
            .RuleFor(x => x.Projects, _ => default)
            .RuleFor(x => x.Orders, _ => default)
            .Generate(amount)
            .ToList();

        var projects = new Faker<Project>(locale)
            .StrictMode(true)
            .RuleFor(project => project.Id, faker => faker.Random.Uuid())
            .RuleFor(project => project.Title, faker => faker.PickRandom(ProjectCodeNames.CodeNames))
            .RuleFor(project => project.CustomerId, faker => faker.PickRandom(customers.Select(c => c.Id).OrNull(faker)))
            .RuleFor(project => project.Customer, _ => default)
            .RuleFor(project => project.Comment, (_, project) => ProjectCodeNames.GetDescription(project.Title))
            .RuleFor(project => project.Hidden, hidden)
            .RuleFor(project => project.Created, createdModified)
            .RuleFor(project => project.Modified, createdModified)
            .Generate(amount * 2)
            .ToList();

        using var activityEnumerator = DevelopmentActivities.Activities.OrderBy(_ => random.Next()).GetEnumerator();
        var activities = new Faker<Activity>(locale)
            .StrictMode(true)
            .RuleFor(activity => activity.Id, faker => faker.Random.Uuid())
            .RuleFor(activity => activity.Title, _ => activityEnumerator.MoveNext() ? activityEnumerator.Current : null)
            .RuleFor(activity => activity.CustomerId, (faker, _) => faker.PickRandom(customers.Select(c => c.Id)).OrNull(faker))
            .RuleFor(activity => activity.Customer, _ => default)
            .RuleFor(activity => activity.ProjectId, (faker, _) => faker.PickRandom(projects.Select(c => c.Id)).OrNull(faker))
            .RuleFor(activity => activity.Project, _ => default)
            .RuleFor(activity => activity.Comment, comment)
            .RuleFor(activity => activity.Hidden, hidden)
            .RuleFor(activity => activity.Created, createdModified)
            .RuleFor(activity => activity.Modified, createdModified)
            .Generate(Math.Min(amount, DevelopmentActivities.Activities.Count))
            .ToList();

        var timeZone = timeZoneId != null ? TZConvert.GetTimeZoneInfo(timeZoneId) : TimeZoneInfo.Utc;
        var referenceDate = DateTime.UtcNow.Date.AddYears(amount / 10 * -1);
        var orderDate = referenceDate.AddDays(random.Next(-15, 15));
        var orders = new Faker<Order>(locale)
            .StrictMode(true)
            .RuleFor(order => order.Id, faker => faker.Random.Uuid())
            .RuleFor(order => order.Title, faker => faker.PickRandom(activities.Select(activity => $"{activity.Title} {activity.Customer?.Title ?? faker.PickRandom(customers.Select(x => x.Title))}")))
            .RuleFor(order => order.Description, faker => faker.Hacker.Phrase())
            .RuleFor(order => order.Number, faker => faker.Random.Replace("???-****-##"))
            .RuleFor(order => order.CustomerId, faker => faker.PickRandom(customers.Select(c => c.Id)))
            .RuleFor(order => order.Customer, _ => default)
            .RuleFor(order => order.StartDateLocal, default(DateTime))
            .RuleFor(order => order.StartDateOffset, default(int))
            .RuleFor(order => order.StartDate, _ => orderDate.ConvertTo(timeZone))
            .RuleFor(order => order.DueDateLocal, default(DateTime))
            .RuleFor(order => order.DueDateOffset, default(int))
            .RuleFor(order => order.DueDate, faker => orderDate.AddDays(faker.Random.Number(15, 120)).ConvertTo(timeZone))
            .RuleFor(order => order.HourlyRate, faker => faker.Random.Number(50, 150))
            .RuleFor(order => order.Budget, (faker, order) => (order.DueDate - order.StartDate).TotalDays * 8 * order.HourlyRate * faker.Random.Double(0.8, 1.2))
            .RuleFor(order => order.Comment, comment)
            .RuleFor(order => order.Hidden, hidden)
            .RuleFor(order => order.Created, createdModified)
            .RuleFor(order => order.Modified, createdModified)
            .FinishWith((faker, _) => orderDate = orderDate.AddDays(faker.Random.Number(30, 90)))
            .Generate((int)(amount / 2d))
            .ToList();

        var timesheetRules = new Faker<TimeSheet>()
            .RuleFor(timeSheet => timeSheet.Id, faker => faker.Random.Uuid())
            //.RuleFor(timeSheet => timeSheet.ProjectId, faker => faker.PickRandom(projects.Select(c => c.Id)))
            //.RuleFor(timeSheet => timeSheet.OrderId, faker => faker.PickRandom(orders.Select(c => c.Id)))
            .RuleFor(timeSheet => timeSheet.ActivityId, faker => faker.PickRandom(activities.Select(c => c.Id)))
            .RuleFor(timeSheet => timeSheet.Issue, faker => faker.Lorem.Word())
            .RuleFor(timeSheet => timeSheet.StartDateLocal, default(DateTime))
            .RuleFor(timeSheet => timeSheet.StartDateOffset, default(int))
            //.RuleFor(timeSheet => timeSheet.StartDate, faker => timesheetDate = timesheetDate.AddMinutes(faker.Random.Number((int)TimeSpan.FromDays(5).TotalMinutes)))
            .RuleFor(timeSheet => timeSheet.EndDateLocal, default(DateTime?))
            .RuleFor(timeSheet => timeSheet.EndDateOffset, default(int?))
            //.RuleFor(timeSheet => timeSheet.EndDate, faker => timesheetDate = timesheetDate.AddMinutes(faker.Random.Number((int)TimeSpan.FromHours(8).TotalMinutes)))
            .RuleFor(timeSheet => timeSheet.Billable, faker => faker.Random.Bool())
            .RuleFor(timeSheet => timeSheet.Comment, comment)
            .RuleFor(timeSheet => timeSheet.Created, createdModified)
            .RuleFor(timeSheet => timeSheet.Modified, createdModified);

        var timesSheets = new List<TimeSheet>();
        var minDate = orders.Min(x => x.StartDateLocal);
        var maxDate = orders.Max(x => x.DueDateLocal);
        var workingDays = await _workDaysService.GetWorkdays(null, minDate, maxDate);
        var randomizer = new Randomizer();

        foreach (var workDay in workingDays.PublicWorkdaysOfAllUsers)
        {
            var minStartOfWOrk = (int)TimeSpan.FromHours(5).TotalMinutes; // 5 o'clock in the morning
            var maxStartOfWOrk = (int)TimeSpan.FromHours(9).TotalMinutes; // 9 o'clock in the morning
            var startOfWork = workDay.AddMinutes(randomizer.Number(minStartOfWOrk, maxStartOfWOrk));

            var minWorkingTime = (int)TimeSpan.FromHours(2).TotalMinutes; // 2 hours
            var maxWorkingTime = (int)TimeSpan.FromHours(14).TotalMinutes; // 14 hours
            var endOfWork = startOfWork.AddMinutes(randomizer.Number(minWorkingTime, maxWorkingTime));

            while (startOfWork < endOfWork)
            {
                var startOfActivity = startOfWork;
                var endOfActivity = startOfWork.AddMinutes(randomizer.Number(10, 420)); // 10 minutes - 7 hours

                timesheetRules = timesheetRules
                    .RuleFor(timeSheet => timeSheet.ProjectId, faker => faker.PickRandom(projects.Select(c => c.Id)).OrNull(faker, .2f))
                    .RuleFor(timeSheet => timeSheet.OrderId, faker => faker.PickRandom(orders.Select(c => c.Id)).OrNull(faker, .2f))
                    .RuleFor(timeSheet => timeSheet.StartDate, _ => startOfActivity.ConvertTo(timeZone))
                    .RuleFor(timeSheet => timeSheet.EndDate, _ => endOfActivity.ConvertTo(timeZone));

                timesSheets.Add(timesheetRules.Generate());

                var isLunchTime = endOfActivity.TimeOfDay.TotalHours is > 11 and < 14; // Lunch time 11:00 am - 2:00 pm
                var pause = isLunchTime ? randomizer.Number(15, 90) : randomizer.Number(0, 10);
                startOfWork = endOfActivity.AddMinutes(pause);
            }
        }

        using var scope = _dbRepository.CreateTransactionScope();

        if (truncateBeforeSeed)
            await TruncateData();

        await _dbRepository.BulkAddRange(customers);
        await _dbRepository.BulkAddRange(projects);
        await _dbRepository.BulkAddRange(activities);
        await _dbRepository.BulkAddRange(orders);
        await _dbRepository.BulkAddRange(timesSheets);

        scope.Complete();
    }

    /// <inheritdoc />
    public async Task TruncateData()
    {
        using var scope = _dbRepository.CreateTransactionScope();

        await _dbRepository.BulkRemove<TimeSheet>();
        await _dbRepository.BulkRemove<Order>();
        await _dbRepository.BulkRemove<Activity>();
        await _dbRepository.BulkRemove<Project>();
        await _dbRepository.BulkRemove<Customer>();
        await _dbRepository.BulkRemove<Holiday>();
        await _dbRepository.BulkRemove<Setting>();

        scope.Complete();
    }
}