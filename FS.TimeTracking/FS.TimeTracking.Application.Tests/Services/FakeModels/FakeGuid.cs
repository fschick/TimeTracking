using System;
using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Application.Tests.Services.FakeModels;

[ExcludeFromCodeCoverage]
public class FakeGuid
{
    private readonly Faker _faker;

    public FakeGuid(Faker faker)
        => _faker = faker;

    public Guid Create()
    {
        var guidBuffer = new byte[16];
        _faker.Random.NextBytes(guidBuffer);
        return new Guid(guidBuffer);
    }
}