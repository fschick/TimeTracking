using FS.TimeTracking.Shared.DTOs.Report;
using FS.TimeTracking.Shared.Models.MasterData;
using System;
using System.ComponentModel.DataAnnotations;

namespace FS.TimeTracking.Shared.Models.Report
{
    /// <summary>
    /// Work times per order.
    /// </summary>
    public class OrderWorkTime
    {
        /// <inheritdoc cref="Order.Id"/>
        [Required]
        public Guid Id { get; set; }

        /// <inheritdoc cref="Order.Title"/>
        [Required]
        public string Title { get; set; }

        /// <inheritdoc cref="Order.Number"/>
        [Required]
        public string Number { get; set; }

        /// <inheritdoc cref="Customer.Id"/>
        [Required]
        public Guid CustomerId { get; set; }

        /// <inheritdoc cref="Customer.Title"/>
        [Required]
        public string CustomerTitle { get; set; }

        /// <inheritdoc cref="WorkTimeDto.TimeWorked"/>
        [Required]
        public TimeSpan WorkedTime { get; set; }

        /// <inheritdoc cref="WorkTimeDto.DaysWorked"/>
        [Required]
        public double WorkedDays { get; set; }

        /// <inheritdoc cref="WorkTimeDto.BudgetWorked"/>
        [Required]
        public double WorkedBudget => WorkedTime.TotalHours * HourlyRate;

        /// <inheritdoc cref="WorkTimeDto.TimePlanned"/>
        [Required]
        public TimeSpan PlannedTime { get; set; }

        /// <inheritdoc cref="WorkTimeDto.DaysPlanned"/>
        [Required]
        public double PlannedDays { get; set; }

        /// <inheritdoc cref="WorkTimeDto.BudgetPlanned"/>
        [Required]
        public double PlannedBudget => PlannedTime.TotalHours * HourlyRate;

        /// <inheritdoc cref="Order.HourlyRate"/>
        [Required]
        public double HourlyRate { get; set; }

        /// <inheritdoc cref="WorkTimeDto.Currency"/>
        [Required]
        public string Currency { get; set; }

        /// <inheritdoc cref="Order.StartDate"/>
        [Required]
        public DateTimeOffset PlannedStart { get; set; }

        /// <inheritdoc cref="Order.DueDate"/>
        [Required]
        public DateTimeOffset PlannedEnd { get; set; }
    }
}
