using System.Text;

namespace MoreInjuries.LocalizationTests.Model;

public class LoadErrorContext
{
    public List<string> Errors { get; } = [];

    public StringBuilder Builder { get; } = new();
}