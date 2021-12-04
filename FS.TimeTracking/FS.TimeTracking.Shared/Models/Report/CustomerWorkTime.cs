using FS.TimeTracking.Shared.Models.MasterData;
using System;
using System.ComponentModel.DataAnnotations;

namespace FS.TimeTracking.Shared.Models.Report
{
    /// <summary>
    /// Work times per customer.
    /// </summary>
    public class CustomerWorkTime
    {
        /// <inheritdoc cref="Customer.Id"/>
        [Required]
        public Guid Id { get; set; }

        /// <inheritdoc cref="Customer.Title"/>
        [Required]
        public string Title { get; set; }

        /// <inheritdoc cref="Order.Id"/>
        public Guid? OrderId { get; set; }

        /// <inheritdoc cref="Order.Title"/>
        public string OrderTitle { get; set; }

        /// <inheritdoc cref="Order.Number"/>
        public string OrderNumber { get; set; }

        /// <summary>
        /// Worked time.
        /// </summary>
        [Required]
        public TimeSpan WorkedTime { get; set; }

        /// <summary>
        /// Worked time in work days.
        /// </summary>
        [Required]
        public double WorkedDays { get; set; }

        /// <summary>
        /// Planned time.
        /// </summary>
        [Required]
        public TimeSpan PlannedTime { get; set; }

        /// <summary>
        /// Planned time in work days.
        /// </summary>
        [Required]
        public double PlannedDays { get; set; }
    }
}
