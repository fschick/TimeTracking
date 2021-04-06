using FS.TimeTracking.Shared.Attributes;
using FS.TimeTracking.Shared.Models.TimeTracking;
using System;
using System.ComponentModel.DataAnnotations;

namespace FS.TimeTracking.Shared.DTOs.TimeTracking
{
    /// <inheritdoc cref="Order"/>
    [ValidationDescription]
    public class OrderListDto
    {
        /// <inheritdoc cref="Order.Id"/>
        [Required]
        public Guid Id { get; set; }

        /// <inheritdoc cref="Order.Title"/>
        [Required]
        public string Title { get; set; }

        /// <inheritdoc cref="Order.Number"/>
        public string Number { get; set; }

        /// <inheritdoc cref="Customer.Title"/>
        public string CustomerTitle { get; set; }

        /// <inheritdoc cref="Customer.CompanyName"/>
        public string CustomerCompanyName { get; set; }

        /// <inheritdoc cref="Order.StartDate"/>
        [Required]
        public DateTimeOffset StartDate { get; set; }

        /// <inheritdoc cref="Order.DueDate"/>
        [Required]
        public DateTimeOffset DueDate { get; set; }

        /// <inheritdoc cref="Order.HourlyRate"/>
        public double HourlyRate { get; set; }

        /// <inheritdoc cref="Order.Budget"/>
        public double Budget { get; set; }

        /// <inheritdoc cref="Order.Hidden"/>
        public bool Hidden { get; set; }
    }
}
