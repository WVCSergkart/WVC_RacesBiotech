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

		public override void Tick()
		{
			if (!pawn.IsHashIntervalTick(6000))
			{
				return;
			}
			if (!pawn.Deathresting)
			{
				return;
			}
			DoTick(6000);
			//nextTick = new IntRange(3000, 15000).RandomInRange;
		}

		public virtual void DoTick(int tick)
		{
		}

	}


	public class Gene_Deathrest_Immunization : Gene_DeathrestDependant
	{

		public override void DoTick(int tick)
		{
			HealingUtility.Immunization(pawn, Undead.immunization, tick);
		}

	}


	public class Gene_Deathrest_Healing : Gene_DeathrestDependant
	{

		private bool? regenerateEyes;
		public bool RegenerateEyes
		{
			get
			{
				if (!regenerateEyes.HasValue)
				{
					regenerateEyes = HealingUtility.ShouldRegenerateEyes(pawn);
				}
				return regenerateEyes.Value;
			}
		}

		public override void DoTick(int tick)
		{
			HealingUtility.Regeneration(pawn, Undead.regeneration, WVC_Biotech.settings.totalHealingIgnoreScarification, tick, RegenerateEyes);
		}

	}


	public class Gene_Deathrest_HemogenGain : Gene_DeathrestDependant
	{

		public override void DoTick(int tick)
		{
			if (Hemogen != null)
			{
				GeneResourceDrainUtility.OffsetResource(Hemogen, ((0f - def.resourceLossPerDay) / 60000f) * tick);
			}
		}

	}


	public class Gene_Deathrest_FastRest : Gene_DeathrestDependant
	{

		public override void DoTick(int tick)
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
			Deathrest.deathrestTicks += Mathf.RoundToInt(12000f * (tick / 6000));
		}

	}

}
