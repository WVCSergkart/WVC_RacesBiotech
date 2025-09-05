using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{


	public class Gene_DeathrestDependant : Gene
	{

		public GeneExtension_Undead Undead => def?.GetModExtension<GeneExtension_Undead>();

		[Unsaved(false)]
		private Gene_Deathrest cachedDeathrestGene;

		public Gene_Deathrest Deathrest
		{
			get
			{
				if (cachedDeathrestGene == null || !cachedDeathrestGene.Active)
				{
					cachedDeathrestGene = pawn?.genes?.GetFirstGeneOfType<Gene_Deathrest>();
				}
				return cachedDeathrestGene;
			}
		}

		[Unsaved(false)]
		private Gene_Hemogen cachedHemogenGene;

		public Gene_Hemogen Hemogen
		{
			get
			{
				if (cachedHemogenGene == null || !cachedHemogenGene.Active)
				{
					cachedHemogenGene = pawn?.genes?.GetFirstGeneOfType<Gene_Hemogen>();
				}
				return cachedHemogenGene;
			}
		}

		//private int nextTick = 2500;

		public override void TickInterval(int delta)
		{
			if (!pawn.IsHashIntervalTick(6000, delta))
			{
				return;
			}
			if (!pawn.Deathresting)
			{
				return;
			}
			DoTick(6000, delta);
			//nextTick = new IntRange(3000, 15000).RandomInRange;
		}

		public virtual void DoTick(int tick, int delta)
		{
		}

	}


	public class Gene_Deathrest_Immunization : Gene_DeathrestDependant
	{

		public override void DoTick(int tick, int delta)
		{
			HealingUtility.Immunization(pawn, immunization: Undead.immunization, tick: tick);
		}

	}


	public class Gene_Deathrest_Healing : Gene_DeathrestDependant
	{

		//private bool? regenerateEyes;
		//public bool RegenerateEyes
		//{
		//	get
		//	{
		//		if (!regenerateEyes.HasValue)
		//		{
		//			regenerateEyes = HealingUtility.ShouldRegenerateEyes(pawn);
		//		}
		//		return regenerateEyes.Value;
		//	}
		//}

		public override void DoTick(int tick, int delta)
		{
			HealingUtility.Regeneration(pawn, regeneration: Undead.regeneration, tick: tick);
		}

	}


	public class Gene_Deathrest_HemogenGain : Gene_DeathrestDependant
	{

		public override void DoTick(int tick, int delta)
		{
			if (Hemogen != null)
			{
				GeneResourceDrainUtility.OffsetResource(Hemogen, ((0f - def.resourceLossPerDay) * (float)delta / 60000f) * tick);
			}
		}

	}


	public class Gene_Deathrest_FastRest : Gene_DeathrestDependant
	{

		public override void DoTick(int tick, int delta)
		{
			//if (pawn.health.hediffSet.TryGetHediff<Hediff_Deathrest>(out Hediff_Deathrest hediff_Deathrest))
			//{
			//	hediff_Deathrest.
			//}
			//Need_Deathrest deathrest = pawn.needs?.TryGetNeed<Need_Deathrest>();
			//if (deathrest != null)
			//{
			//	Deathrest.deathrestTicks += restBoost;
			//	deathrest.CurLevel += ((Undead.deathrestBoost * 0.01f) / 60000) * tick;
			//}
			//Log.Error(((int)(0.1f * tick)).ToString());
			if (Deathrest.DeathrestPercent >= 1f)
            {
				return;
            }
			Deathrest.deathrestTicks += Mathf.RoundToInt(12000f * (tick * delta / 6000));
		}

	}

}
