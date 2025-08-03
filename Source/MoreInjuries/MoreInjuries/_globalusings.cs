global using System;
global using Logger = MoreInjuries.Logging.Logger;
global using Std = System;
global using System.Diagnostics.CodeAnalysis;
global using static MoreInjuries.Globals;
global using static MoreInjuries.Roslyn.BuildIntrinsics.Suppressions;

namespace MoreInjuries;

public static class Globals
{
    /// <summary>
    /// A typed null value to be used for parameter-less switch expressions.
    /// </summary>
    public const object? __ = null;
}