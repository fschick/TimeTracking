﻿namespace FS.TimeTracking.Abstractions.DTOs.Shared;

/// <summary>
/// Product information DTO.
/// </summary>
public class ProductInformationDto
{
    /// <summary>
    /// The name of the product.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The version of the product.
    /// </summary>
    public string Version { get; set; }

    /// <summary>
    /// The copyright of the product.
    /// </summary>
    public string Copyright { get; set; }
}