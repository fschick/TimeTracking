using System;
using System.ComponentModel.DataAnnotations;
using FS.TimeTracking.Shared.Models.MasterData;

namespace FS.TimeTracking.Shared.DTOs.MasterData;

/// <inheritdoc cref="Project"/>
public class ProjectListDto
{
    /// <inheritdoc cref="Project.Id"/>
    [Required]
    public Guid Id { get; set; }

    /// <inheritdoc cref="Project.Title"/>
    [Required]
    public string Title { get; set; }

    /// <inheritdoc cref="Customer.Title"/>
    public string CustomerTitle { get; set; }

    /// <inheritdoc cref="Project.Hidden"/>
    public bool Hidden { get; set; }
}