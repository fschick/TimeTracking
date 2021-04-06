using Bogus;
using FS.TimeTracking.Shared.Interfaces.Application.Services;
using FS.TimeTracking.Shared.Interfaces.Services;
using FS.TimeTracking.Shared.Models.TimeTracking;
using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FS.TimeTracking.Application.Services
{
    /// <inheritdoc />
    public class TestDataService : ITestDataService
    {
        private readonly IRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestDataService"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        public TestDataService(IRepository repository)
            => _repository = repository;

        /// <inheritdoc />
        public async Task SeedTestData(int amount = 10, bool truncateBeforeSeed = false)
        {
            const string locale = "de";
            static string comment(Faker faker) => faker.Lorem.Sentences(faker.Random.Number(0, 3), ".");
            static bool hidden(Faker faker) => faker.Random.WeightedRandom(new[] { true, false }, new[] { .2f, .8f });
            var referenceDate = DateTimeOffset.Now.AddYears(amount / 10 * -1);

            var customers = new Faker<Customer>(locale)
                .StrictMode(true)
                .RuleFor(x => x.Id, f => f.Random.Uuid())
                .RuleFor(x => x.CompanyName, faker => faker.Company.CompanyName())
                .RuleFor(x => x.Title, (_, entity) => Regex.Replace(entity.CompanyName, @"^(\w+).*$", "$1"))
                .RuleFor(x => x.ContactName, faker => $"{faker.Name.FirstName()} {faker.Name.LastName()}")
                .RuleFor(x => x.Street, faker => faker.Address.StreetAddress())
                .RuleFor(x => x.ZipCode, f => f.Address.ZipCode())
                .RuleFor(x => x.City, f => f.Address.City())
                .RuleFor(x => x.Country, new RegionInfo(locale).NativeName)
                .RuleFor(x => x.Comment, comment)
                .RuleFor(x => x.Hidden, hidden)
                .RuleFor(x => x.Created, default(DateTime))
                .RuleFor(x => x.Modified, default(DateTime))
                .RuleFor(x => x.Projects, _ => default)
                .RuleFor(x => x.Orders, f => default)
                .Generate(amount)
                .ToList();

            var projects = new Faker<Project>(locale)
                .StrictMode(true)
                .RuleFor(x => x.Id, f => f.Random.Uuid())
                .RuleFor(x => x.Title, faker => faker.Commerce.ProductName())
                .RuleFor(x => x.CustomerId, faker => faker.PickRandom(customers.Select(c => c.Id)))
                .RuleFor(x => x.Customer, _ => default)
                .RuleFor(x => x.Comment, comment)
                .RuleFor(x => x.Hidden, hidden)
                .RuleFor(x => x.Created, default(DateTime))
                .RuleFor(x => x.Modified, default(DateTime))
                .Generate(amount * 2)
                .ToList();

            var activities = new Faker<Activity>(locale)
                .StrictMode(true)
                .RuleFor(x => x.Id, f => f.Random.Uuid())
                .RuleFor(x => x.Title, faker => faker.Hacker.IngVerb())
                .RuleFor(x => x.ProjectId, (faker, entity) => faker.PickRandom(projects.Select(c => (Guid?)c.Id).Concat(Enumerable.Repeat<Guid?>(null, projects.Count))))
                .RuleFor(x => x.Project, _ => default)
                .RuleFor(x => x.Comment, comment)
                .RuleFor(x => x.Hidden, hidden)
                .RuleFor(x => x.Created, default(DateTime))
                .RuleFor(x => x.Modified, default(DateTime))
                .Generate(amount)
                .ToList();

            var orders = new Faker<Order>(locale)
                .StrictMode(true)
                .RuleFor(o => o.Id, f => f.Random.Uuid())
                .RuleFor(o => o.Title, f => f.Hacker.Phrase())
                .RuleFor(o => o.Description, f => f.Hacker.Phrase())
                .RuleFor(o => o.Number, f => f.Random.Replace("???-****-##"))
                .RuleFor(x => x.CustomerId, faker => faker.PickRandom(customers.Select(c => c.Id)))
                .RuleFor(o => o.Customer, _ => default)
                .RuleFor(o => o.StartDateUtc, default(DateTime))
                .RuleFor(o => o.StartDateOffset, default(double))
                .RuleFor(x => x.StartDate, faker => referenceDate = referenceDate.AddDays(faker.Random.Number((int)TimeSpan.FromDays(5).TotalDays)))
                .RuleFor(o => o.DueDateUtc, default(DateTime))
                .RuleFor(o => o.DueDateOffset, default(double))
                .RuleFor(x => x.DueDate, faker => referenceDate = referenceDate.AddDays(faker.Random.Number((int)TimeSpan.FromHours(8).TotalDays)))
                .RuleFor(o => o.Budget, faker => faker.Random.Number(15, 1500) * 8 * 75)
                .RuleFor(o => o.HourlyRate, faker => faker.Random.Number(50, 150))
                .RuleFor(x => x.Comment, comment)
                .RuleFor(x => x.Hidden, hidden)
                .RuleFor(x => x.Created, default(DateTime))
                .RuleFor(x => x.Modified, default(DateTime))
                .Generate((int)(amount / 2d))
                .ToList();

            var timesheet = new Faker<TimeSheet>()
                .RuleFor(x => x.Id, f => f.Random.Uuid())
                .RuleFor(x => x.ProjectId, faker => faker.PickRandom(projects.Select(c => c.Id)))
                .RuleFor(x => x.OrderId, faker => faker.PickRandom(orders.Select(c => c.Id)))
                .RuleFor(x => x.ActivityId, faker => faker.PickRandom(activities.Select(c => c.Id)))
                .RuleFor(x => x.Issue, f => f.Lorem.Word())
                .RuleFor(x => x.StartDateUtc, default(DateTime))
                .RuleFor(x => x.StartDateOffset, default(double))
                .RuleFor(x => x.StartDate, faker => referenceDate = referenceDate.AddMinutes(faker.Random.Number((int)TimeSpan.FromDays(5).TotalMinutes)))
                .RuleFor(x => x.EndDateUtc, default(DateTime?))
                .RuleFor(x => x.EndDateOffset, default(double?))
                .RuleFor(x => x.EndDate, faker => referenceDate = referenceDate.AddMinutes(faker.Random.Number((int)TimeSpan.FromHours(8).TotalMinutes)))
                .RuleFor(x => x.Billable, f => f.Random.Bool())
                .RuleFor(x => x.Comment, comment)
                .RuleFor(x => x.Created, default(DateTime))
                .RuleFor(x => x.Modified, default(DateTime))
                .Generate(amount * 10)
                .ToList();

            using var scope = _repository.CreateTransactionScope();

            if (truncateBeforeSeed)
                await TruncateData();

            await _repository.BulkAddRange(customers);
            await _repository.BulkAddRange(projects);
            await _repository.BulkAddRange(orders);
            await _repository.BulkAddRange(activities);
            await _repository.BulkAddRange(timesheet);

            scope.Complete();
        }

        /// <inheritdoc />
        public async Task TruncateData()
        {
            using var scope = _repository.CreateTransactionScope();

            await _repository.Remove<TimeSheet>();
            await _repository.Remove<Activity>();
            await _repository.Remove<Order>();
            await _repository.Remove<Project>();
            await _repository.Remove<Customer>();

            scope.Complete();
        }
    }
}
