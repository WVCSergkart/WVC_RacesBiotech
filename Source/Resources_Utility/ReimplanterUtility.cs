using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

    public static class ReimplanterUtility
    {

        public static void ReimplantGenesBase(Pawn caster, Pawn recipient)
        {
            recipient.genes.SetXenotype(caster.genes.Xenotype);
            recipient.genes.xenotypeName = caster.genes.xenotypeName;
            recipient.genes.iconDef = caster.genes.iconDef;
            Pawn_GeneTracker recipientGenes = recipient.genes;
            if (recipientGenes != null && recipientGenes.GenesListForReading.Count > 0)
            {
                foreach (Gene item in recipient.genes?.GenesListForReading)
                {
                    recipient.genes?.RemoveGene(item);
                }
            }
            foreach (Gene endogene in caster.genes.Endogenes)
            {
                recipient.genes.AddGene(endogene.def, xenogene: false);
            }
            foreach (Gene xenogene in caster.genes.Xenogenes)
            {
                recipient.genes.AddGene(xenogene.def, xenogene: true);
            }
            if (!caster.genes.Xenotype.soundDefOnImplant.NullOrUndefined())
            {
                caster.genes.Xenotype.soundDefOnImplant.PlayOneShot(SoundInfo.InMap(recipient));
            }
            recipient.health.AddHediff(HediffDefOf.XenogerminationComa);
            GeneUtility.UpdateXenogermReplication(recipient);
        }

		public static List<GeneDef> GenesNonCandidatesForSerums()
		{
			List<GeneDef> list = new();
			foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
			{
				list.AddRange(item.nonCandidatesForSerums);
			}
			return list;
		}

		public static List<GeneDef> GenesPerfectCandidatesForSerums()
		{
			List<GeneDef> list = new();
			foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
			{
				list.AddRange(item.perfectCandidatesForSerums);
			}
			return list;
		}

        public static bool DelayedReimplanterIsActive(Pawn pawn)
        {
            if (pawn.health != null && pawn.health.hediffSet != null)
            {
                List<HediffDef> hediffDefs = new();
                foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
                {
                    hediffDefs.AddRange(item.blackListedHediffDefForReimplanter);
                }
                for (int i = 0; i < hediffDefs.Count; i++)
                {
                    if (pawn.health.hediffSet.HasHediff(hediffDefs[i]))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

    }
}
