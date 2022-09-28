using System;
using System.Collections.Generic;

namespace FS.TimeTracking.Application.Tests.Services;

public class LimitToSecondsComparer : IEqualityComparer<TimeSpan>
{
    public bool Equals(TimeSpan x, TimeSpan y)
        => (int)x.TotalSeconds == (int)y.TotalSeconds;

    public int GetHashCode(TimeSpan obj)
        => EqualityComparer<TimeSpan>.Default.GetHashCode(obj);
}