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

    }

    public class Gene_MorpherTrigger : Gene_MorpherDependant
	{

		public virtual bool CanMorph()
		{
			return false;
		}

		private int nextTick = 0;
		private bool cachedBool = false;

		private bool CacheableBool(int ticksTumeOut = 120)
		{
			nextTick--;
			if (nextTick < 0)
			{
				cachedBool = !CanMorph();
				nextTick = ticksTumeOut;
			}
			return cachedBool;
		}

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
				Disabled = CacheableBool(),
				disabledReason = "WVC_XaG_GeneMorphAbilityDisabled".Translate(),
				action = delegate
				{
					FloatMenu();
				}
			};
		}

		private void FloatMenu()
		{
			if (Morpher == null)
			{
				//Log.Warning("Trying morph without morpher. Removing gene " + LabelCap + ".");
				//pawn.genes?.RemoveGene(this);
				Messages.Message("WVC_XaG_GeneAbilityMorpherIsNullMessage".Translate().CapitalizeFirst(), null, MessageTypeDefOf.RejectInput, historical: false);
				return;
            }
			List<FloatMenuOption> list = new();
			List<PawnGeneSetHolder> geneSets = Morpher.GeneSets;
			if (!geneSets.NullOrEmpty())
			{
				for (int i = 0; i < geneSets.Count; i++)
				{
					PawnGeneSetHolder geneSet = geneSets[i];
					list.Add(new FloatMenuOption(geneSet.LabelCap + " " + geneSet.formId.ToString(), delegate
					{
						MorpherTrigger(geneSet);
					}, orderInPriority: 0 - geneSet.formId));
				}
			}
			if (!list.Any() || Morpher.CurrentLimit > Morpher.FormsCount)
			{
				list.Add(new FloatMenuOption("WVC_XaG_GeneAbilityMorphCreateNewForm".Translate(), delegate
				{
					MorpherTrigger(null);
				}));
			}
			Find.WindowStack.Add(new FloatMenu(list));
		}

		public virtual void MorpherTrigger(PawnGeneSetHolder geneSet)
		{
			Dialog_MessageBox window = Dialog_MessageBox.CreateConfirmation("WVC_XaG_GeneAbilityMorphWarning".Translate(), delegate
			{
				try
				{
					Morpher?.TryMorph(geneSet, true);
				}
				catch (Exception arg)
				{
					Log.Error("Failed create form and morph. Reason: " + arg);
				}
			});
			Find.WindowStack.Add(window);
		}

	}

	//public class Gene_TickMorph : Gene_MorpherDependant
	//{

	//	private int nextTick = 30000;


	//	public override void PostAdd()
 //       {
 //           base.PostAdd();
 //           ResetInterval();
 //       }

	//	public override void Tick()
	//	{
	//		nextTick--;
	//		if (nextTick > 0)
	//		{
	//			return;
	//		}
	//		if (ShouldMorph())
 //           {
 //               MorpherTrigger();
 //           }
 //           ResetInterval();
	//	}

	//	private void ResetInterval()
	//	{
	//		nextTick = new IntRange(2500, 5000).RandomInRange;
	//	}

	//	public override void ExposeData()
	//	{
	//		base.ExposeData();
	//		Scribe_Values.Look(ref nextTick, "nextTick", 30000);
	//	}

	//}

	public class Gene_NocturnalMorph : Gene_MorpherTrigger
	{

        public override bool CanMorph()
        {
            float num = GenLocalDate.DayTick(pawn);
            if (num > 45000f || num < 15000f)
            {
                return true;
            }
            return false;
		}

	}

	public class Gene_DiurnalMorph : Gene_MorpherTrigger
	{

		public override bool CanMorph()
		{
			float num = GenLocalDate.DayTick(pawn);
			if (num < 40000f && num > 25000f)
			{
				return true;
			}
			return false;
		}

	}

	public class Gene_SeasonalMorph : Gene_MorpherTrigger
	{

		private Season savedSeason;

        //public override IntRange IntervalRange => new(50000, 80000);

        public override void PostAdd()
        {
            base.PostAdd();
			savedSeason = GenLocalDate.Season(pawn);
		}

        public override bool CanMorph()
		{
			if (GenLocalDate.Season(pawn) != savedSeason)
			{
				return true;
			}
			return false;
		}

		public override void MorpherTrigger(PawnGeneSetHolder geneSet)
		{
			savedSeason = GenLocalDate.Season(pawn);
			base.MorpherTrigger(geneSet);
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref savedSeason, "savedSeason");
		}

	}

	public class Gene_DamageMorph : Gene_MorpherTrigger
	{

		//public override IntRange IntervalRange => new(4000, 6000);

		public override bool CanMorph()
		{
            float? summaryHealthPercent = pawn?.health?.summaryHealth?.SummaryHealthPercent;
            if (summaryHealthPercent.HasValue && summaryHealthPercent.Value < 0.8f)
            {
                return true;
            }
            return false;
		}

	}

	public class Gene_AbilityMorph : Gene_MorpherTrigger
	{

		public override bool CanMorph()
		{
			return true;
		}

	}

	public class Gene_DeathrestMorph : Gene_MorpherTrigger
	{

		public override bool CanMorph()
		{
			Need_Deathrest deathrest = pawn.needs?.TryGetNeed<Need_Deathrest>();
			if (deathrest != null && deathrest.CurLevelPercentage > 0.8f)
			{
				return true;
			}
			return false;
		}

		public override void MorpherTrigger(PawnGeneSetHolder geneSet)
		{
			Need_Deathrest deathrest = pawn.needs?.TryGetNeed<Need_Deathrest>();
			if (deathrest != null)
			{
				deathrest.CurLevel = 0.1f;
			}
			base.MorpherTrigger(geneSet);
		}

	}

}
