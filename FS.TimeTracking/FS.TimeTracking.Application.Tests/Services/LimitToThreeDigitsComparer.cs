using System;
using System.Collections.Generic;

namespace FS.TimeTracking.Application.Tests.Services;

public class LimitToThreeDigitsComparer : IEqualityComparer<double>
{
    public bool Equals(double x, double y)
        => Math.Abs(Math.Round(x, 3, MidpointRounding.AwayFromZero) - Math.Round(y, 3, MidpointRounding.AwayFromZero)) < 0.00001;

    public int GetHashCode(double obj)
        => EqualityComparer<double>.Default.GetHashCode(obj);
}