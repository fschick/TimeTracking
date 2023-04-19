using FS.Keycloak.RestApiClient.Model;
using FS.TimeTracking.Abstractions.Constants;
using FS.TimeTracking.Abstractions.DTOs.Administration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FS.TimeTracking.Application.Extensions;

internal static class UserRepresentationExtensions
{
    public static List<Guid> GetRestrictedCustomerIds(this UserRepresentation user)
        => user.Attributes?.TryGetValue(RestrictToCustomer.ATTRIBUTE, out var customerIds) == true
            ? customerIds.Select(Guid.Parse).ToList()
            : new List<Guid>();

    public static Dictionary<string, List<string>> SetRestrictedCustomerIds(this UserRepresentation user, UserDto userDto)
    {
        user.Attributes ??= new Dictionary<string, List<string>>();

        return user.Attributes
            .Where(isNotRestrictedCustomerId)
            .Concat(userDto.GetRestrictedCustomerIdAttribute())
            .ToDictionary(x => x.Key, x => x.Value);

        static bool isNotRestrictedCustomerId(KeyValuePair<string, List<string>> attribute)
            => attribute.Key != RestrictToCustomer.ATTRIBUTE;
    }

    private static Dictionary<string, List<string>> GetRestrictedCustomerIdAttribute(this UserDto userDto)
        => new() { [RestrictToCustomer.ATTRIBUTE] = userDto.RestrictToCustomerIds?.Select(a => a.ToString()).ToList() };
}