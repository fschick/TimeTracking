﻿#if DEBUG
using Bogus;
using FS.TimeTracking.Shared.Interfaces.Application.Services;
using FS.TimeTracking.Shared.Interfaces.Services;
using FS.TimeTracking.Shared.Models.TimeTracking;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Application.Services
{
    /// <inheritdoc />
    public class DebugService : IDebugService
    {
        private readonly IRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="DebugService"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <autogeneratedoc />
        public DebugService(IRepository repository)
            => _repository = repository;

        /// <inheritdoc />
        public async Task<object> TestMethod(CancellationToken cancellationToken = default)
        {
            await _repository.Add(new Customer { ShortName = "Test" }, cancellationToken);
            await _repository.SaveChanges(cancellationToken);
            var customer = await _repository.FirstOrDefault((Customer x) => x, cancellationToken: cancellationToken);
            return Task.FromResult<object>(1);
        }

        /// <inheritdoc />
        public async Task SeedData(int amount = 10, bool truncateBeforeSeed = false)
        {
            if (truncateBeforeSeed)
                await TruncateData();

            const string locale = "de";
            static string comment(Faker faker) => faker.Lorem.Sentences(faker.Random.Number(0, 3), ".");
            static bool hidden(Faker faker) => faker.Random.WeightedRandom(new[] { true, false }, new[] { .2f, .8f });

            var customers = new Faker<Customer>(locale)
                .StrictMode(true)
                .RuleFor(x => x.Id, f => f.Random.Uuid())
                .RuleFor(x => x.CompanyName, faker => faker.Company.CompanyName())
                .RuleFor(x => x.ShortName, (_, entity) => Regex.Replace(entity.CompanyName, @"^(\w+).*$", "$1"))
                .RuleFor(x => x.ContactName, faker => $"{faker.Name.FirstName()} {faker.Name.LastName()}")
                .RuleFor(x => x.Street, faker => faker.Address.StreetAddress())
                .RuleFor(x => x.ZipCode, f => f.Address.ZipCode())
                .RuleFor(x => x.City, f => f.Address.City())
                .RuleFor(x => x.Country, new RegionInfo(locale).NativeName)
                .RuleFor(x => x.Comment, comment)
                .RuleFor(x => x.Hidden, hidden)
                .RuleFor(x => x.Created, default(DateTime))
                .RuleFor(x => x.Modified, default(DateTime))
                .RuleFor(x => x.Projects, default(List<Project>))
                .Generate(amount)
                .ToList();

            var projects = new Faker<Project>(locale)
                .StrictMode(true)
                .RuleFor(x => x.Id, f => f.Random.Uuid())
                .RuleFor(x => x.Name, faker => faker.Commerce.ProductName())
                .RuleFor(x => x.CustomerId, faker => faker.PickRandom(customers.Select(c => c.Id)))
                .RuleFor(x => x.Customer, default(Customer))
                .RuleFor(x => x.Comment, comment)
                .RuleFor(x => x.Hidden, hidden)
                .RuleFor(x => x.Created, default(DateTime))
                .RuleFor(x => x.Modified, default(DateTime))
                .Generate(amount * 2)
                .ToList();

            var activities = new Faker<Activity>(locale)
                .StrictMode(true)
                .RuleFor(x => x.Id, f => f.Random.Uuid())
                .RuleFor(x => x.Name, faker => faker.Hacker.Verb())
                .RuleFor(x => x.CustomerId, faker => faker.PickRandom(customers.Select(c => (Guid?)c.Id).Concat(Enumerable.Repeat<Guid?>(null, customers.Count))))
                .RuleFor(x => x.ProjectId, (faker, entity) => entity.CustomerId != null ? null : faker.PickRandom(projects.Select(c => (Guid?)c.Id).Concat(Enumerable.Repeat<Guid?>(null, projects.Count))))
                .RuleFor(x => x.Comment, comment)
                .RuleFor(x => x.Hidden, hidden)
                .RuleFor(x => x.Created, default(DateTime))
                .RuleFor(x => x.Modified, default(DateTime))
                .Generate(amount)
                .ToList();

            var referenceDate = DateTime.Now.AddYears(amount / 10 * -1);
            var timesheet = new Faker<TimeSheet>()
                .RuleFor(x => x.Id, f => f.Random.Uuid())
                .RuleFor(x => x.CustomerId, faker => faker.PickRandom(customers.Select(c => c.Id)))
                .RuleFor(x => x.ActivityId, faker => faker.PickRandom(activities.Select(c => c.Id)))
                .RuleFor(x => x.StartTime, faker => referenceDate = referenceDate.AddMinutes(faker.Random.Number((int)TimeSpan.FromDays(5).TotalMinutes)))
                .RuleFor(x => x.EndTime, faker => referenceDate = referenceDate.AddMinutes(faker.Random.Number((int)TimeSpan.FromHours(8).TotalMinutes)))
                .RuleFor(x => x.Billable, f => f.Random.Bool())
                .RuleFor(x => x.Comment, comment)
                .RuleFor(x => x.Created, default(DateTime))
                .RuleFor(x => x.Modified, default(DateTime))
                .Generate(amount * 10)
                .ToList();

            //var debug = JsonConvert.SerializeObject(new { customers, projects, activities, timesheet }, Formatting.Indented);
            await _repository.BulkAddRange(customers);
            await _repository.BulkAddRange(projects);
            await _repository.BulkAddRange(activities);
            await _repository.BulkAddRange(timesheet);
        }

        /// <inheritdoc />
        public async Task TruncateData()
        {
            await _repository.Remove<TimeSheet>();
            await _repository.Remove<Activity>();
            await _repository.Remove<Project>();
            await _repository.Remove<Customer>();
            await _repository.SaveChanges();
        }
    }
}
#endif