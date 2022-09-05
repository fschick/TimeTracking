using FileTypeChecker;
using SixLabors.ImageSharp;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace FS.TimeTracking.Abstractions.Attributes.Validation;

/// <summary>
/// Validates an uploaded image.
/// </summary>
internal class ImageAttribute : ValidationAttribute
{
    private const string IMAGE_STREAM_NOT_SEEKABLE = "Image stream must be seekable / buffered in order to be validated.";

    /// <summary>
    /// Gets or sets the maximum size of the file/stream in bytes.
    /// </summary>
    public long MaxFileSize { get; set; }

    /// <summary>
    /// Gets or sets the maximum width of the image.
    /// </summary>
    public int MaxImageWidth { get; set; }

    /// <summary>
    /// Gets or sets the maximum height of the image.
    /// </summary>
    public int MaxImageHeight { get; set; }

    /// <inheritdoc />
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        try
        {
            switch (value)
            {
                case null:
                    return ValidationResult.Success;
                case byte[] bytes:
                    using (var stream = new MemoryStream(bytes))
                        return ValidateImageStream(stream, validationContext.DisplayName);
                case Stream stream:
                    return ValidateImageStream(stream, validationContext.DisplayName);
                default:
                    return new ValidationResult($"Validation of images of type {value.GetType()} is unsupported.");
            }
        }
        catch (UnknownImageFormatException)
        {
            return new ValidationResult("Image cannot be loaded: Unknown image format.");
        }
        catch (NotSupportedException ex) when (ex.Message.StartsWith("ImageSharp does not support this"))
        {
            return new ValidationResult("Image cannot be loaded: Unknown image format.");
        }
        catch (NotSupportedException ex) when (ex.Message == IMAGE_STREAM_NOT_SEEKABLE)
        {
            return new ValidationResult(ex.Message);
        }
        catch
        {
            return new ValidationResult("Unknown error occurred.");
        }
    }

    private ValidationResult ValidateImageStream(Stream imageStream, string memberName)
    {
        if (MaxFileSize != 0 && imageStream.Length > MaxFileSize)
            return new ValidationResult($"{memberName} exceeds max file/stream length.");

        if (!imageStream.CanSeek)
            throw new NotSupportedException(IMAGE_STREAM_NOT_SEEKABLE);

        imageStream.Position = 0;
        using var memoryStream = new MemoryStream();
        imageStream.CopyTo(memoryStream);
        if (memoryStream.Length == 0)
            return ValidationResult.Success;

        imageStream.Position = 0;
        if (!FileTypeValidator.IsImage(imageStream))
            return new ValidationResult($"{memberName} not recognized as image.");

        imageStream.Position = 0;
        using var image = Image.Load(imageStream);
        if (MaxImageWidth != 0 && image.Width > MaxImageWidth)
            return new ValidationResult($"{memberName} exceeds max image width.");

        if (MaxImageHeight != 0 && image.Height > MaxImageHeight)
            return new ValidationResult($"{memberName} exceeds max image height.");

        return ValidationResult.Success;
    }
}