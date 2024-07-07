using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using UnityEngine;

namespace Oof
{
    public class FixerModExt : DefModExtension
    {
        public List<BodyPartDef> bodyparts;
    }

    [StaticConstructorOnStartup]
    public class ConstructorFixPart
    {
        static ConstructorFixPart()
        {
            var A = DefDatabase<RecipeDef>.AllDefs.Where(x => 
            (x.addsHediff?.addedPartProps ?? null) != null
            &&
            (((x.appliedOnFixedBodyParts != null) && x.appliedOnFixedBodyParts.Count > 0))

            );


            foreach (RecipeDef recDef in A)
            {

                if (recDef.addsHediff.modExtensions == null)
                {
                    recDef.addsHediff.modExtensions = new List<DefModExtension>();
                }

                if (recDef.appliedOnFixedBodyParts != null && recDef.appliedOnFixedBodyParts.Count > 0)
                {
                    recDef.addsHediff.modExtensions.Add(new FixerModExt { bodyparts = recDef.appliedOnFixedBodyParts });
                }

               
            }
        }
    }
}
