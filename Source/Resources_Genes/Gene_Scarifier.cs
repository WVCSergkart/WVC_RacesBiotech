using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Scarifier : Gene, IGeneInspectInfo
	{

		// For external requests
		public int scarifyInterval = 60000;
		public int cachedMaxScars = 3;

		public bool CanScarify => CanScarifyCheck();

		private bool CanScarifyCheck()
		{
			if (cachedMaxScars > pawn.health.hediffSet.GetHediffCount(HediffDefOf.Scarification))
			{
				return true;
			}
			return false;
		}

		// public IntRange range = new(1,5);
		// public int BaseScars => def.GetModExtension<GeneExtension_Spawner>().stackCount;

		public override void PostAdd()
		{
			base.PostAdd();
			if (Active && !pawn.Spawned)
			{
				IntRange range = new(0,3);
				for (int i = 0; i < range.RandomInRange; i++)
				{
					// if (!CanScarifyCheck())
					// {
						// continue;
					// }
					Scarify(pawn);
				}
			}
			TimeBeforeScarify();
		}

		public override void Tick()
		{
			base.Tick();
			// if (!pawn.IsHashIntervalTick(1500))
			if (!pawn.IsHashIntervalTick(scarifyInterval))
			{
				return;
			}
			ScarifierScarify();
		}

		private void TimeBeforeScarify()
		{
			IntRange range = new(50000,140000);
			scarifyInterval = range.RandomInRange;
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref scarifyInterval, "scarifyInterval");
			Scribe_Values.Look(ref cachedMaxScars, "cachedMaxScars");
		}

		public void ScarifierScarify()
		{
			if (!Active)
			{
				return;
			}
			if (MaxScars() > pawn.health.hediffSet.GetHediffCount(HediffDefOf.Scarification))
			{
				Scarify(pawn);
			}
			TimeBeforeScarify();
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: Scarify",
					action = delegate
					{
						ScarifierScarify();
					}
				};
			}
		}

		private int MaxScars()
		{
			int scars = def.GetModExtension<GeneExtension_Giver>().scarsCount;
			List<Gene> genesListForReading = pawn.genes?.GenesListForReading;
			if (genesListForReading.NullOrEmpty())
			{
				cachedMaxScars = scars;
				return scars;
			}
			foreach (Gene item in genesListForReading)
			{
				GeneExtension_Giver modExtension = item?.def?.GetModExtension<GeneExtension_Giver>();
				if (item?.Active != true || modExtension == null || modExtension.scarsCount == 0)
				{
					continue;
				}
				scars += modExtension.scarsCount;
			}
			cachedMaxScars = scars;
			return scars;
		}

		public static void Scarify(Pawn pawn)
		{
			if (!ModLister.CheckIdeology("Scarification"))
			{
				return;
			}
			List<BodyPartRecord> bodyparts = new();
			foreach (BodyPartRecord notMissingPart in pawn.health.hediffSet.GetNotMissingParts())
			{
				if (notMissingPart.def.canScarify)
				{
					bodyparts.Add(notMissingPart);
					// Log.Error("2");
				}
			}
			BodyPartRecord part = bodyparts.RandomElement();
			if (part == null || pawn.health.WouldDieAfterAddingHediff(HediffDefOf.Scarification, part, 1f) && pawn.health.WouldLosePartAfterAddingHediff(HediffDefOf.Scarification, part, 1f))
			{
				return;
			}
			Hediff hediff = HediffMaker.MakeHediff(HediffDefOf.Scarification, pawn, part);
			HediffComp_GetsPermanent hediffComp_GetsPermanent = hediff.TryGetComp<HediffComp_GetsPermanent>();
			hediffComp_GetsPermanent.IsPermanent = true;
			hediffComp_GetsPermanent.permanentDamageThreshold = 0f;
			// hediffComp_GetsPermanent.SetPainCategory(JobDriver_Scarify.InjuryPainCategories.RandomElementByWeight((HealthTuning.PainCategoryWeighted e) => e.weight).category);
			hediffComp_GetsPermanent.SetPainCategory(PainCategory.Painless);
			pawn.health.AddHediff(hediff);
			// Log.Error("3");
			if (pawn.RaceProps.BloodDef == null || pawn.Map == null)
			{
				return;
			}
			SoundDefOf.Execute_Cut.PlayOneShot(pawn);
			// CellRect cellRect = new(pawn.PositionHeld.x - 1, pawn.PositionHeld.z - 1, 3, 3);
			// for (int i = 0; i < 3; i++)
			// {
				// IntVec3 randomCell = cellRect.RandomCell;
				// if (!randomCell.InBounds(pawn.Map) || !GenSight.LineOfSight(randomCell, pawn.PositionHeld, pawn.Map))
				// {
					// continue;
				// }
				// FilthMaker.TryMakeFilth(randomCell, pawn.MapHeld, pawn.RaceProps.BloodDef, pawn.LabelIndefinite());
			// }
			GeneFeaturesUtility.TrySpawnBloodFilth(pawn, new(2,3));
		}

		public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
		{
			yield return new StatDrawEntry(StatCategoryDefOf.Genetics, "WVC_XaG_Gene_DisplayStats_Scarifier_MaxScars".Translate().CapitalizeFirst(), MaxScars().ToString(), "WVC_XaG_Gene_DisplayStats_Scarifier_MaxScars_Desc".Translate(), 500);
		}

		public string GetInspectInfo
		{
			get
			{
				if (pawn.Drafted)
				{
					return null;
				}
				if (CanScarify)
				{
					return "WVC_XaG_Gene_Scarifier_On_Info".Translate().Resolve() + ": " + scarifyInterval.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor);
				}
				return null;
			}
		}

	}

	// public class Gene_Scarifier_AdditionalScars : Gene
	// {

	// }
}
