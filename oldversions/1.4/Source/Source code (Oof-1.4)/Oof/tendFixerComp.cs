using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using RimWorld;
using UnityEngine;

namespace Oof
{
    public class tendFixerComp : HediffComp
    {
        public bool SemiFixed = false;
        public override void CompTended(float quality, float maxQuality, int batchPosition = 0)
        {
            base.CompTended(quality, maxQuality, batchPosition);
            if (quality > 0.8f)
            {
                SemiFixed = true;
                this.parent.Severity = Math.Min(0.15f, this.parent.Severity);
            }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            if (SemiFixed)
            {
                severityAdjustment = 0.00001f;
            }
            base.CompPostTick(ref severityAdjustment);
           
        }
    }
}
