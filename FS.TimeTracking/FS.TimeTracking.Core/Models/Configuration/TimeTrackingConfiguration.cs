﻿using FS.TimeTracking.Core.Extensions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace FS.TimeTracking.Core.Models.Configuration;

/// <summary>
/// Global application configuration
/// </summary>
[ExcludeFromCodeCoverage]
public class TimeTrackingConfiguration
{
    /// <summary>
    /// The configuration section bind to.
    /// </summary>
    public const string CONFIGURATION_SECTION = "TimeTracking";

    /// <summary>
    /// Pathname of the web user interface folder.
    /// </summary>
    public const string WEB_UI_FOLDER = "webui";

    /// <summary>
    /// Pathname of the configuration folder.
    /// </summary>
    public const string CONFIG_FOLDER = "config";

    /// <summary>
    /// Pathname of folder containing translation files.
    /// </summary>
    public const string TRANSLATION_FOLDER = "translations";

    /// <summary>
    /// Name of the configuration file.
    /// </summary>
    /// <autogeneratedoc />
    public const string CONFIG_BASE_NAME = "FS.TimeTracking.config";

    /// <summary>
    /// The nlog configuration file.
    /// </summary>
    /// <autogeneratedoc />
    public const string NLOG_CONFIGURATION_FILE = CONFIG_BASE_NAME + ".nlog";

    /// <summary>
    /// Full pathname of the executable file.
    /// </summary>
    /// <autogeneratedoc />
    public static readonly string ExecutablePath = AssemblyExtensions.GetProgramDirectory();

    /// <summary>
    /// Path to the content root.
    /// </summary>
    /// <autogeneratedoc />
    public static readonly string PathToContentRoot = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"
        ? Directory.GetCurrentDirectory()
        : ExecutablePath;

    /// <summary>
    /// Database specific configuration.
    /// </summary>
    public DatabaseConfiguration Database { get; set; } = new();

    /// <summary>
    /// Keycloak specific configuration.
    /// </summary>
    public KeycloakConfiguration Keycloak { get; set; } = new();

    /// <summary>
    /// Report specific configuration.
    /// </summary>
    public ReportingConfiguration Reporting { get; set; } = new();

    /// <summary>
    /// Enable or disable feature modules
    /// </summary>
    public FeatureConfiguration Features { get; set; } = new();
}