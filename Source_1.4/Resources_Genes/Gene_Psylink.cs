using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Psylink : Gene
	{

		public GeneExtension_Giver Props => def?.GetModExtension<GeneExtension_Giver>();

		public float recoveryRate = 0.01f;

		public override void PostAdd()
		{
			base.PostAdd();
			if (!pawn.health.hediffSet.HasHediff(HediffDefOf.PsychicAmplifier))
			{
				pawn.health.AddHediff(HediffDefOf.PsychicAmplifier, pawn.health.hediffSet.GetBrain());
			}
			ChangePsylinkLevel(pawn);
		}

		public static void ChangePsylinkLevel(Pawn pawn)
		{
			if (pawn.Map != null)
			{
				return;
			}
			Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.PsychicAmplifier);
			if (Rand.Chance(0.34f) && firstHediffOfDef != null)
			{
				IntRange level = new(1, 5);
				((Hediff_Level)firstHediffOfDef).ChangeLevel(level.RandomInRange);
			}
		}

		public override void Tick()
		{
			base.Tick();
			if (!pawn.IsHashIntervalTick(750))
			{
				return;
			}
			if (!Active)
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
				recoveryRate = GetRecoveryRate(pawn, Props);
			}
		}

		public static float GetRecoveryRate(Pawn pawn, GeneExtension_Giver giver)
		{
			if (giver == null)
			{
				return 0.01f * pawn.GetPsylinkLevel();
			}
			return giver.curve.Evaluate(pawn.GetPsylinkLevel());
		}

		public override void Reset()
		{
			base.Reset();
			if (!pawn.health.hediffSet.HasHediff(HediffDefOf.PsychicAmplifier))
			{
				pawn.health.AddHediff(HediffDefOf.PsychicAmplifier, pawn.health.hediffSet.GetBrain());
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref recoveryRate, "psyfocusRecoveryRate", 0);
		}

	}

	public class Gene_HemogenDrain_Psylink : Gene_HemogenDrain
	{

		public GeneExtension_Giver Props => def?.GetModExtension<GeneExtension_Giver>();

		public float recoveryRate = 0.01f;

		public override void PostAdd()
		{
			base.PostAdd();
			if (!pawn.health.hediffSet.HasHediff(HediffDefOf.PsychicAmplifier))
			{
				pawn.health.AddHediff(HediffDefOf.PsychicAmplifier, pawn.health.hediffSet.GetBrain());
			}
			Gene_Psylink.ChangePsylinkLevel(pawn);
		}

		public override void Tick()
		{
			base.Tick();
			if (!pawn.IsHashIntervalTick(750))
			{
				return;
			}
			if (!Active)
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
				recoveryRate = Gene_Psylink.GetRecoveryRate(pawn, Props);
			}
		}

		public override void Reset()
		{
			base.Reset();
			if (!pawn.health.hediffSet.HasHediff(HediffDefOf.PsychicAmplifier))
			{
				pawn.health.AddHediff(HediffDefOf.PsychicAmplifier, pawn.health.hediffSet.GetBrain());
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref recoveryRate, "psyfocusRecoveryRate", 0);
		}

	}

}
