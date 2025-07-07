using MoreInjuries.Roslyn.CompilerServices;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace MoreInjuries.Logging;

internal static partial class Logger
{
    [CompileTimePlatformInfo]
    private static partial CompileTimePlatformInfo CompileTimePlatformInfo { get; }

    public static void Log(string message)
    {
        if (MoreInjuriesMod.Settings.EnableLogging)
        {
            Verse.Log.Message($"[{nameof(MoreInjuries)}] {message}");
        }
    }

    public static void LogVerbose(string message)
    {
        if (MoreInjuriesMod.Settings.EnableVerboseLogging)
        {
            Log(message);
        }
    }

    [Conditional("DEBUG")]
    public static void LogDebug(string message) => Log(message);

    public static void Error(string message, [CallerFilePath] string callsite = "", [CallerLineNumber] int lineNo = -1) =>
        Verse.Log.Error($"[{nameof(MoreInjuries)}] {FormatCallSite(callsite, lineNo)}{message}");

    public static void Warning(string message) =>
        Verse.Log.Warning($"[{nameof(MoreInjuries)}] {message}");

    public static void LogAlways(string message) =>
        Verse.Log.Message($"[{nameof(MoreInjuries)}] {message}");

#if DEBUG
    [DoesNotReturn]
#endif
    public static void ConfigError(string message, [CallerFilePath] string callsite = "", [CallerLineNumber] int lineNo = -1)
    {
        string formattedMessage = $"[{nameof(MoreInjuries)}] {FormatCallSite(callsite, lineNo)}Config Error: {message}";
        Verse.Log.Error(formattedMessage);
#if DEBUG
        throw new InvalidOperationException(formattedMessage);
#endif
    }

    private static string FormatCallSite(string callsite, int lineNo)
    {
        if (string.IsNullOrEmpty(callsite))
        {
            return string.Empty;
        }
        int fileNameIndex = callsite.LastIndexOf(CompileTimePlatformInfo.DirectorySeperatorChar);
        if (fileNameIndex == -1 || ++fileNameIndex >= callsite.Length)
        {
            return $"{callsite}::L{lineNo}: ";
        }
        return $"{callsite.Substring(fileNameIndex)}::L{lineNo}: ";
    }
}
