using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using UnityEngine;
using RimWorld;

namespace Oof
{
    public static class HardModeUtil
    {
        public static int TendTime(this BetterInjury injury, Pawn pawn)
        {
            int result = (int)(1f / pawn.GetStatValue(StatDefOf.MedicalTendSpeed, true) * 600f);
            

            if ((injury.Part?.depth ?? BodyPartDepth.Undefined) == BodyPartDepth.Inside)
            {
                result = (int)(result * 1.5f);
            }

            Log.Message(injury.Label + " " + (injury.Part?.Label ?? "(no bodypart)")  + " tend time " + result.ToString());
            return result;
        }

        public static int TendTime(this Hediff hediff, Pawn pawn)
        {
            int result = (int)(1f / pawn.GetStatValue(StatDefOf.MedicalTendSpeed, true) * 600f);

            return result;
        }

        public static void DecreaseStack(this Thing t)
        {
            if(t.stackCount <= 1)
            {
                t.Destroy();
            }
            --t.stackCount;
        }
    }
}
