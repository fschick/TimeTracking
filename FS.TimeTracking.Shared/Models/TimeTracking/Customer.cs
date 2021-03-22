using FS.TimeTracking.Shared.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FS.TimeTracking.Shared.Models.TimeTracking
{
    /// <summary>
    /// Customer
    /// </summary>
    public class Customer : IEntityModel
    {
        /// <inheritdoc />
        [Required]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the short/display name.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string ShortName { get; set; }

        /// <summary>
        /// Gets or sets the name of the company.
        /// </summary>
        [StringLength(100)]
        public string CompanyName { get; set; }

        /// <summary>
        /// Gets or sets the name of the contact.
        /// </summary>
        [StringLength(100)]
        public string ContactName { get; set; }

        /// <summary>
        /// Gets or sets the street.
        /// </summary>
        [StringLength(100)]
        public string Street { get; set; }

        /// <summary>
        /// Gets or sets the zip code.
        /// </summary>
        [StringLength(100)]
        public string ZipCode { get; set; }

        /// <summary>
        /// Gets or sets the city.
        /// </summary>
        [StringLength(100)]
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the country.
        /// </summary>
        [StringLength(100)]
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets a comment.
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this item is hidden.
        /// </summary>
        [Required]
        public bool Hidden { get; set; }

        /// <inheritdoc />
        [Required]
        public DateTime Created { get; set; }

        /// <inheritdoc />
        [Required]
        public DateTime Modified { get; set; }

        /// <summary>
        /// Gets or sets the projects related to this customer.
        /// </summary>
        public List<Project> Projects { get; set; }
    }
}
