using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class Gene_AddOrRemoveHediff : Gene
    {

        public HediffDef HediffDefName => def.GetModExtension<GeneExtension_Giver>().hediffDefName;

        public override void PostAdd()
        {
            base.PostAdd();
            AddOrRemoveHediff(HediffDefName, pawn, this);
        }

        public override void Tick()
        {
            base.Tick();
            if (!pawn.IsHashIntervalTick(60000))
            {
                return;
            }
            AddOrRemoveHediff(HediffDefName, pawn, this);
        }

        public override void PostRemove()
        {
            base.PostRemove();
            RemoveHediff(HediffDefName, pawn);
        }

        public static void AddOrRemoveHediff(HediffDef hediffDef, Pawn pawn, Gene gene)
        {
            if (gene.Active)
            {
                if (!pawn.health.hediffSet.HasHediff(hediffDef))
                {
                    pawn.health.AddHediff(hediffDef);
                }
            }
            else
            {
                RemoveHediff(hediffDef, pawn);
            }
        }

        public static void RemoveHediff(HediffDef hediffDef, Pawn pawn)
        {
            if (pawn.health.hediffSet.HasHediff(hediffDef))
            {
                Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(hediffDef);
                if (firstHediffOfDef != null)
                {
                    pawn.health.RemoveHediff(firstHediffOfDef);
                }
            }
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            if (DebugSettings.ShowDevGizmos)
            {
                yield return new Command_Action
                {
                    defaultLabel = "DEV: Add Or Remove Hediff",
                    action = delegate
                    {
                        if (Active)
                        {
                            AddOrRemoveHediff(HediffDefName, pawn, this);
                        }
                    }
                };
            }
        }

    }

}
