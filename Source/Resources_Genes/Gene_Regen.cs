using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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

	public class Gene_SelfRepair : Gene_MachineWoundHealing
	{

		public override void Tick()
		{
			if (!pawn.IsHashIntervalTick(579))
			{
				return;
			}
			if (pawn.Downed || pawn.Drafted || !pawn.Awake())
			{
				return;
			}
			HealingUtility.Regeneration(pawn, Undead.regeneration, WVC_Biotech.settings.totalHealingIgnoreScarification, 579);
		}

	}

	// Health
	public class Gene_HealingStomach : Gene
	{

		public override void Tick()
		{
			//base.Tick();
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

}
