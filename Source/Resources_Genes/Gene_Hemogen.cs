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

	}

	// Rare Hemogen Drain
	public class Gene_HemogenOffset : Gene_HemogenDependant, IGeneResourceDrain
	{

		public Gene_Resource Resource => Hemogen;

		public bool CanOffset
		{
			get
			{
				return Hemogen?.CanOffset == true;
			}
		}

		public float ResourceLossPerDay => def.resourceLossPerDay;

		public Pawn Pawn => pawn;

		public string DisplayLabel => Label + " (" + "Gene".Translate() + ")";

		// private int tick;

		public override void Tick()
		{
			// base.Tick();
			// tick++;
			if (pawn.IsHashIntervalTick(360))
			{
				// Log.Error(tick.ToString() + " | 120");
				// tick = 0;
				GeneResourceUtility.TickHemogenDrain(this, 360);
			}
		}

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
						if (!TryHuntForFood())
						{
							Log.Error("Target is null");
						}
					}
				};
			}
		}

		public virtual bool TryHuntForFood()
		{
			if (WVC_Biotech.settings.bloodeater_disableAutoFeed)
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
				if (!MiscUtility.TryGetAbilityJob(pawn, colonist, WVC_GenesDefOf.Bloodfeed, out Job job))
				{
					continue;
				}
				// if (PawnReserved(biters, colonist, pawn))
				// {
					// continue;
				// }
				job.def = WVC_GenesDefOf.WVC_XaG_CastBloodfeedOnPawnMelee;
				// job.MakeDriver(pawn);
				if (!PawnHaveBloodHuntJob(pawn, job))
				{
					pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc, true);
					// Log.Error("Target: " + colonist.Name.ToString());
					return true;
				}
				return false;
			}
			return false;
		}

		public static bool PawnHaveBloodHuntJob(Pawn pawn, Job job)
		{
			foreach (Job item in pawn.jobs.AllJobs().ToList())
			{
				if (item.def == job.def && item.ability == job.ability)
				{
					return true;
				}
			}
			return false;
		}

		[Obsolete]
		public static bool PawnReserved(List<Pawn> biters, Pawn victim, Pawn biter)
		{
			foreach (Pawn item in biters)
			{
				if (item == biter)
				{
					continue;
				}
				foreach (Job job in item.jobs.AllJobs().ToList())
				{
					if (job?.targetA.Pawn != null && job.targetA.Pawn == victim)
					{
						return true;
					}
				}
			}
			return false;
		}

		public static List<Pawn> GetAllBloodHuntersFromList(List<Pawn> pawns)
		{
			List<Pawn> hunters = new();
			foreach (Pawn item in pawns.ToList())
			{
				if (item?.genes?.GetFirstGeneOfType<Gene_BloodHunter>() == null)
				{
					continue;
				}
				hunters.Add(item);
			}
			return hunters;
		}

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
			if (victim?.health?.immunity?.AnyGeneMakesFullyImmuneTo(WVC_GenesDefOf.WVC_XaG_ImplanterFangsMark) == true)
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
			if (GeneUtility.PawnWouldDieFromReimplanting(pawn) || GeneUtility.PawnWouldDieFromReimplanting(victim))
			{
				return;
			}
			// Log.Error("4");
			if (ReimplanterUtility.TryReimplant(pawn, victim, General.reimplantEndogenes, General.reimplantXenogenes))
			{
				victim.health.AddHediff(WVC_GenesDefOf.WVC_XaG_ImplanterFangsMark, ExecutionUtility.ExecuteCutPart(victim));
				if (PawnUtility.ShouldSendNotificationAbout(pawn) || PawnUtility.ShouldSendNotificationAbout(victim))
				{
					int max = HediffDefOf.XenogerminationComa.CompProps<HediffCompProperties_Disappears>().disappearsAfterTicks.max;
					int max2 = HediffDefOf.XenogermLossShock.CompProps<HediffCompProperties_Disappears>().disappearsAfterTicks.max;
					Find.LetterStack.ReceiveLetter("LetterLabelGenesImplanted".Translate(), "LetterTextGenesImplanted".Translate(pawn.Named("CASTER"), victim.Named("TARGET"), max.ToStringTicksToPeriod().Named("COMADURATION"), max2.ToStringTicksToPeriod().Named("SHOCKDURATION")), LetterDefOf.NeutralEvent, new LookTargets(pawn, victim));
				}
			}
		}

	}

	public class Gene_Bloodfeeder : Gene_BloodHunter
	{

		public override void Tick()
		{
			// base.Tick();
			if (!pawn.IsHashIntervalTick(10628))
			{
				return;
			}
			if (Hemogen?.ShouldConsumeHemogenNow() != true)
			{
				return;
			}
			if (pawn.Faction != Faction.OfPlayer)
			{
				return;
			}
			if (pawn.Map == null)
			{
				// In caravan use
				InCaravan();
				return;
			}
			if (pawn.Downed || pawn.Drafted)
			{
				return;
			}
			TryHuntForFood();
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

	public class Gene_HemogenicMetabolism : Gene_HemogenDependant, IGeneResourceDrain
	{

		public bool consumeHemogen = false;

		// public new float ResourceLossPerDay
		// {
			// get
			// {
				// if (consumeHemogen)
				// {
					// return def.resourceLossPerDay;
				// }
				// return 0f;
			// }
		// }

		private float? cachedNutritionPerTick;

		public Gene_Resource Resource => Hemogen;

		public bool CanOffset
		{
			get
			{
				return Hemogen?.CanOffset == true;
			}
		}

		public float ResourceLossPerDay
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

		public Pawn Pawn => pawn;

		public string DisplayLabel => Label + " (" + "Gene".Translate() + ")";

		public override void Tick()
		{
			// base.Tick();
			if (!consumeHemogen)
			{
				return;
			}
			if (pawn.IsHashIntervalTick(360))
			{
				GeneResourceUtility.TickHemogenDrain(this, 360);
			}
			if (!pawn.IsHashIntervalTick(527))
			{
				return;
			}
			ReplenishHunger();
			if (Hemogen == null || Hemogen.MinLevelForAlert > Hemogen.Value)
			{
				consumeHemogen = false;
			}
		}

		public void ReplenishHunger()
		{
			if (!cachedNutritionPerTick.HasValue)
			{
				cachedNutritionPerTick = 0.02f + (pawn.needs?.food != null ? pawn.needs.food.FoodFallPerTick : 0f);
			}
			GeneResourceUtility.OffsetNeedFood(pawn, cachedNutritionPerTick.Value);
		}

		public string Flick()
		{
			if (consumeHemogen)
			{
				return "WVC_XaG_Gene_DustMechlink_On".Translate().Colorize(ColorLibrary.Green);
			}
			return "WVC_XaG_Gene_DustMechlink_Off".Translate().Colorize(ColorLibrary.RedReadable);
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (XaG_GeneUtility.SelectorActiveFaction(pawn, this))
			{
				yield break;
			}
			yield return new Command_Action
			{
				defaultLabel = def.LabelCap + ": " + Flick(),
				defaultDesc = "WVC_XaG_Gene_HemogenicMetabolismDesc".Translate(),
				icon = ContentFinder<Texture2D>.Get(def.iconPath),
				action = delegate
				{
					consumeHemogen = !consumeHemogen;
					if (!consumeHemogen)
					{
						SoundDefOf.Tick_High.PlayOneShotOnCamera();
					}
					else
					{
						SoundDefOf.Tick_Low.PlayOneShotOnCamera();
					}
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
