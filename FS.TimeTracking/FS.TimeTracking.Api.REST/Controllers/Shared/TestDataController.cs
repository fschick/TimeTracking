#if DEBUG
using FS.TimeTracking.Api.REST.Routing;
using FS.TimeTracking.Core.Interfaces.Application.Services.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FS.TimeTracking.Api.REST.Controllers.Shared
{
    /// <inheritdoc cref="ITestDataService" />
    /// <seealso cref="ControllerBase" />
    /// <seealso cref="ITestDataService" />
    [V1ApiController]
    public class TestDataController : ControllerBase, ITestDataService
    {
        private readonly ITestDataService _testDataService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestDataController"/> class.
        /// </summary>
        /// <param name="testDataService">The test data service.</param>
        public TestDataController(ITestDataService testDataService)
            => _testDataService = testDataService;

        /// <inheritdoc />
        [HttpPost]
        public async Task SeedTestData(int amount = 10, string timeZoneId = null, bool truncateBeforeSeed = false)
            => await _testDataService.SeedTestData(amount, timeZoneId, truncateBeforeSeed);

        /// <inheritdoc />
        [HttpDelete]
        public async Task TruncateData()
            => await _testDataService.TruncateData();
    }
}
#endif