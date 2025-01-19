namespace MoreInjuries.BuildIntrinsics;

internal static class Justifications
{
    public const string XML_DEF_REQUIRES_FIELD = "Cannot encapsulate replace XML-bound fields with properties, required for reflection. Name must match XML def name.";
    public const string XML_NAMING_CONVENTION = "Comply with default RimWorld XML naming convention";
    public const string XML_DEF_OF_REQUIRES_FIELD = "Cannot encapsulate DefOf fields in properties, required for reflection. Name must match XML def name.";
}
