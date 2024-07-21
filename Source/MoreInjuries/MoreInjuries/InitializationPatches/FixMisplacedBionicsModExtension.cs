using System.Collections.Generic;
using Verse;

namespace MoreInjuries.InitializationPatches;

public class FixMisplacedBionicsModExtension : DefModExtension
{
    public required List<BodyPartDef> BodyParts { get; set; }
}
