using FS.TimeTracking.Shared.Interfaces.Application.Services.Shared;
using System.Threading;

namespace FS.TimeTracking.Shared.DTOs.Shared;

/// <summary>
/// DTO returned by <see cref="IInformationService.GetProductInformation(CancellationToken)"/>
/// </summary>
public class ProductInformationDto
{
    /// <summary>
    /// The name of the product.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The version of the product.
    /// </summary>
    public string Version { get; set; }

    /// <summary>
    /// The copyright of the product.
    /// </summary>
    public string Copyright { get; set; }
}