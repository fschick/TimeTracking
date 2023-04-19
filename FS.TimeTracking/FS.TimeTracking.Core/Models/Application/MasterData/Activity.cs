using FS.TimeTracking.Abstractions.DTOs.MasterData;
using FS.TimeTracking.Core.Interfaces.Models;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Core.Models.Application.MasterData;

/// <inheritdoc cref="ActivityDto"/>
[ExcludeFromCodeCoverage]
[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
public class Activity : IIdEntityModel, ICustomerLinkedModel
{
    /// <inheritdoc />
    [Required]
    public Guid Id { get; set; }

    /// <inheritdoc cref="ActivityDto.Id"/>
    [Required]
    [StringLength(100)]
    public string Title { get; set; }

    /// <summary>
    /// Identifier to the related <see cref="MasterData.Customer"/>.
    /// </summary>
    public Guid? CustomerId { get; set; }

    /// <inheritdoc cref="MasterData.Project"/>
    public Customer Customer { get; set; }

    /// <summary>
    /// Identifier to the related <see cref="MasterData.Project"/>.
    /// </summary>
    public Guid? ProjectId { get; set; }

    /// <inheritdoc cref="MasterData.Project"/>
    public Project Project { get; set; }

    /// <summary>
    /// Comment for this item.
    /// </summary>
    public string Comment { get; set; }

    /// <summary>
    /// Indicates whether this item is hidden.
    /// </summary>
    [Required]
    public bool Hidden { get; set; }

    /// <inheritdoc />
    [Required]
    public DateTime Created { get; set; }

    /// <inheritdoc />
    [Required]
    public DateTime Modified { get; set; }

    [JsonIgnore]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"{Title} {(Customer != null ? $"({Customer.Title})" : string.Empty)}";
}