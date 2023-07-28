using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class Gene_DustHediffGiver : Gene_DustDrain
    {

        // public HediffDef HediffDefName => def.GetModExtension<GeneExtension_Giver>().hediffDefName;
        public List<HediffDef> HediffDefs => def.GetModExtension<GeneExtension_Giver>().hediffDefs;
        public List<BodyPartDef> Bodyparts => def.GetModExtension<GeneExtension_Giver>().bodyparts;

        public override void PostAdd()
        {
            base.PostAdd();
            foreach (HediffDef hediffDef in HediffDefs)
            {
                if (!pawn.health.hediffSet.HasHediff(hediffDef))
                {
                    Gene_PermanentHediff.BodyPartGiver(Bodyparts, pawn, hediffDef);
                }
            }
        }
    }

}
