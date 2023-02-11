using Microsoft.AspNetCore.Authorization;
using System;

namespace FS.TimeTracking.Api.REST.Extensions;

internal static class AuthorizationOptionsExtensions
{
    public static void ExtendPolicy(this AuthorizationOptions options, string name, Action<AuthorizationPolicyBuilder> configurePolicy)
        => options.AddPolicy(name, policy =>
        {
            var existingPolicy = options.GetPolicy(name);
            if (existingPolicy != null)
                policy.Combine(existingPolicy);
            configurePolicy.Invoke(policy);
        });
}