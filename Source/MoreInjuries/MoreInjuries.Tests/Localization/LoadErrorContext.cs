using System.Text;

namespace MoreInjuries.Tests.Localization;

public class LoadErrorContext
{
    public List<string> Errors { get; } = [];

    public StringBuilder Builder { get; } = new();
}