#if DEBUG
using FS.TimeTracking.Api.REST.Routing;
using FS.TimeTracking.Core.Interfaces.Application.Services.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace FS.TimeTracking.Api.REST.Controllers.Shared
{
    /// <inheritdoc cref="ITestDataApiService" />
    /// <seealso cref="ControllerBase" />
    /// <seealso cref="ITestDataApiService" />
    [ApiV1Controller]
    [Authorize]
    [ExcludeFromCodeCoverage]
    public class TestDataController : ControllerBase, ITestDataApiService
    {
        private readonly ITestDataApiService _testDataService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestDataController"/> class.
        /// </summary>
        /// <param name="testDataService">The test data service.</param>
        public TestDataController(ITestDataApiService testDataService)
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