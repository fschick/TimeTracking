using FluentAssertions;
using FluentAssertions.Specialized;
using FS.TimeTracking.Core.Exceptions;
using FS.TimeTracking.Core.Models.Application.Core;
using System.Threading.Tasks;

namespace FS.TimeTracking.Application.Tests.Extensions;

public static class AssertionExtensions
{
    public static Task<ExceptionAssertions<ForbiddenException>> ThrowForbiddenForeignUserDataAsync<TTask, TAssertions>(this AsyncFunctionAssertions<TTask, TAssertions> assertion)
        where TTask : Task
        where TAssertions : AsyncFunctionAssertions<TTask, TAssertions>
    {
        return assertion
            .ThrowAsync<ForbiddenException>()
            .Where(x => x.ErrorCode == ApplicationErrorCode.ForbiddenForeignUserData);
    }

    public static Task<ExceptionAssertions<ForbiddenException>> ThrowForbiddenAsync<TTask, TAssertions>(this AsyncFunctionAssertions<TTask, TAssertions> assertion, string because = "")
        where TTask : Task
        where TAssertions : AsyncFunctionAssertions<TTask, TAssertions>
    {
        return assertion
            .ThrowAsync<ForbiddenException>(because)
            .Where(x => x.ErrorCode == ApplicationErrorCode.Forbidden);
    }

    public static Task<ExceptionAssertions<ConflictException>> ThrowConflictAsync<TTask, TAssertions>(this AsyncFunctionAssertions<TTask, TAssertions> assertion)
        where TTask : Task
        where TAssertions : AsyncFunctionAssertions<TTask, TAssertions>
    {
        return assertion
            .ThrowAsync<ConflictException>()
            .Where(x => x.ErrorCode == ApplicationErrorCode.Conflict);
    }
}