using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public static class DustUtility
	{

		public static void OffsetNeedFood(Pawn pawn, float offset)
		{
			// if (!ModsConfig.BiotechActive)
			// {
				// return;
			// }
			Need_Food need_Food = pawn.needs?.food;
			if (need_Food != null)
			{
				need_Food.CurLevel += offset;
			}
		}

		// public static void OffsetDust(Pawn pawn, float offset)
		// {
			// if (!ModsConfig.BiotechActive)
			// {
				// return;
			// }
			// Gene_Dust gene_Hemogen = pawn.genes?.GetFirstGeneOfType<Gene_Dust>();
			// if (gene_Hemogen != null)
			// {
				// gene_Hemogen.Value += offset;
			// }
		// }

		public static bool PawnInPronePosition(Pawn pawn)
		{
			if (pawn.Downed || pawn.Deathresting || RestUtility.InBed(pawn))
			{
				return true;
			}
			return false;
		}

		public static void ReimplantGenes(Pawn caster, Pawn recipient)
		{
			if (!ModLister.CheckBiotech("xenogerm reimplantation"))
			{
				return;
			}
			ReimplanterUtility.ReimplantGenesBase(caster, recipient);
			if (PawnUtility.ShouldSendNotificationAbout(recipient))
			{
				int max = HediffDefOf.XenogerminationComa.CompProps<HediffCompProperties_Disappears>().disappearsAfterTicks.max;
				Find.LetterStack.ReceiveLetter("LetterLabelGenesImplanted".Translate(), "WVC_LetterTextGenesImplanted".Translate(recipient.Named("TARGET"), max.ToStringTicksToPeriod().Named("COMADURATION")), LetterDefOf.NeutralEvent, new LookTargets(recipient));
			}
		}

	}
}
