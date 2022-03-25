using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Abstractions.Enums;
using FS.TimeTracking.Abstractions.Interfaces.Application.Services.Shared;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Abstractions.Interfaces.Application.Services.MasterData;

/// <inheritdoc />
public interface IHolidayService : ICrudModelService<HolidayDto, HolidayGridDto>
{
    /// <summary>
    /// Imports holidays/public holidays from iCal file.
    /// </summary>
    /// <param name="file">The file to import.</param>
    /// <param name="type">The holiday type.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    Task Import(IFormFile file, HolidayType type, CancellationToken cancellationToken = default);
}