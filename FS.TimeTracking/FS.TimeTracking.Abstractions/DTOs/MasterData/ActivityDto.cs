using FS.TimeTracking.Abstractions.Attributes;
using FS.TimeTracking.Abstractions.Interfaces.DTOs;
using Newtonsoft.Json;
using Plainquire.Filter.Abstractions;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Abstractions.DTOs.MasterData;

/// <summary>
/// Activity
/// </summary>
[ValidationDescription]
[EntityFilter(Prefix = "Activity")]
[ExcludeFromCodeCoverage]
[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
public record ActivityDto : IIdEntityDto, IManageableDto, ICustomerLinkedDto
{
    ///  <summary>
    /// The display name for this item.
    /// </summary>
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// The title for this item.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Title { get; set; }

    /// <summary>
    /// Identifier to the related <see cref="CustomerDto"/>.
    /// </summary>
    [Filter(Filterable = false)]
    public Guid? CustomerId { get; set; }

    /// <summary>
    /// Identifier to the related <see cref="ProjectDto"/>.
    /// </summary>
    [Filter(Filterable = false)]
    public Guid? ProjectId { get; set; }

    /// <summary>
    /// ID of the project's customer. Unused while create/update the entity.
    /// </summary>
    [Filter(Filterable = false)]
    public Guid? ProjectCustomerId { get; set; }

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
    [Filter(Filterable = false)]
    public bool? IsReadonly { get; set; }

    [JsonIgnore]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"{Title}";
}