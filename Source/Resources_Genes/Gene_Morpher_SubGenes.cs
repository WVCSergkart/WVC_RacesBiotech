using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Gene_MorpherDependant : Gene
	{

		[Unsaved(false)]
		private Gene_Morpher cachedMorpherGene;

		public Gene_Morpher Morpher
		{
			get
			{
				if (cachedMorpherGene == null || !cachedMorpherGene.Active)
				{
					cachedMorpherGene = pawn?.genes?.GetFirstGeneOfType<Gene_Morpher>();
				}
				return cachedMorpherGene;
			}
		}
        //public Gene_Morpher Morpher => pawn?.genes?.GetFirstGeneOfType<Gene_Morpher>();

    }

	public class Gene_TickMorph : Gene_MorpherDependant
	{

		private int nextTick = 30000;

		//public virtual IntRange IntervalRange => new(1200, 3400);


		public override void PostAdd()
        {
            base.PostAdd();
            ResetInterval();
        }

		public override void Tick()
		{
			nextTick--;
			if (nextTick > 0)
			{
				return;
			}
			if (ShouldMorph())
            {
                MorpherTrigger();
            }
            ResetInterval();
		}

        private void MorpherTrigger()
        {
            try
            {
                Morpher?.TryMorph(true);
            }
            catch (Exception arg)
            {
                Log.Error("Failed trigger morph. Reason: " + arg);
            }
        }

        public virtual bool ShouldMorph()
		{
			return false;
		}

		private void ResetInterval()
		{
			nextTick = new IntRange(2500, 5000).RandomInRange;
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref nextTick, "nextTick", 30000);
		}

	}

	public class Gene_NocturnalMorph : Gene_TickMorph
	{

        public override bool ShouldMorph()
        {
            float num = GenLocalDate.DayTick(pawn);
            if (num > 45000f || num < 15000f)
            {
                return true;
            }
            return false;
		}

	}

	public class Gene_DiurnalMorph : Gene_TickMorph
	{

		public override bool ShouldMorph()
		{
			float num = GenLocalDate.DayTick(pawn);
			if (num < 40000f || num > 25000f)
			{
				return true;
			}
			return false;
		}

	}

	public class Gene_SeasonalMorph : Gene_TickMorph
	{

		private Season savedSeason;

		//public override IntRange IntervalRange => new(50000, 80000);

		public override bool ShouldMorph()
		{
			Season currentSeason = GenLocalDate.Season(pawn);
			if (currentSeason != savedSeason)
			{
				savedSeason = currentSeason;
				return true;
			}
			return false;
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref savedSeason, "savedSeason");
		}

	}

	public class Gene_DamageMorph : Gene_TickMorph
	{

		//public override IntRange IntervalRange => new(4000, 6000);

		public override bool ShouldMorph()
		{
            float? summaryHealthPercent = pawn?.health?.summaryHealth?.SummaryHealthPercent;
            if (summaryHealthPercent.HasValue && summaryHealthPercent.Value < 0.8f)
            {
                return true;
            }
            return false;
		}

	}

	public class Gene_AbilityMorph : Gene_MorpherDependant
	{

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (XaG_GeneUtility.SelectorActiveFactionMap(pawn, this))
			{
				yield break;
			}
			yield return new Command_Action
			{
				defaultLabel = "WVC_XaG_GeneAbilityMorphLabel".Translate(),
				defaultDesc = "WVC_XaG_GeneAbilityMorphDesc".Translate(),
				icon = ContentFinder<Texture2D>.Get(def.iconPath),
				action = delegate
				{
					Dialog_MessageBox window = Dialog_MessageBox.CreateConfirmation("WVC_XaG_GeneAbilityMorphWarning".Translate(), delegate
					{
						Morpher.TryMorph(true);
					});
					Find.WindowStack.Add(window);
				}
			};
		}

	}

}
