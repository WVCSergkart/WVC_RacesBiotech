using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Psylink : Gene
	{

		public float recoveryRate = 0.01f;

		public override void PostAdd()
		{
			base.PostAdd();
			if (!pawn.health.hediffSet.HasHediff(HediffDefOf.PsychicAmplifier))
			{
				pawn.health.AddHediff(HediffDefOf.PsychicAmplifier, pawn.health.hediffSet.GetBrain());
			}
		}

		public override void Tick()
		{
			base.Tick();
			if (!pawn.IsHashIntervalTick(150))
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
				recoveryRate = 0.01f * pawn.GetPsylinkLevel();
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

	public class Gene_HemogenDrain_Psylink : Gene_HemogenDrain
	{

		public float recoveryRate = 0.01f;

		public override void PostAdd()
		{
			base.PostAdd();
			if (!pawn.health.hediffSet.HasHediff(HediffDefOf.PsychicAmplifier))
			{
				pawn.health.AddHediff(HediffDefOf.PsychicAmplifier, pawn.health.hediffSet.GetBrain());
			}
		}

		public override void Tick()
		{
			base.Tick();
			if (!pawn.IsHashIntervalTick(150))
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
				recoveryRate = 0.01f * pawn.GetPsylinkLevel();
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
