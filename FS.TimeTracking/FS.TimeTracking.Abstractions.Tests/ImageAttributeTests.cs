using FluentAssertions;
using FS.TimeTracking.Abstractions.Attributes.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FS.TimeTracking.Abstractions.Tests;

[TestClass, ExcludeFromCodeCoverage]
public class ImageAttributeTests
{
    private static readonly Assembly _resourceAssembly = typeof(ImageAttributeTests).GetTypeInfo().Assembly;

    [DataTestMethod]
    [DynamicData(nameof(GetTestImages), DynamicDataSourceType.Method)]
    public void WhenValidImageDataIsValidated_ValidationSucceeds(string imageResourceName)
    {
        // Prepare
        using var resource = _resourceAssembly.GetManifestResourceStream(imageResourceName);
        if (resource == null)
            throw new ArgumentException($"Image resource with name {imageResourceName} not found.", nameof(imageResourceName));

        var imageValidation = new ImageValidation { ImageStream = resource, ImageBytes = ReadBytes(resource) };

        // Act
        var isValid = ValidateImage(imageValidation);

        // Check
        isValid.Should().BeTrue();
    }

    [TestMethod]
    public void WhenNullImageDataIsValidated_ValidationSucceeds()
    {
        var nullImageValidation = new ImageValidation { ImageStream = null, ImageBytes = null };
        var isValid = ValidateImage(nullImageValidation);
        isValid.Should().BeTrue();
    }

    [TestMethod]
    public void WhenEmptyImageDataIsValidated_ValidationSucceeds()
    {
        var imageBytes = Array.Empty<byte>();
        using var imageStream = new MemoryStream(imageBytes);
        var nullImageValidation = new ImageValidation { ImageStream = imageStream, ImageBytes = imageBytes };
        var isValid = ValidateImage(nullImageValidation);
        isValid.Should().BeTrue();
    }

    [DataTestMethod]
    [DynamicData(nameof(BadTestImages), DynamicDataSourceType.Method)]
    public void WhenInvalidImageDataIsValidated_ValidationFails(byte[] badImageBytes)
    {
        using var imageStream = new MemoryStream(badImageBytes);
        var nullImageValidation = new ImageValidation { ImageStream = imageStream, ImageBytes = badImageBytes };
        var isValid = ValidateImage(nullImageValidation);
        isValid.Should().BeFalse();
    }

    [DataTestMethod]
    [DynamicData(nameof(BigTestImages), DynamicDataSourceType.Method)]
    public void WhenTooBigImageDataIsValidated_ValidationFails(byte[] bigImageBytes)
    {
        using var imageStream = new MemoryStream(bigImageBytes);
        var nullImageValidation = new ImageValidation { ImageStream = imageStream, ImageBytes = bigImageBytes };
        var isValid = ValidateImage(nullImageValidation);
        isValid.Should().BeFalse();
    }

    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
    private class ImageValidation
    {
        [Image(MaxFileSize = 1024, MaxImageHeight = 400, MaxImageWidth = 400)]
        public Stream ImageStream { get; set; }

        [Image(MaxFileSize = 1024, MaxImageHeight = 400, MaxImageWidth = 400)]
        public byte[] ImageBytes { get; set; }
    }

    private static IEnumerable<object[]> GetTestImages()
        => _resourceAssembly
            .GetManifestResourceNames()
            .Where(resourceName => resourceName.Contains("ImageAttributeTestImage"))
            .Select(imageResourceName => new object[] { imageResourceName });

    private static byte[] ReadBytes(Stream stream)
    {
        using var memoryStream = new MemoryStream();
        stream.Position = 0;
        stream.CopyTo(memoryStream);
        return memoryStream.ToArray();
    }

    private static bool ValidateImage(ImageValidation imageValidation)
    {
        var context = new ValidationContext(imageValidation);
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(imageValidation, context, validationResults, true);
        return isValid;
    }

    private static byte[] CreateImage(int width, int height)
    {
        using var memoryStream = new MemoryStream();
        using var image = new Image<Abgr32>(width, height);
        image.SaveAsPng(memoryStream);
        return memoryStream.ToArray();
    }

    private static IEnumerable<object[]> BadTestImages()
    {
        yield return new object[] { new byte[] { 0, 0 } }; // Bad at all
        yield return new object[] { new byte[] { 0x42, 0x4d } }; // PNG without data
    }

    private static IEnumerable<object[]> BigTestImages()
    {
        yield return new object[] { CreateImage(1, 401) };
        yield return new object[] { CreateImage(401, 1) };
        yield return new object[] { CreateImage(399, 399) };
    }

}