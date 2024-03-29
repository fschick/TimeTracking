﻿using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Api.REST.Routing;

/// <summary>
/// Feature modules flags.
/// </summary>
/// <autogeneratedoc />
[SuppressMessage("ReSharper", "InconsistentNaming")]
[ExcludeFromCodeCoverage]
public static class Features
{
    /// <summary>
    /// Enable / disable authentication and authorization using Keycloak.
    /// </summary>
    public const string Authorization = nameof(Authorization);

    /// <summary>
    /// Enable / disable reporting module.
    /// </summary>
    public const string Reporting = nameof(Reporting);
}