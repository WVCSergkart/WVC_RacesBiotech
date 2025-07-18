using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Scarifier : Gene
	{

		public GeneExtension_Giver Giver => def?.GetModExtension<GeneExtension_Giver>();

		private int nextTick = 60000;
		private float? cachedMaxScars;

        public bool CanScarify
        {
            get
			{
				if (MaxScars > pawn.health.hediffSet.GetHediffCount(HediffDefOf.Scarification))
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
			if (MiscUtility.GameNotStarted())
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
                nextTick = new IntRange(50000, 140000).RandomInRange;
            }
			else
			{
				nextTick = 300000;
				//ResetInterval();
			}
		}

		public override void TickInterval(int delta)
		{
			//base.Tick();
			// if (!pawn.IsHashIntervalTick(1500))
			//nextTick--;
			//if (nextTick > 0)
			//{
			//	return;
			//}
			if (!GeneResourceUtility.CanTick(ref nextTick, 240000, delta))
			{
				return;
			}
			ScarifierScarify();
		}

		//private void ResetInterval()
		//{
		//	IntRange range = new(50000, 140000);
		//	scarifyInterval = range.RandomInRange;
		//}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref nextTick, "scarifyInterval");
			//Scribe_Values.Look(ref cachedMaxScars, "cachedMaxScars");
		}

		public void ScarifierScarify()
		{
			cachedMaxScars = null;
			if (CanScarify)
			{
				Scarify(pawn);
			}
			//ResetInterval();
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

		private float MaxScars
        {
            get
            {
                if (!cachedMaxScars.HasValue)
                {
					cachedMaxScars = pawn.GetStatValue(Giver.scarsStatDef);
				}
                return cachedMaxScars.Value;
            }
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
			if (hediffComp_GetsPermanent != null)
			{
				hediffComp_GetsPermanent.IsPermanent = true;
				//hediffComp_GetsPermanent.permanentDamageThreshold = 0f;
				hediffComp_GetsPermanent.parent.Severity = 0.001f;
				hediffComp_GetsPermanent.SetPainCategory(PainCategory.Painless);
			}
            pawn.health.AddHediff(hediff);
            Notify_SubGenes(pawn);
            if (pawn.RaceProps.BloodDef == null || pawn.Map == null)
            {
                return;
            }
            SoundDefOf.Execute_Cut.PlayOneShot(pawn);
            GeneFeaturesUtility.TrySpawnBloodFilth(pawn, new(2, 3));
        }

        private static void Notify_SubGenes(Pawn pawn)
        {
            foreach (Hediff item in pawn.health.hediffSet.hediffs)
            {
                if (item is HediffWithComps_Scars scarsHediff)
                {
                    scarsHediff.Reset();
                }
			}
			foreach (Gene item in pawn.genes.GenesListForReading)
			{
				if (item is IGeneScarifier subgene && item.Active)
				{
					subgene.Notify_Scarified();
				}
			}
		}

        public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
		{
			yield return new StatDrawEntry(StatCategoryDefOf.Genetics, Giver.scarsStatDef.LabelCap, pawn.GetStatValue(Giver.scarsStatDef).ToString(), Giver.scarsStatDef.description, 500);
		}

  //      public void Notify_GenesChanged(Gene changedGene)
		//{
		//	Notify_SubGenes(pawn);
		//}

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

	public class Gene_ScarifierStability : Gene_GeneticStability, IGeneScarifier
	{

		public override void TickInterval(int delta)
		{

		}

		public void Notify_Scarified()
        {
			Stabilize();
        }

	}

	// public class Gene_Scarifier_AdditionalScars : Gene
	// {

	// }
}
