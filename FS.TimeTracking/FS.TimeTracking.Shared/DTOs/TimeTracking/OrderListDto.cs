using FS.TimeTracking.Shared.Models.TimeTracking;
using System;
using System.ComponentModel.DataAnnotations;

namespace FS.TimeTracking.Shared.DTOs.TimeTracking
{
    /// <inheritdoc cref="Order"/>
    public class OrderListDto
    {
        /// <inheritdoc cref="Order.Id"/>
        [Required]
        public Guid Id { get; set; }

        /// <inheritdoc cref="Order.Title"/>
        [Required]
        public string Title { get; set; }

        /// <inheritdoc cref="Customer.Title"/>
        public string CustomerTitle { get; set; }

        /// <inheritdoc cref="Order.StartDate"/>
        [Required]
        public DateTimeOffset StartDate { get; set; }

        /// <inheritdoc cref="Order.DueDate"/>
        [Required]
        public DateTimeOffset DueDate { get; set; }

        /// <inheritdoc cref="Order.Hidden"/>
        public bool Hidden { get; set; }
    }
}
