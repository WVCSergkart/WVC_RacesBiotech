using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class Gene_ResurgentCells : Gene_Resource, IGeneResourceDrain
    {
        public bool woundClottingAllowed = true;
        public bool ageReversionAllowed = true;
        public bool totalHealingAllowed = true;

        public Gene_Resource Resource => this;

        public Pawn Pawn => pawn;

        public bool CanOffset
        {
            get
            {
                if (Active)
                {
                    // return !pawn.Deathresting;
                    return true;
                }
                return false;
            }
        }

        public string DisplayLabel => Label + " (" + "Gene".Translate() + ")";

        public float ResourceLossPerDay => def.resourceLossPerDay;

        public override float InitialResourceMax => 1.0f;

        // public override float Max => 5.0f;

        public override float MinLevelForAlert => 0.05f;

        public override float MaxLevelOffset => 0.20f;

        protected override Color BarColor => new ColorInt(93, 101, 126).ToColor;

        protected override Color BarHighlightColor => new ColorInt(123, 131, 156).ToColor;

        public override void PostAdd()
        {
            base.PostAdd();
            Reset();
            ResetResourceValue();
        }

        private void ResetResourceValue()
        {
            FloatRange floatRange = new(0.06f, 0.97f);
            Value = floatRange.RandomInRange;
        }

        // public override void Notify_IngestedThing(Thing thing, int numTaken)
        // {
        // if (thing.def.IsMeat)
        // {
        // IngestibleProperties ingestible = thing.def.ingestible;
        // if (ingestible != null && ingestible.sourceDef?.race?.Humanlike == true)
        // {
        // GeneUtility.OffsetHemogen(pawn, 0.0375f * thing.GetStatValue(StatDefOf.Nutrition) * (float)numTaken);
        // }
        // }
        // }

        public override void Tick()
        {
            base.Tick();
            UndeadUtility.TickResourceDrain(this);
        }

        // public override void SetTargetValuePct(float val)
        // {
        // targetValue = Mathf.Clamp(val * Max, 0f, Max - MaxLevelOffset);
        // }

        // public bool ShouldConsumeHemogenNow()
        // {
        // return Value < targetValue;
        // }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo gizmo in base.GetGizmos())
            {
                yield return gizmo;
            }
            foreach (Gizmo resourceDrainGizmo in UndeadUtility.GetResourceDrainGizmos(this))
            {
                yield return resourceDrainGizmo;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref woundClottingAllowed, "woundClottingAllowed", defaultValue: true);
            Scribe_Values.Look(ref ageReversionAllowed, "ageReversionAllowed", defaultValue: true);
            Scribe_Values.Look(ref totalHealingAllowed, "totalHealingAllowed", defaultValue: true);
        }
    }

}
