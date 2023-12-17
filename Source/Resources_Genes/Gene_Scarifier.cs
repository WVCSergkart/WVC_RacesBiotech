using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Scarifier : Gene
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
			IntRange range = new(1,5);
			for (int i = 0; i < range.RandomInRange; i++)
			{
				// Log.Error("1");
				ScarifierScarify();
			}
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
			if (Active)
			{
				if (MaxScars() > pawn.health.hediffSet.GetHediffCount(HediffDefOf.Scarification))
				{
					Scarify(pawn);
					TimeBeforeScarify();
				}
			}
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
			List<GeneDef> genesListForReading = def.GetModExtension<GeneExtension_Giver>()?.scarGeneDefs;
			if (!genesListForReading.NullOrEmpty())
			{
				for (int i = 0; i < genesListForReading.Count; i++)
				{
					if (XaG_GeneUtility.HasActiveGene(genesListForReading[i], pawn) && genesListForReading[i].GetModExtension<GeneExtension_Giver>() != null)
					{
						scars += genesListForReading[i].GetModExtension<GeneExtension_Giver>().scarsCount;
					}
				}
			}
			cachedMaxScars = scars;
			// Log.Error("Max scars " + scars);
			return scars;
		}

		public static void Scarify(Pawn pawn)
		{
			if (ModLister.CheckIdeology("Scarification"))
			{
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
				if (part != null && (!pawn.health.WouldDieAfterAddingHediff(HediffDefOf.Scarification, part, 1f) || !pawn.health.WouldLosePartAfterAddingHediff(HediffDefOf.Scarification, part, 1f)))
				{
					Hediff hediff = HediffMaker.MakeHediff(HediffDefOf.Scarification, pawn, part);
					HediffComp_GetsPermanent hediffComp_GetsPermanent = hediff.TryGetComp<HediffComp_GetsPermanent>();
					hediffComp_GetsPermanent.IsPermanent = true;
					hediffComp_GetsPermanent.SetPainCategory(JobDriver_Scarify.InjuryPainCategories.RandomElementByWeight((HealthTuning.PainCategoryWeighted e) => e.weight).category);
					pawn.health.AddHediff(hediff);
					// Log.Error("3");
					if (pawn.RaceProps.BloodDef != null && pawn.Map != null)
					{
						SoundDefOf.Execute_Cut.PlayOneShot(pawn);
						CellRect cellRect = new(pawn.PositionHeld.x - 1, pawn.PositionHeld.z - 1, 3, 3);
						for (int i = 0; i < 3; i++)
						{
							IntVec3 randomCell = cellRect.RandomCell;
							if (randomCell.InBounds(pawn.Map) && GenSight.LineOfSight(randomCell, pawn.PositionHeld, pawn.Map))
							{
								FilthMaker.TryMakeFilth(randomCell, pawn.MapHeld, pawn.RaceProps.BloodDef, pawn.LabelIndefinite());
							}
						}
					}
				}
			}
		}

		public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
		{
			yield return new StatDrawEntry(StatCategoryDefOf.Genetics, "WVC_XaG_Gene_DisplayStats_Scarifier_MaxScars".Translate().CapitalizeFirst(), MaxScars().ToString(), "WVC_XaG_Gene_DisplayStats_Scarifier_MaxScars_Desc".Translate(), 500);
		}

	}

	// public class Gene_Scarifier_AdditionalScars : Gene
	// {

	// }
}
