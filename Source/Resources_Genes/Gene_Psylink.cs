using RimWorld;
using System;
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
			GeneResourceUtility.TryAddPsylinkRandomly(pawn, WVC_Biotech.settings.psylink_HediffFromGeneChance);
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
			PsyfocusOffset();
		}

		public void PsyfocusOffset()
		{
			if (!pawn.IsHashIntervalTick(750))
			{
				return;
			}
			pawn?.psychicEntropy?.OffsetPsyfocusDirectly(recoveryRate);
			if (!pawn.IsHashIntervalTick(7500))
			{
				return;
			}
			if (pawn.HasPsylink)
			{
				recoveryRate = GetRecoveryRate();
			}
		}

		private float GetRecoveryRate()
		{
			if (Props == null)
			{
				return 0.01f * pawn.GetPsylinkLevel();
			}
			return (float)Math.Round(Props.curve.Evaluate(pawn.GetPsylinkLevel()), 2);
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref recoveryRate, "psyfocusRecoveryRate", 0);
		}

	}

	public class Gene_HemogenDrain_Psylink : Gene_HemogenOffset, IGeneBloodfeeder
	{

		public GeneExtension_Giver Props => def?.GetModExtension<GeneExtension_Giver>();

		public float recoveryRate = 0.01f;
		private int nextTick = 300;
		private int nextSecondTick = 0;

		public override void PostAdd()
		{
			base.PostAdd();
			GeneResourceUtility.AddPsylink(pawn);
		}

		public override void Tick()
		{
			base.Tick();
			PsyfocusOffset();
			GeneResourceUtility.TryAddPsylinkRandomly(pawn, WVC_Biotech.settings.psylink_HediffFromGeneChance);
		}

		public void PsyfocusOffset()
		{
			if (!GeneResourceUtility.CanTick(ref nextTick))
			{
				return;
			}
			if (recoveryRate < 0f)
			{
				Hemogen.Value += 0.01f;
			}
			pawn?.psychicEntropy?.OffsetPsyfocusDirectly(recoveryRate);
			if (!GeneResourceUtility.CanTick(ref nextSecondTick, 2))
			{
				return;
			}
			recoveryRate = GetRecoveryRate();
		}

		private float GetRecoveryRate()
		{
			if (Hemogen == null)
			{
				return 0.01f * pawn.GetPsylinkLevel();
			}
			return (float)Math.Round(Props.curve.Evaluate(Hemogen.Value), 2);
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref recoveryRate, "psyfocusRecoveryRate", 0);
		}

        public void Notify_Bloodfeed(Pawn victim)
		{
			pawn?.psychicEntropy?.OffsetPsyfocusDirectly(0.2f);
		}

    }

}
