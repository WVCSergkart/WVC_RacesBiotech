using RimWorld;
using RimWorld.Planet;
using System.Linq;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class Gene_OverriderDependant : Gene_SelfOverrider
	{

		// public int CurrentGenes => pawn.genes.GenesListForReading.Where((gene) => gene.def.IsGeneDefOfType<Gene_MainframeDependant>()).ToList().Count;

		public override TaggedString RemoteActionDesc
		{
			get
			{
				string text = base.RemoteActionDesc;
				text += "\n\n" + "Complexity".Translate().Colorize(GeneUtility.GCXColor) + ": " + def.biostatCpx.ToStringWithSign();
				text += "\n" + "Metabolism".Translate().CapitalizeFirst().Colorize(GeneUtility.METColor) + ": " + def.biostatMet.ToStringWithSign();
				text += "\n" + "ArchitesRequired".Translate().Colorize(GeneUtility.ARCColor) + ": " + def.biostatArc.ToStringWithSign();
				return text;
			}
		}

		[Unsaved(false)]
		private Gene_Overrider cachedGene;

		public Gene_Overrider Energy
		{
			get
			{
				if (cachedGene == null || !cachedGene.Active)
				{
					cachedGene = pawn?.genes?.GetFirstGeneOfType<Gene_Overrider>();
				}
				return cachedGene;
			}
		}

		public override void RemoteControl_Action(Dialog_GenesSettings genesSettings)
		{
			base.RemoteControl_Action(genesSettings);
			Notify_Overrider();
		}

		public override void ResetCooldown()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				return;
			}
			lastTick = Find.TickManager.TicksGame + 30000;
		}

        public override void Notify_OverriddenBy(Gene overriddenBy)
        {
            base.Notify_OverriddenBy(overriddenBy);
            Notify_Overrider();
        }

        public override void Notify_Override()
        {
            base.Notify_Override();
            Notify_Overrider();
        }

        public override void PostAdd()
        {
            base.PostAdd();
            Notify_Overrider();
        }

        public override void PostRemove()
        {
            base.PostRemove();
            Notify_Overrider();
        }

        public void Notify_Overrider()
		{
			Energy?.Notify_HediffReset();
		}

    }

	public class Gene_SelfOverrider_Deathrest : Gene_OverriderDependant
	{

		private int deathrestCapacity = 0;
		private float deathrestNeed = 1f;
		private bool deathrestAdded = false;
		private bool firtsAdded = false;

		public override void RemoteControl_Action(Dialog_GenesSettings genesSettings)
		{
			firtsAdded = false;
			base.RemoteControl_Action(genesSettings);
		}

		public void Notify_GeneDeathrest()
		{
			//if (!deathrestAdded)
			//{
			//	return;
			//}
			foreach (Gene gene in pawn.genes.GenesListForReading)
			{
				if (gene is Gene_Deathrest deathrest)
				{
					//Log.Error("Saved capacity: " + deathrestCapacity);
					//Log.Error("Target capacity: " + (deathrestCapacity - deathrest.CurrentCapacity));
					deathrest.OffsetCapacity(Mathf.Clamp(deathrestCapacity - deathrest.DeathrestCapacity, 0, 999), false);
					Need_Deathrest deathrestNeed1 = deathrest.DeathrestNeed;
					if (deathrestNeed1 != null)
					{
						deathrestNeed1.CurLevel = deathrestNeed;
					}
				}
			}
		}

		private bool? cachedIsXenogene;
		public bool IsXenogene
		{
			get
			{
				if (!cachedIsXenogene.HasValue)
				{
					cachedIsXenogene = pawn.genes.IsXenogene(this);
				}
				return cachedIsXenogene.Value;
			}
		}

		//private Gene_Deathrest gene_Deathrest;

		public void UpdGeneDeathrest()
		{
			Gene_Deathrest gene_Deathrest = pawn.genes.GetFirstGeneOfType<Gene_Deathrest>();
			if (Overridden)
			{
				if (gene_Deathrest != null)
				{
					//Log.Error("Capacity: " + gene_Deathrest.DeathrestCapacity);
					deathrestCapacity = gene_Deathrest.DeathrestCapacity;
					deathrestNeed = gene_Deathrest.DeathrestNeed.CurLevel;
					//Log.Error("Saved capacity: " + deathrestCapacity);
				}
				RemoveDeathrestGene();
			}
			else
			{
				if (gene_Deathrest == null)
				{
					pawn.genes.AddGene(MainDefOf.Deathrest, IsXenogene);
					deathrestAdded = true;
				}
				Notify_GeneDeathrest();
			}
		}


		public override void PostMake()
		{
			base.PostMake();
			firtsAdded = true;
		}

		public override void PostAdd()
		{
			base.PostAdd();
			if (!firtsAdded)
			{
				UpdGeneDeathrest();
			}
			else
			{
				overrided = true;
				OverrideBy(this);
			}
		}

		public override void Notify_OverriddenBy(Gene overriddenBy)
		{
			base.Notify_OverriddenBy(overriddenBy);
			if (!firtsAdded)
			{
				UpdGeneDeathrest();
			}
		}

		public override void Notify_Override()
		{
			base.Notify_Override();
			if (firtsAdded)
			{
				OverrideBy(this);
			}
			else if (!overrided)
			{
				UpdGeneDeathrest();
			}
		}

		public override void PostRemove()
		{
			base.PostRemove();
			if (!firtsAdded)
			{
				RemoveDeathrestGene();
			}
		}

		private void RemoveDeathrestGene()
		{
			if (!deathrestAdded)
			{
				return;
			}
			foreach (Gene gene in pawn.genes.GenesListForReading.ToList())
			{
				if (gene is Gene_Deathrest deathrest)
				{
					pawn.genes.RemoveGene(deathrest);
				}
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref deathrestCapacity, "deathrestCapacity", defaultValue: 0);
			Scribe_Values.Look(ref deathrestAdded, "deathrestAdded", defaultValue: false);
			Scribe_Values.Look(ref deathrestNeed, "deathrestNeed", defaultValue: 1);
		}

	}

	public class Gene_SelfOverrider_Healing : Gene_OverriderDependant
	{

		public GeneExtension_Undead Undead => def.GetModExtension<GeneExtension_Undead>();

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

		public override void TickInterval(int delta)
		{
			base.TickInterval(delta);
			if (!pawn.IsHashIntervalTick(713, delta))
			{
				return;
			}
			HealingUtility.Regeneration(pawn, regeneration: Undead.regeneration, tick: 713, regenEyes: RegenerateEyes);
		}

	}

	public class Gene_SelfOverrider_Stomach : Gene_OverriderDependant, IGeneNotifyGenesChanged
	{

		private float? cachedOffset;
		public float Offset
		{
			get
			{
				if (!cachedOffset.HasValue)
				{
					cachedOffset = 1f / 60000 * 6514;
				}
				return cachedOffset.Value;
			}
		}

		public void Notify_GenesChanged(Gene changedGene)
		{
			cachedOffset = null;
		}

		public override void TickInterval(int delta)
		{
			base.TickInterval(delta);
			if (!pawn.IsHashIntervalTick(6514, delta))
			{
				return;
			}
			if (pawn.Faction != Faction.OfPlayer)
			{
				return;
			}
			if (GeneResourceUtility.DownedSleepOrInBed(pawn))
			{
				return;
			}
			GeneResourceUtility.OffsetNeedFood(pawn, -1 * Offset);
		}

		public override void Notify_IngestedThing(Thing thing, int numTaken)
		{
			if (!Active)
			{
				return;
			}
			MiscUtility.TryAddFoodPoisoningHediff(pawn, thing);
		}

	}

	public class Gene_SelfOverrider_Learning : Gene_OverriderDependant
	{

		public override void PostAdd()
		{
			base.PostAdd();
			AddTraits();
		}

		public override void TickInterval(int delta)
		{
			base.TickInterval(delta);
			if (!pawn.IsHashIntervalTick(37194, delta))
			{
				return;
			}
			AutoLearning();
		}

		private void AutoLearning()
		{
			foreach (SkillRecord skillRecord in pawn.skills.skills)
			{
				if (!skillRecord.TotallyDisabled)
				{
					skillRecord.Learn(skillRecord.XpRequiredForLevelUp * (1f / (60000 * 12) * 37194), false, true);
				}
			}
		}

		public override void Notify_OverriddenBy(Gene overriddenBy)
		{
			base.Notify_OverriddenBy(overriddenBy);
			GeneTraitUpd();
		}

		public override void Notify_Override()
		{
			base.Notify_Override();
			GeneTraitUpd();
		}

		public override void PostRemove()
		{
			base.PostRemove();
			RemoveTraits();
		}

		public void GeneTraitUpd()
		{
			if (Active)
			{
				AddTraits();
			}
			else
			{
				RemoveTraits();
			}
		}

		private void AddTraits()
		{
			TraitsUtility.AddGeneTraits(pawn, this);
		}

		private void RemoveTraits()
		{
			TraitsUtility.RemoveGeneTraits(pawn, this);
		}

	}

	public class Gene_SelfOverrider_Ageless : Gene_OverriderDependant
	{

		public override void PostAdd()
		{
			base.PostAdd();
			AgelessUtility.InitialRejuvenation(pawn);
		}

		public void AgeRevers()
		{
			AgelessUtility.TryAgeReverse(pawn);
		}

		public override void TickInterval(int delta)
        {
            base.TickInterval(delta);
            if (!pawn.IsHashIntervalTick(59001, delta))
            {
                return;
            }
            AgeRevers();
        }

	}

	public class Gene_SelfOverrider_NoLearning : Gene_OverriderDependant
	{

		public override void TickInterval(int delta)
        {
            base.TickInterval(delta);
            if (!pawn.IsHashIntervalTick(11345, delta))
            {
                return;
            }
            Recreation();
        }

		public void Recreation()
		{
			Need_Joy recreation = pawn.needs.joy;
			if (recreation != null)
			{
				recreation.CurLevelPercentage += 0.33f / 60000 * 11345;
			}
		}

	}

	public class Gene_SelfOverrider_Solar : Gene_OverriderDependant, IGeneNotifyGenesChanged
	{

		public int basicTick = 7119;

		public override void TickInterval(int delta)
		{
			base.TickInterval(delta);
			if (!pawn.IsHashIntervalTick(basicTick, delta))
			{
				return;
			}
			Charge();
		}

		private float? cachedNutritionPerTick;

		private float Nutrition
		{
			get
			{
				if (!cachedNutritionPerTick.HasValue)
				{
					cachedNutritionPerTick = 0.8f / 60000 * basicTick;
				}
				return cachedNutritionPerTick.Value;
			}
		}

		public void Charge()
		{
			SolarEating();
			Gene_Rechargeable.NotifySubGenes_Charging(pawn, Nutrition, basicTick, 0.5f);
		}

		private void SolarEating()
		{
			if (pawn.Map == null)
			{
				InCaravan();
				return;
			}
			if (!pawn.Position.InSunlight(pawn.Map))
			{
				return;
			}
			ReplenishHunger();
		}

		private void InCaravan()
		{
			Caravan caravan = pawn.GetCaravan();
			if (caravan?.NightResting != false)
			{
				return;
			}
			ReplenishHunger();
		}

		public void ReplenishHunger()
        {
            GeneResourceUtility.OffsetNeedFood(pawn, Nutrition);
		}

        public void Notify_GenesChanged(Gene changedGene)
        {
			cachedNutritionPerTick = null;
		}

    }

}
