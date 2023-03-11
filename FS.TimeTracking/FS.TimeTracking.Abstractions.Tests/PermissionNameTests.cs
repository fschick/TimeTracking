using FluentAssertions;
using FS.TimeTracking.Abstractions.Constants;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace FS.TimeTracking.Abstractions.Tests;

[TestClass, ExcludeFromCodeCoverage]
public class PermissionNameTests
{
    [TestMethod]
    public void EnsureDefaultPermissions_HaveAllPermissionsInitialized()
    {
        DefaultPermissions.NoPermissions.Select(x => x.Name).Should().BeEquivalentTo(PermissionName.All);
        DefaultPermissions.ReadPermissions.Select(x => x.Name).Should().BeEquivalentTo(PermissionName.All);
        DefaultPermissions.WritePermissions.Select(x => x.Name).Should().BeEquivalentTo(PermissionName.All);
    }
}