using System.Collections.Generic;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class HediffComp_XenotypeGestator : HediffComp, IHediffCustomPregnancy
	{

		private readonly int ticksInday = 60000;

		private int ticksCounter = 0;

		public float gestationIntervalDays;

		//public XenotypeDef xenotypeDef;

		private XenotypeHolder_Exposable xenotypeHolder;

		public void SetupHolder(XenotypeHolder holder)
		{
			xenotypeHolder = new XenotypeHolder_Exposable(holder);
		}


		public HediffCompProperties_Gestator Props => (HediffCompProperties_Gestator)props;

		//public override bool CompShouldRemove => XenotypeGestator == null;

		//[Unsaved(false)]
		//private Gene_SimpleGestator cachedGestatorGene;

		//public Gene_SimpleGestator XenotypeGestator
		//{
		//	get
		//	{
		//		if (cachedGestatorGene == null || !cachedGestatorGene.Active)
		//		{
		//			cachedGestatorGene = Pawn?.genes?.GetFirstGeneOfType<Gene_XenotypeGestator>();
		//		}
		//		return cachedGestatorGene;
		//	}
		//}

		private GeneSet cachedGeneSet;
		public GeneSet GeneSet
		{
			get
			{
				if (cachedGeneSet == null)
				{
					cachedGeneSet = new();
					HediffUtility.AddGeneDefs(cachedGeneSet, xenotypeHolder.xenotypeDef != null && xenotypeHolder.xenotypeDef != XenotypeDefOf.Baseliner ? xenotypeHolder.xenotypeDef.genes : xenotypeHolder.genes);
					cachedGeneSet.SortGenes();
				}
				return cachedGeneSet;
			}
		}

		public override string CompLabelInBracketsExtra => GetLabel();

		public override void CompPostMake()
		{
			if (Props.gestationIntervalDays > 0)
			{
				gestationIntervalDays = Props.gestationIntervalDays;
			}
			else
			{
				gestationIntervalDays = Pawn.RaceProps.gestationPeriodDays;
			}
			//if (Props.xenotypeDef != null)
			//{
			//	xenotypeDef = Props.xenotypeDef;
			//}
		}
		public override void CompPostPostAdd(DamageInfo? dinfo)
		{
			if (Pawn.Faction != Faction.OfPlayer || !Pawn.ageTracker.CurLifeStage.reproductive)
			{
				RemoveHediff();
			}
		}

		public override void CompExposeData()
		{
			//base.CompExposeData();
			Scribe_Values.Look(ref ticksCounter, "ticksCounter_" + Props.uniqueTag, 0);
			Scribe_Values.Look(ref gestationIntervalDays, "gestationIntervalDays_" + Props.uniqueTag, 0);
			Scribe_Deep.Look(ref xenotypeHolder, "xenotypeHolder_" + Props.uniqueTag);
		}

		public override void CompPostTickInterval(ref float severityAdjustment, int delta)
		{
			ticksCounter += delta;
			if (ticksCounter < ticksInday * gestationIntervalDays)
			{
				return;
			}
			if (Pawn.Map == null)
			{
				return;
			}
			EndGestation();
		}

		private void EndGestation()
		{
			GestationUtility.GestateChild_WithXenotype(Pawn, null, xenotypeHolder, Props.completeLetterLabel, Props.completeLetterDesc);
			ticksCounter = 0;
			RemoveHediff(true);
			Pawn.health.AddHediff(HediffDefOf.Lactating);
		}

		private void RemoveHediff(bool replaceWithOtherHediff = false)
		{
			Pawn.health.RemoveHediff(parent);
			if (replaceWithOtherHediff && Props.hediffDef != null)
			{
				Hediff hediff = HediffMaker.MakeHediff(Props.hediffDef, Pawn);
				Pawn.health.AddHediff(hediff);
			}
		}

		public string GetLabel()
		{
			if (Pawn.Faction == Faction.OfPlayer)
			{
				return (ticksCounter / (ticksInday * gestationIntervalDays)).ToStringPercent();
			}
			return "";
		}

		public override IEnumerable<Gizmo> CompGetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: CompleteGestation",
					action = delegate
					{
						EndGestation();
					}
				};
			}
			if (!Pawn.Drafted)
			{
				yield return XaG_UiUtility.ITab_InspectBabyGenes();
			}
		}

	}

}
