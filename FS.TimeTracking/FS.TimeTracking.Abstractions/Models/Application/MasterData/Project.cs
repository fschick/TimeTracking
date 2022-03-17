using FS.TimeTracking.Abstractions.Interfaces.Models;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace FS.TimeTracking.Abstractions.Models.Application.MasterData;

/// <summary>
/// Project
/// </summary>
[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
public class Project : IIdEntityModel
{
    /// <inheritdoc />
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// The display name of this item.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Title { get; set; }

    /// <summary>
    /// Identifier to the related <see cref="MasterData.Customer"/>.
    /// </summary>
    [Required]
    public Guid CustomerId { get; set; }

    /// <inheritdoc cref="MasterData.Customer"/>
    public Customer Customer { get; set; }

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