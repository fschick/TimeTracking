using System;
using System.ComponentModel.DataAnnotations;

namespace FS.TimeTracking.Shared.DTOs.Report;

/// <summary>
/// Time sheet report grid data transfer object.
/// </summary>
public class ActivityReportGridDto
{
    /// <summary>
    /// The unique identifier of the entity.
    /// </summary>
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the start date.
    /// </summary>
    [Required]
    public DateTimeOffset StartDate { get; set; }

    /// <summary>
    ///  Gets or sets the end date.
    /// </summary>
    public DateTimeOffset? EndDate { get; set; }

    /// <summary>
    ///  Gets or sets the total working time.
    /// </summary>
    public TimeSpan? Duration { get; set; }

    /// <summary>
    /// Gets or sets the comment.
    /// </summary>
    public string Comment { get; set; }

    /// <summary>
    /// Gets or sets the related issue/ticket/... .
    /// </summary>
    public string Issue { get; set; }

    /// <summary>
    /// Gets or sets the customer title.
    /// </summary>
    public string CustomerTitle { get; set; }

    /// <summary>
    /// Gets or sets the project title.
    /// </summary>
    public string ProjectTitle { get; set; }

    /// <summary>
    /// Gets or sets the activity title.
    /// </summary>
    public string ActivityTitle { get; set; }

    /// <summary>
    /// Gets or sets the order title.
    /// </summary>
    public string OrderTitle { get; set; }

    /// <summary>
    /// Gets or sets the order number.
    /// </summary>
    public string OrderNumber { get; set; }

    /// <summary>
    /// Indicates whether this item is billable.
    /// </summary>
    [Required]
    public bool Billable { get; set; }

    /// <summary>
    /// Gets or sets the name of the customer's company.
    /// </summary>
    public string CustomerCompanyName { get; set; }

    /// <summary>
    /// Gets or sets the customer's department.
    /// </summary>
    public string CustomerDepartment { get; set; }

    /// <summary>
    /// Gets or sets the name of the customer's contact.
    /// </summary>
    public string CustomerContactName { get; set; }

    /// <summary>
    /// Gets or sets entity the report should grouped by.
    /// </summary>
    public string GroupBy { get; set; }
}