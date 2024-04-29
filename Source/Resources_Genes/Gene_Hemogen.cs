using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{

	// Rare Hemogen Drain
	public class Gene_HemogenOffset : Gene, IGeneResourceDrain
	{

		[Unsaved(false)]
		private Gene_Hemogen cachedHemogenGene;

		public Gene_Resource Resource => Hemogen;

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

		public override void Tick()
		{
			base.Tick();
			if (pawn.IsHashIntervalTick(120))
			{
				TickHemogenDrain(this, 120);
			}
		}

		public static void TickHemogenDrain(IGeneResourceDrain drain, int tick = 1)
		{
			if (drain.Resource != null && drain.CanOffset)
			{
				GeneResourceDrainUtility.OffsetResource(drain, ((0f - drain.ResourceLossPerDay) / 60000f) * tick);
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
			List<Pawn> targets = new();
			// List<Pawn> colonists = pawn?.Map?.mapPawns?.SpawnedPawnsInFaction(pawn.Faction);
			// =
			List<Pawn> prisoners = GetAndSortPrisoners(pawn);
			targets.AddRange(prisoners);
			// =
			List<Pawn> slaves = pawn?.Map?.mapPawns?.SlavesOfColonySpawned;
			slaves.Shuffle();
			targets.AddRange(slaves);
			// =
			List<Pawn> colonists = pawn?.Map?.mapPawns?.FreeColonists;
			colonists.Shuffle();
			targets.AddRange(colonists);
			// =
			// List<Pawn> biters = GetAllBloodHuntersFromList(colonists);
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
			}
			return false;
		}

		public static List<Pawn> GetAndSortPrisoners(Pawn pawn)
		{
			List<Pawn> allPawns = new();
			List<Pawn> prisoners = pawn?.Map?.mapPawns?.PrisonersOfColony;
			List<Pawn> nonBloodfeedPrisoners = new();
			List<Pawn> bloodfeedPrisoners = new();
			foreach (Pawn prisoner in prisoners)
			{
				if (prisoner.guest.IsInteractionDisabled(PrisonerInteractionModeDefOf.Bloodfeed))
				{
					nonBloodfeedPrisoners.Add(prisoner);
				}
				else
				{
					bloodfeedPrisoners.Add(prisoner);
				}
			}
			bloodfeedPrisoners.Shuffle();
			allPawns.AddRange(bloodfeedPrisoners);
			nonBloodfeedPrisoners.Shuffle();
			allPawns.AddRange(nonBloodfeedPrisoners);
			return allPawns;
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
			float? immunity = victim?.GetStatValue(StatDefOf.ImmunityGainSpeed);
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

}
