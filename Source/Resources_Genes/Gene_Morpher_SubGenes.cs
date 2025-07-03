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

		// Currently unused
		//public virtual void PreMorph(Pawn pawn)
		//{

		//}

		public virtual void PostMorph(Pawn pawn)
		{

		}

        public override void TickInterval(int delta)
        {

        }

    }

	// In Dev
	public class Gene_MorpherOneTimeUse : Gene_MorpherDependant
	{

		//public virtual void AfterUseMessage()
		//{
		//	Messages.Message("WVC_XaG_GeneAbilityMorpherIsNullMessage".Translate().CapitalizeFirst(), null, MessageTypeDefOf.RejectInput, historical: false);
		//}

	}

    public class Gene_MorphMutations : Gene_MorpherDependant
	{

		public override void PostMorph(Pawn pawn)
		{
			if (!ModLister.CheckAnomaly("FleshbeastMutations"))
			{
				return;
			}
			if (TryGetBestMutation(pawn, out HediffDef mutation))
			{
				FleshbeastUtility.TryGiveMutation(pawn, mutation);
			}
		}

		public static bool TryGetBestMutation(Pawn pawn, out HediffDef mutation)
		{
			mutation = null;
			if (DefDatabase<HediffDef>.AllDefsListForReading.Where((HediffDef hediffDef) => hediffDef.defaultInstallPart != null && hediffDef.CompProps<HediffCompProperties_FleshbeastEmerge>() != null && hediffDef.IsHediffDefOfType<Hediff_AddedPart>()).TryRandomElementByWeight((HediffDef hediffDef) => pawn.health.hediffSet.HasHediff(hediffDef) ? 1f : 100f, out HediffDef mutationHediff))
            {
				mutation = mutationHediff;
			}
			return mutation != null;
		}

	}

    public class Gene_MorpherTrigger : Gene_MorpherDependant
	{

		//public bool OneTimeUse
		//{
		//	get
		//	{
		//		return pawn?.genes?.GetFirstGeneOfType<Gene_MorpherOneTimeUse>() != null;
		//	}
		//}

		public virtual bool CanMorph()
		{
			return false;
		}

		private int nextTick = 0;
		private bool cachedBool = false;

		private bool CacheableBool(int ticksTimeOut = 120)
		{
			nextTick--;
			if (nextTick < 0)
			{
				cachedBool = !CanMorph();
				nextTick = ticksTimeOut;
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
				Order = -95,
				disabledReason = "WVC_XaG_GeneMorphAbilityDisabled".Translate(),
				action = delegate
				{
					FloatMenu();
				}
			};
		}

		private void FloatMenu()
		{
			if (!GeneResourceUtility.CanDo_GeneralGeneticStuff(pawn))
			{
				return;
			}
			if (Morpher == null)
			{
				//Log.Warning("Trying morph without morpher. Removing gene " + LabelCap + ".");
				//pawn.genes?.RemoveGene(this);
				Messages.Message("WVC_XaG_GeneAbilityMorpherIsNullMessage".Translate().CapitalizeFirst(), null, MessageTypeDefOf.RejectInput, historical: false);
				return;
			}
			if (!CanMorph() || !Morpher.CanMorphNow)
			{
				Messages.Message("WVC_XaG_GeneMorpherCannotMorphNow".Translate().CapitalizeFirst(), null, MessageTypeDefOf.RejectInput, historical: false);
				cachedBool = true;
				return;
			}
			List<FloatMenuOption> list = new();
			List<PawnGeneSetHolder> geneSets = Morpher.SavedGeneSets;
			if (!geneSets.NullOrEmpty())
			{
				for (int i = 0; i < geneSets.Count; i++)
				{
					PawnGeneSetHolder geneSet = geneSets[i];
					list.Add(new FloatMenuOption(geneSet.LabelCap + " " + geneSet.formId.ToString(), delegate
					{
						MorpherWarining(geneSet);
					}, orderInPriority: 0 - geneSet.formId));
				}
			}
			if (!list.Any() || Morpher.CanAddNewForm)
			{
				list.Add(new FloatMenuOption("WVC_XaG_GeneAbilityMorphCreateNewForm".Translate(), delegate
				{
					MorpherWarining(null);
				}));
			}
			Find.WindowStack.Add(new FloatMenu(list));
		}

		public virtual void MorpherWarining(PawnGeneSetHolder geneSet)
		{
			Dialog_MessageBox window = Dialog_MessageBox.CreateConfirmation(Morpher.WarningDesc, delegate
			{
				MorpherTrigger(geneSet);
			});
			Find.WindowStack.Add(window);
		}

		public virtual void MorpherTrigger(PawnGeneSetHolder geneSet)
		{
			try
			{
				Morpher?.TryMorph(geneSet, true, Morpher.IsOneTime);
			}
			catch (Exception arg)
			{
				Log.Error("Failed create form and morph. Reason: " + arg);
			}
		}

	}

	//public class Gene_TickMorph : Gene_MorpherDependant
	//{

	//	private int nextTick = 30000;


	//	public override void PostAdd()
 //		  {
 //			  base.PostAdd();
 //			  ResetInterval();
 //		  }

	//	public override void Tick()
	//	{
	//		nextTick--;
	//		if (nextTick > 0)
	//		{
	//			return;
	//		}
	//		if (ShouldMorph())
 //			  {
 //				  MorpherTrigger();
 //			  }
 //			  ResetInterval();
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

	public class Gene_DeathlessMorph : Gene_MorpherTrigger
	{

		public override bool CanMorph()
		{
			return SanguophageUtility.ShouldBeDeathrestingOrInComaInsteadOfDead(pawn);
		}

		public override void MorpherTrigger(PawnGeneSetHolder geneSet)
		{
			Gene_PostShapeshift_Regeneration.RemoveAllRemovableBadHediffs(pawn);
			base.MorpherTrigger(geneSet);
		}

	}

	public class Gene_UndeadMorph : Gene_MorpherTrigger
	{

		//private bool shouldMorph = true;

		//public override bool CanMorph()
		//{
		//	return shouldMorph && Active && Morpher != null;
		//}

		//public bool TryMorphWithChance(PawnGeneSetHolder geneSet, float chance = 0.2f)
		//{
		//	if (Rand.Chance(chance) && CanMorph())
		//	{
		//		MorpherTrigger(geneSet);
		//		return true;
		//	}
		//	return false;
		//}

		//public override void MorpherTrigger(PawnGeneSetHolder geneSet)
		//{
		//	try
		//	{
		//		Morpher?.TryMorph(geneSet, true, OneTimeUse);
		//	}
		//	catch (Exception arg)
		//	{
		//		Log.Error("Failed create form and morph. Reason: " + arg);
		//	}
		//}

		//public override IEnumerable<Gizmo> GetGizmos()
		//{
		//	if (XaG_GeneUtility.SelectorDraftedActiveFactionMap(pawn, this))
		//	{
		//		yield break;
		//	}
		//	yield return new Command_Action
		//	{
		//		defaultLabel = "WVC_XaG_GeneAbilityMorphLabel".Translate() + ": " + XaG_UiUtility.OnOrOff(shouldMorph),
		//		defaultDesc = "WVC_XaG_GeneAbilityMorphUndeadResurrectionDesc".Translate(),
		//		icon = ContentFinder<Texture2D>.Get(def.iconPath),
		//		action = delegate
		//		{
		//			shouldMorph = !shouldMorph;
		//			if (shouldMorph)
		//			{
		//				SoundDefOf.Tick_High.PlayOneShotOnCamera();
		//			}
		//			else
		//			{
		//				SoundDefOf.Tick_Low.PlayOneShotOnCamera();
		//			}
		//		}
		//	};
		//}

		//public override void ExposeData()
		//{
		//	base.ExposeData();
		//	Scribe_Values.Look(ref shouldMorph, "shouldMorph", false);
		//}

		public override bool CanMorph()
		{
			return pawn.health?.hediffSet?.HasHediff(HediffDefOf.ResurrectionSickness) == true;
		}

		public override void MorpherTrigger(PawnGeneSetHolder geneSet)
		{
			Hediff resurrectionSickness = null;
			pawn.health?.hediffSet?.TryGetHediff(HediffDefOf.ResurrectionSickness, out resurrectionSickness);
			HediffComp_Disappears hediffComp_Disappears = resurrectionSickness?.TryGetComp<HediffComp_Disappears>();
			if (hediffComp_Disappears != null)
			{
				hediffComp_Disappears.ticksToDisappear += 10 * 60000;
			}
			base.MorpherTrigger(geneSet);
		}

	}

	public class Gene_HemogenMorph : Gene_MorpherTrigger
	{

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

		public override bool CanMorph()
		{
			if (Hemogen != null && Hemogen.ValuePercent > 0.8f)
			{
				return true;
			}
			return false;
		}

		public override void MorpherTrigger(PawnGeneSetHolder geneSet)
		{
			if (Hemogen != null)
			{
				Hemogen.Value = 0.1f;
			}
			base.MorpherTrigger(geneSet);
		}

	}

	public class Gene_PsyfocusMorph : Gene_MorpherTrigger
	{

		public override bool CanMorph()
		{
			float? psyfocus = pawn.psychicEntropy?.CurrentPsyfocus;
			if (psyfocus.HasValue && psyfocus.Value > 0.8f)
			{
				return true;
			}
			return false;
		}

		public override void MorpherTrigger(PawnGeneSetHolder geneSet)
		{
			pawn.psychicEntropy?.OffsetPsyfocusDirectly(-1f);
			base.MorpherTrigger(geneSet);
		}

	}

	public class Gene_AbilityMorph : Gene_MorpherTrigger
	{

		public override bool CanMorph()
		{
			return true;
		}

	}

	public class Gene_MonolithMorph : Gene_AbilityMorph
	{

        [Unsaved(false)]
        private GameComponent_Anomaly cachedGameComponent;

        public GameComponent_Anomaly GameComponent
        {
            get
            {
                if (cachedGameComponent == null)
                {
                    cachedGameComponent = Current.Game.GetComponent<GameComponent_Anomaly>();
                }
                return cachedGameComponent;
            }
        }

        public override bool CanMorph()
		{
			return GameComponent?.LevelDef == MonolithLevelDefOf.Embraced;
		}

	}

	public class Gene_DeathRefusalMorph : Gene_MorpherTrigger
	{

		public override bool CanMorph()
		{
			return pawn.health?.hediffSet?.HasHediff(HediffDefOf.DeathRefusal) == true;
		}

		public override void MorpherTrigger(PawnGeneSetHolder geneSet)
		{
			Hediff resurrectionSickness = null;
			pawn.health?.hediffSet?.TryGetHediff(HediffDefOf.DeathRefusal, out resurrectionSickness);
			if (resurrectionSickness is Hediff_DeathRefusal refusal && refusal.UsesLeft > 1)
			{
				refusal.SetUseAmountDirect(refusal.UsesLeft - 1, true);
			}
			else
            {
				pawn.health.RemoveHediff(resurrectionSickness);
            }
			base.MorpherTrigger(geneSet);
		}

	}

}
