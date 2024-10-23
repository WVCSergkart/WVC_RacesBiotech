using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_SimplePsylink : Gene
	{

		// private bool pawnHadPsylinkBefore = false;

		public override void PostAdd()
		{
			base.PostAdd();
			GeneResourceUtility.AddPsylink(pawn);
		}

		public override void Tick()
		{
			//base.Tick();
			GeneResourceUtility.TryAddPsylinkRandomly(pawn);
		}

		//public void Notify_OverriddenBy(Gene overriddenBy)
		//{
		//	if (WVC_Biotech.settings.link_removePsylinkWithGene && !pawnHadPsylinkBefore)
		//	{
		//		HediffUtility.TryRemoveHediff(HediffDefOf.PsychicAmplifier, pawn);
		//	}
		//}

		//public void Notify_Override()
		//{
		//	if (WVC_Biotech.settings.link_removePsylinkWithGene && WVC_Biotech.settings.link_addedPsylinkWithGene)
		//	{
		//		if (!pawn.health.hediffSet.HasHediff(HediffDefOf.PsychicAmplifier))
		//		{
		//			pawn.health.AddHediff(HediffDefOf.PsychicAmplifier, pawn.health.hediffSet.GetBrain());
		//		}
		//	}
		//}

		//public override void PostRemove()
		//{
		//	base.PostRemove();
		//	if (WVC_Biotech.settings.link_removePsylinkWithGene && !pawnHadPsylinkBefore)
		//	{
		//		HediffUtility.TryRemoveHediff(HediffDefOf.PsychicAmplifier, pawn);
		//	}
		//}

		//public override void ExposeData()
		//{
		//	base.ExposeData();
		//	Scribe_Values.Look(ref pawnHadPsylinkBefore, "pawnHadPsylinkBefore", false);
		//}

	}

	public class Gene_Psylink : Gene_SimplePsylink
	{

		public GeneExtension_Giver Props => def?.GetModExtension<GeneExtension_Giver>();

		public float recoveryRate = 0.01f;

		public override void Tick()
		{
			base.Tick();
			GeneResourceUtility.PsyfocusOffset(pawn, this, ref recoveryRate, Props);
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref recoveryRate, "psyfocusRecoveryRate", 0);
		}

	}

	public class Gene_HemogenDrain_Psylink : Gene_HemogenOffset
	{

		public GeneExtension_Giver Props => def?.GetModExtension<GeneExtension_Giver>();

		public float recoveryRate = 0.01f;

		//private bool pawnHadPsylinkBefore = false;

		public override void PostAdd()
		{
			base.PostAdd();
			GeneResourceUtility.AddPsylink(pawn);
		}

		public override void Tick()
		{
			base.Tick();
			GeneResourceUtility.PsyfocusOffset(pawn, this, ref recoveryRate, Props);
			GeneResourceUtility.TryAddPsylinkRandomly(pawn);
		}

		//public void Notify_OverriddenBy(Gene overriddenBy)
		//{
		//	if (WVC_Biotech.settings.link_removePsylinkWithGene && !pawnHadPsylinkBefore)
		//	{
		//		HediffUtility.TryRemoveHediff(HediffDefOf.PsychicAmplifier, pawn);
		//	}
		//}

		//public void Notify_Override()
		//{
		//	if (WVC_Biotech.settings.link_removePsylinkWithGene && WVC_Biotech.settings.link_addedPsylinkWithGene)
		//	{
		//		if (!pawn.health.hediffSet.HasHediff(HediffDefOf.PsychicAmplifier))
		//		{
		//			pawn.health.AddHediff(HediffDefOf.PsychicAmplifier, pawn.health.hediffSet.GetBrain());
		//		}
		//	}
		//}

		//public override void PostRemove()
		//{
		//	base.PostRemove();
		//	if (WVC_Biotech.settings.link_removePsylinkWithGene && !pawnHadPsylinkBefore)
		//	{
		//		HediffUtility.TryRemoveHediff(HediffDefOf.PsychicAmplifier, pawn);
		//	}
		//}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref recoveryRate, "psyfocusRecoveryRate", 0);
			//Scribe_Values.Look(ref pawnHadPsylinkBefore, "pawnHadPsylinkBefore", false);
		}

	}

}
