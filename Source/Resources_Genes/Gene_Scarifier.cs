using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Scarifier : Gene
	{

		private int scarifyInterval = 60000;
		private int? cachedMaxScars;

        public bool CanScarify
        {
            get
			{
				if (!cachedMaxScars.HasValue)
				{
					MaxScars();
				}
				if (cachedMaxScars.Value > pawn.health.hediffSet.GetHediffCount(HediffDefOf.Scarification))
				{
					return true;
				}
				return false;
			}
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
			ResetInterval();
		}

		public override void Tick()
		{
			//base.Tick();
			// if (!pawn.IsHashIntervalTick(1500))
			scarifyInterval--;
			if (scarifyInterval > 0)
			{
				return;
			}
			ScarifierScarify();
		}

		private void ResetInterval()
		{
			IntRange range = new(50000,140000);
			scarifyInterval = range.RandomInRange;
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref scarifyInterval, "scarifyInterval");
			//Scribe_Values.Look(ref cachedMaxScars, "cachedMaxScars");
		}

		public void ScarifierScarify()
		{
			if (Active)
			{
				cachedMaxScars = null;
				if (CanScarify)
				{
					Scarify(pawn);
				}
			}
			ResetInterval();
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
			//List<BodyPartRecord> bodyparts = new();
			//foreach (BodyPartRecord notMissingPart in pawn.health.hediffSet.GetNotMissingParts())
			//{
			//	if (notMissingPart.def.canScarify)
			//	{
			//		bodyparts.Add(notMissingPart);
			//	}
			//}
			if (!pawn.health.hediffSet.GetNotMissingParts().Where((BodyPartRecord part) => part.def.canScarify && !pawn.health.WouldDieAfterAddingHediff(HediffDefOf.Scarification, part, 1f) && !pawn.health.WouldLosePartAfterAddingHediff(HediffDefOf.Scarification, part, 1f)).ToList().TryRandomElement(out BodyPartRecord part))
			{
				return;
			}
			Hediff hediff = HediffMaker.MakeHediff(HediffDefOf.Scarification, pawn, part);
			HediffComp_GetsPermanent hediffComp_GetsPermanent = hediff.TryGetComp<HediffComp_GetsPermanent>();
			hediffComp_GetsPermanent.IsPermanent = true;
			hediffComp_GetsPermanent.permanentDamageThreshold = 0f;
			hediffComp_GetsPermanent.SetPainCategory(PainCategory.Painless);
			pawn.health.AddHediff(hediff);
			if (pawn.RaceProps.BloodDef == null || pawn.Map == null)
			{
				return;
			}
			SoundDefOf.Execute_Cut.PlayOneShot(pawn);
			GeneFeaturesUtility.TrySpawnBloodFilth(pawn, new(2,3));
		}

		public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
		{
			yield return new StatDrawEntry(StatCategoryDefOf.Genetics, "WVC_XaG_Gene_DisplayStats_Scarifier_MaxScars".Translate().CapitalizeFirst(), MaxScars().ToString(), "WVC_XaG_Gene_DisplayStats_Scarifier_MaxScars_Desc".Translate(), 500);
		}

		//public string GetInspectInfo
		//{
		//	get
		//	{
		//		if (pawn.Drafted)
		//		{
		//			return null;
		//		}
		//		if (CanScarify)
		//		{
		//			return "WVC_XaG_Gene_Scarifier_On_Info".Translate().Resolve() + ": " + scarifyInterval.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor);
		//		}
		//		return null;
		//	}
		//}

	}

	// public class Gene_Scarifier_AdditionalScars : Gene
	// {

	// }
}
