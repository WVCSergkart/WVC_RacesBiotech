using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_MachineWoundHealing : Gene_OverOverridable
	{

		public GeneExtension_Undead Undead => def.GetModExtension<GeneExtension_Undead>();

		public override void Tick()
		{
			// base.Tick();
			if (!pawn.IsHashIntervalTick(676))
			{
				return;
			}
			HealingUtility.Regeneration(pawn, Undead.regeneration, WVC_Biotech.settings.totalHealingIgnoreScarification, 676);
		}

	}

	[Obsolete]
	public class Gene_Regeneration : Gene_MachineWoundHealing
	{


	}

	// Health
	public class Gene_HealingStomach : Gene
	{

		public override void Tick()
		{
			base.Tick();
			if (!pawn.IsHashIntervalTick(2317))
			{
				return;
			}
			EatWounds();
		}

		public void EatWounds()
		{
			List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
			float eatedDamage = 0f;
			foreach (Hediff hediff in hediffs.ToList())
			{
				if (hediff is not Hediff_Injury injury)
				{
					continue;
				}
				if (hediff.def == HediffDefOf.Scarification && WVC_Biotech.settings.totalHealingIgnoreScarification)
				{
					continue;
				}
				eatedDamage += 0.005f;
				injury.Heal(0.5f);
			}
			GeneResourceUtility.OffsetNeedFood(pawn, eatedDamage);
		}

	}

	public class Gene_FleshmassNucleus : Gene_OverOverridable
	{

		public GeneExtension_Undead Undead => def.GetModExtension<GeneExtension_Undead>();

		private int nextTick = 0;

		public override void PostAdd()
		{
			base.PostAdd();
			nextTick = 150000;
		}

		public override void Tick()
		{
			// base.Tick();
			if (pawn.IsHashIntervalTick(600))
			{
				HealingUtility.Regeneration(pawn, Undead.regeneration, WVC_Biotech.settings.totalHealingIgnoreScarification, 600);
			}
			//new IntRange(60000, 150000).RandomInRange
			if (GeneResourceUtility.CanTick(ref nextTick, 150000))
            {
                TryGiveMutation();
            }
        }

        private void TryGiveMutation()
        {
            if (Gene_MorphMutations.TryGetBestMutation(pawn, out HediffDef mutation))
            {
                if (!HediffUtility.TryGiveFleshmassMutation(pawn, mutation) && TryGetPawnMutation(out HediffAddedPart_FleshmassNucleus hediffWithComps_FleshmassHeart))
                {
                    hediffWithComps_FleshmassHeart.LevelUp();
                }
            }
        }

        private bool TryGetPawnMutation(out HediffAddedPart_FleshmassNucleus hediffWithComps_FleshmassHeart)
        {
			hediffWithComps_FleshmassHeart = null;
			foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
            {
				if (hediff is HediffAddedPart_FleshmassNucleus massHediff && massHediff.CanLevelUp)
                {
					hediffWithComps_FleshmassHeart = massHediff;
					break;
				}
            }
            return hediffWithComps_FleshmassHeart != null;
        }

        public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
		{
			if (!Active)
			{
				return;
			}
			if (ModsConfig.AnomalyActive && pawn.Corpse?.Map != null)
			{
				if (GenDrop.TryDropSpawn(PawnGenerator.GeneratePawn(new PawnGenerationRequest(PawnKindDefOf.FleshmassNucleus, Faction.OfEntities)), pawn.Corpse.PositionHeld, pawn.Corpse.MapHeld, ThingPlaceMode.Near, out var resultingThing))
                {
                    CompActivity activity = resultingThing.TryGetComp<CompActivity>();
					if (activity != null)
                    {
						activity.AdjustActivity(1f);
					}
					//if (!pawn.Corpse.Destroyed)
					//{
					//	EffecterDefOf.MeatExplosion.Spawn(pawn.Corpse.PositionHeld, pawn.Corpse.MapHeld).Cleanup();
					//	pawn.Corpse.Destroy();
					//}
				}
			}
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: FleshmassMutation",
					action = delegate
					{
						TryGiveMutation();
					}
				};
			}
		}

	}

}
