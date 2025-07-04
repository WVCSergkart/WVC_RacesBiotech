using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	// Rare Hemogen Drain
	public class Gene_HemogenDependant : Gene
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

		public override void TickInterval(int delta)
		{

		}

	}

	// Rare Hemogen Drain
	public class Gene_HemogenOffset : Gene_HemogenDependant, IGeneResourceDrain
	{

		public Gene_Resource Resource => Hemogen;

		public virtual bool CanOffset
		{
			get
			{
				return Hemogen?.CanOffset == true;
			}
		}

		public virtual float ResourceLossPerDay => def.resourceLossPerDay;

		public Pawn Pawn => pawn;

		public string DisplayLabel => Label + " (" + "Gene".Translate() + ")";

		//private int tick;

		//public override void Tick()
		//{
  //          tick--;
  //          if (tick <= 0)
  //          {
  //              tick = 360;
  //              Log.Error("Tick");
  //          }
  //      }

		public override void TickInterval(int delta)
		{
			//tick--;
			//if (tick <= 0)
			//{
			//	tick = 360;
			//	Log.Error("Tick");
			//}
			if (pawn.IsHashIntervalTick(360, delta))
			//if (GeneResourceUtility.CanTick(ref nextTick, 360))
			{
				//Log.Error("tick: " + 360);
				// Log.Error(tick.ToString() + " | 120");
				// tick = 0;
				//Log.Error("TickHemogenDrain");
                GeneResourceUtility.TickHemogenDrain(this, 360, delta, CanOffset);
			}
		}

		//public override void ExposeData()
		//{
		//	base.ExposeData();
		//	Scribe_Values.Look(ref nextTick, "nextTick", -1);
		//}

	}

	// public class Gene_Bloodfeeder : Gene_HemogenOffset
	// {

		// public virtual void Notify_Bloodfeed(Pawn victim)
		// {
		// }

	// }

	public class Gene_BloodHunter : Gene_HemogenOffset
	{

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: TryHuntForFood",
					action = delegate
					{
						if (!TryHuntForFood(pawn))
						{
							Log.Error("Target is null");
						}
					}
				};
			}
		}

		public static bool TryHuntForFood(Pawn pawn, bool requestQueueing = true, bool queue = false)
		{
			if (!queue && Gene_Rechargeable.PawnHaveThisJob(pawn, MainDefOf.WVC_XaG_CastBloodfeedOnPawnMelee))
			{
				return false;
			}
			// =
			List<Pawn> targets = MiscUtility.GetAllPlayerControlledMapPawns_ForBloodfeed(pawn);
			// =
			foreach (Pawn colonist in targets)
			{
                if (!GeneFeaturesUtility.CanBloodFeedNowWith(pawn, colonist))
                {
                    continue;
                }
                if (colonist.IsForbidden(pawn) || !pawn.CanReserveAndReach(colonist, PathEndMode.OnCell, pawn.NormalMaxDanger()))
				{
					continue;
				}
				//if (colonist.health.hediffSet.HasHediff(HediffDefOf.BloodLoss))
				//{
				//	continue;
				//}
				if (!MiscUtility.TryGetAbilityJob(pawn, colonist, MainDefOf.Bloodfeed, out Job job))
				{
					continue;
				}
				job.def = MainDefOf.WVC_XaG_CastBloodfeedOnPawnMelee;
				pawn.TryTakeOrderedJob(job, JobTag.SatisfyingNeeds, requestQueueing);
				return true;
			}
			return false;
		}
		
		//[Obsolete]
		//public static bool PawnHaveBloodHuntJob(Pawn pawn, Job job)
		//{
		//	foreach (Job item in pawn.jobs.AllJobs().ToList())
		//	{
		//		if (item.def == job.def && item.ability == job.ability)
		//		{
		//			return true;
		//		}
		//	}
		//	return false;
		//}

		//[Obsolete]
		//public static bool PawnReserved(List<Pawn> biters, Pawn victim, Pawn biter)
		//{
		//	foreach (Pawn item in biters)
		//	{
		//		if (item == biter)
		//		{
		//			continue;
		//		}
		//		foreach (Job job in item.jobs.AllJobs().ToList())
		//		{
		//			if (job?.targetA.Pawn != null && job.targetA.Pawn == victim)
		//			{
		//				return true;
		//			}
		//		}
		//	}
		//	return false;
		//}

		//public static List<Pawn> GetAllBloodHuntersFromList(List<Pawn> pawns)
		//{
		//	List<Pawn> hunters = new();
		//	foreach (Pawn item in pawns.ToList())
		//	{
		//		if (item?.genes?.GetFirstGeneOfType<Gene_BloodHunter>() == null)
		//		{
		//			continue;
		//		}
		//		hunters.Add(item);
		//	}
		//	return hunters;
		//}

		// public static List<Pawn> GetAllBloodfeedPrisonersFromList(List<Pawn> pawns, bool removeFromList = true)
		// {
			// List<Pawn> hunters = new();
			// foreach (Pawn item in pawns.ToList())
			// {
				// if (item?.guest?.IsInteractionDisabled(PrisonerInteractionModeDefOf.Bloodfeed))
				// {
					// continue;
				// }
				// hunters.Add(item);
			// }
			// return hunters;
		// }

	}

	public class Gene_ImplanterFang : Gene_HemogenOffset, IGeneBloodfeeder
	{

		public GeneExtension_General General => def?.GetModExtension<GeneExtension_General>();

		public void Notify_Bloodfeed(Pawn victim)
		{
			// Log.Error("0");
			if (General == null || victim == null)
			{
				return;
			}
			if (victim.Dead)
			{
				return;
			}
			// Log.Error("1");
			if (victim?.health?.immunity?.AnyGeneMakesFullyImmuneTo(MainDefOf.WVC_XaG_ImplanterFangsMark) == true)
			{
				return;
			}
			// Log.Error("2");
			float? immunity = victim?.GetStatValue(StatDefOf.ImmunityGainSpeed, cacheStaleAfterTicks: 360000);
			float chance = (General.reimplantChance / (immunity.HasValue && immunity.Value > 0.01f ? immunity.Value : 0.01f)) * WVC_Biotech.settings.hemogenic_ImplanterFangsChanceFactor;
			if (!Rand.Chance(chance))
			{
				// Log.Error("2 Chance: " + (General.reimplantChance / (immunity > 0.01f ? immunity : 0.01f)).ToString());
				return;
			}
			// Log.Error("3");
			if (GeneUtility.PawnWouldDieFromReimplanting(pawn))
			{
				return;
			}
			// Log.Error("4");
			if (ReimplanterUtility.TryReimplant(pawn, victim, General.reimplantEndogenes, General.reimplantXenogenes))
			{
				victim.health.AddHediff(MainDefOf.WVC_XaG_ImplanterFangsMark, ExecutionUtility.ExecuteCutPart(victim));
				if (PawnUtility.ShouldSendNotificationAbout(pawn) || PawnUtility.ShouldSendNotificationAbout(victim))
				{
					int max = HediffDefOf.XenogerminationComa.CompProps<HediffCompProperties_Disappears>().disappearsAfterTicks.max;
					int max2 = HediffDefOf.XenogermLossShock.CompProps<HediffCompProperties_Disappears>().disappearsAfterTicks.max;
					Find.LetterStack.ReceiveLetter("LetterLabelGenesImplanted".Translate(), "LetterTextGenesImplanted".Translate(pawn.Named("CASTER"), victim.Named("TARGET"), max.ToStringTicksToPeriod().Named("COMADURATION"), max2.ToStringTicksToPeriod().Named("SHOCKDURATION")), LetterDefOf.NeutralEvent, new LookTargets(pawn, victim));
				}
			}
		}

	}

	public class Gene_Bloodfeeder : Verse.Gene_Bloodfeeder
	{
		//public override void PostAdd()
		//{
		//	base.PostAdd();
		//	if (pawn.IsPrisonerOfColony && pawn.guest != null && pawn.guest.HasInteractionWith((PrisonerInteractionModeDef interaction) => interaction.hideIfNoBloodfeeders))
		//	{
		//		pawn.guest.SetNoInteraction();
		//	}
		//}

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

		public override void TickInterval(int delta)
		{
			// base.Tick();
			if (!pawn.IsHashIntervalTick(10628, delta))
			{
				return;
			}
			if (pawn.Faction != Faction.OfPlayer)
			{
				return;
			}
			if (Hemogen?.ShouldConsumeHemogenNow() != true)
			{
				return;
			}
			if (pawn.Map == null)
			{
				// In caravan use
				InCaravan();
				return;
			}
			if (pawn.Downed || pawn.Drafted || !pawn.Awake())
			{
				return;
			}
			Gene_BloodHunter.TryHuntForFood(pawn);
		}

		private void InCaravan()
		{
			Caravan caravan = pawn.GetCaravan();
			if (caravan == null)
			{
				return;
			}
			List<Pawn> pawns = caravan.PawnsListForReading;
			if (pawns.NullOrEmpty())
			{
				return;
			}
			pawns.Shuffle();
			for (int j = 0; j < pawns.Count; j++)
			{
				if (!GeneFeaturesUtility.CanBloodFeedNowWith(pawn, pawns[j]))
				{
					continue;
				}
				SanguophageUtility.DoBite(pawn, pawns[j], 0.2f, 0.1f, 0.4f, 1f, new (0, 0), ThoughtDefOf.FedOn, ThoughtDefOf.FedOn_Social);
				break;
			}
		}

	}

	public class Gene_HemogenicMetabolism : Gene_HemogenOffset
	{

		public bool consumeHemogen = false;

		private float? cachedNutritionPerTick;

		public override float ResourceLossPerDay
		{
			get
			{
				if (consumeHemogen)
				{
					return def.resourceLossPerDay;
				}
				return 0f;
			}
		}

		public override void TickInterval(int delta)
		{
			if (!consumeHemogen)
			{
				return;
			}
			base.TickInterval(delta);
			if (!pawn.IsHashIntervalTick(527, delta))
			{
				return;
			}
			ReplenishHunger();
			if (Hemogen == null || Hemogen.MinLevelForAlert > Hemogen.Value)
			{
				consumeHemogen = false;
				Messages.Message("WVC_XaG_HemogenicMetabolism_LowHemogen".Translate(), pawn, MessageTypeDefOf.NeutralEvent, historical: false);
			}
		}

		public void ReplenishHunger()
		{
			if (!cachedNutritionPerTick.HasValue)
			{
				cachedNutritionPerTick = 0.02f + (pawn.needs?.food != null ? pawn.needs.food.FoodFallPerTick : 0f);
			}
			if (!GeneResourceUtility.TryOffsetNeedFood(pawn, cachedNutritionPerTick.Value, 0.05f))
			{
				consumeHemogen = false;
			}
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (XaG_GeneUtility.SelectorActiveFaction(pawn, this))
			{
				yield break;
			}
			yield return new Command_Action
			{
				defaultLabel = def.LabelCap + ": " + XaG_UiUtility.OnOrOff(consumeHemogen),
				defaultDesc = "WVC_XaG_Gene_HemogenicMetabolismDesc".Translate(),
				icon = ContentFinder<Texture2D>.Get(def.iconPath),
				action = delegate
				{
					consumeHemogen = !consumeHemogen;
					XaG_UiUtility.FlickSound(!consumeHemogen);
				}
			};
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref consumeHemogen, "consumeHemogen", false);
		}

	}

}
