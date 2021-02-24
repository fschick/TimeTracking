using FS.TimeTracking.Shared.DTOs.TimeTracking;
using FS.TimeTracking.Shared.Interfaces.Application.Converters;
using FS.TimeTracking.Shared.Models.TimeTracking;

namespace FS.TimeTracking.Application.ModelConverters
{
    /// <inheritdoc />
    public class CustomerConverter : IModelConverter<Customer, CustomerDto>
    {
        /// <inheritdoc />
        public CustomerDto ToDto(Customer model)
            => new CustomerDto
            {
                Id = model.Id,
                ShortName = model.ShortName,
                CompanyName = model.CompanyName,
                ContactName = model.ContactName,
                Street = model.Street,
                ZipCode = model.ZipCode,
                City = model.City,
                Country = model.Country,
                Hidden = model.Hidden,
            };

        /// <inheritdoc />
        public Customer FromDto(CustomerDto dto)
            => new Customer
            {
                Id = dto.Id,
                ShortName = dto.ShortName,
                CompanyName = dto.CompanyName,
                ContactName = dto.ContactName,
                Street = dto.Street,
                ZipCode = dto.ZipCode,
                City = dto.City,
                Country = dto.Country,
                Hidden = dto.Hidden,
            };
    }
}
