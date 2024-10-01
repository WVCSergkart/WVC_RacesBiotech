using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Hediff_Undead : HediffWithComps
	{
		private static readonly FloatRange ResurrectionSecondsRange = new FloatRange(5f, 10f);

		private static readonly FloatRange StunSecondsRange = new FloatRange(1f, 3f);

		private static readonly FloatRange AlertSecondsRange = new FloatRange(0.5f, 3f);

		private static readonly IntRange SelfRaiseHoursRange = new IntRange(3, 4);

		private const float ParticleSpawnMTBSeconds = 1f;

		private static readonly IntRange CheckForTargetTicksInterval = new IntRange(900, 1800);

		private const float ExtinguishFireMTB = 45f;

		private const float BioferriteOnDeathChance = 0.04f;

		private const int BioferriteAmountOnDeath = 10;

		public float headRotation;

		private float resurrectTimer;

		private float selfRaiseTimer;

		private float alertTimer;

		private int nextTargetCheckTick = -99999;

		private Thing alertedTarget;

		private Effecter riseEffecter;

		private Sustainer riseSustainer;

		private float corpseDamagePct = 1f;

		// public bool IsRising => pawn.health.hediffSet.HasHediff(HediffDefOf.Rising);

		public bool IsRising => true;

		public override void Tick()
		{
			base.Tick();
			if (IsRising)
			{
				if ((float)Find.TickManager.TicksGame > resurrectTimer)
				{
					// FinishRising();
				}
				if (!pawn.Spawned)
				{
					return;
				}
				if ((float)Find.TickManager.TicksGame > resurrectTimer - 15f)
				{
					riseSustainer?.End();
				}
				else if (IsRising)
				{
					if (riseSustainer == null || riseSustainer.Ended)
					{
						SoundInfo info = SoundInfo.InMap(pawn, MaintenanceType.PerTick);
						riseSustainer = SoundDefOf.Pawn_Shambler_Rise.TrySpawnSustainer(info);
					}
					if (riseEffecter == null)
					{
						riseEffecter = EffecterDefOf.ShamblerRaise.Spawn(pawn, pawn.Map);
					}
					if (pawn.Drawer.renderer.CurAnimation != AnimationDefOf.ShamblerRise)
					{
						pawn.Drawer.renderer.SetAnimation(AnimationDefOf.ShamblerRise);
					}
					riseSustainer.Maintain();
					riseEffecter.EffectTick(pawn, TargetInfo.Invalid);
				}
				Log.Error("Hediff tick");
				return;
			}
		}

		// public override void ExposeData()
		// {
			// base.ExposeData();
			// Scribe_Values.Look(ref alertTimer, "alertTimer", 0f);
			// Scribe_Values.Look(ref nextTargetCheckTick, "nextTargetCheckTick", 0);
			// Scribe_References.Look(ref alertedTarget, "alertedTarget");
			// Scribe_Values.Look(ref resurrectTimer, "resurrectTimer", 0f);
			// Scribe_Values.Look(ref selfRaiseTimer, "selfRaiseTimer", 0f);
			// Scribe_Values.Look(ref headRotation, "headRotation", 0f);
		// }
	}

}
