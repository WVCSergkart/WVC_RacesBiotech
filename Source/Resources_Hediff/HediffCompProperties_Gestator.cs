using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class HediffCompProperties_Gestator : HediffCompProperties
	{

		public int gestationIntervalDays = -1;
		public XenotypeDef xenotypeDef;

		public string completeLetterLabel = "WVC_XaG_XenoTreeBirthLabel";

		public string completeLetterDesc = "WVC_XaG_XenoTreeBirthDesc";

		public bool endogeneTransfer = true;
		public bool xenogeneTransfer = true;

		public bool xenogerminationComa = false;

		public bool shouldBeAdult = true;

		public HediffDef hediffDef;

		public string uniqueTag = "HediffGestator";

		// public HediffCompProperties_Gestator()
		// {
			// compClass = typeof(HediffComp_Gestator);
		// }

	}

	public class HediffComp_XenotypeGestator : HediffComp
	{

		private readonly int ticksInday = 60000;

		private int ticksCounter = 0;

		public float gestationIntervalDays;

		//public XenotypeDef xenotypeDef;

		private SaveableXenotypeHolder xenotypeHolder;

		public void SetupHolder(XenotypeHolder holder)
		{
			xenotypeHolder = new SaveableXenotypeHolder(holder);
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
				return ((float)ticksCounter / (ticksInday * gestationIntervalDays)).ToStringPercent();
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
		}

	}

	public class HediffComp_Gestator : HediffComp
	{
		private readonly int ticksInday = 60000;

		private int ticksCounter = 0;

		public HediffCompProperties_Gestator Props => (HediffCompProperties_Gestator)props;

		protected int GestationIntervalDays => Props.gestationIntervalDays;

		public override string CompLabelInBracketsExtra => GetLabel();

		public override void CompExposeData()
		{
			base.CompExposeData();
			Scribe_Values.Look(ref ticksCounter, "ticksCounter", 0);
		}

		public override void CompPostTickInterval(ref float severityAdjustment, int delta)
		{
			//base.CompPostTick(ref severityAdjustment);
			ticksCounter += delta;
			if (Pawn.Map == null)
			{
				return;
			}
			if (Pawn.Faction != Faction.OfPlayer || !Pawn.ageTracker.CurLifeStage.reproductive)
			{
				base.Pawn.health.RemoveHediff(parent);
				return;
			}
			if (ticksCounter < ticksInday * GestationIntervalDays)
			{
				return;
			}
			EndGestation();
		}

		private void EndGestation()
		{
			GestationUtility.GestateChild_WithGenes(parent.pawn, completeMessage: Props.completeLetterDesc, endogenes: Props.endogeneTransfer, xenogenes: Props.xenogeneTransfer);
			ticksCounter = 0;
			base.Pawn.health.RemoveHediff(parent);
		}

		public string GetLabel()
		{
			Pawn pawn = parent.pawn;
			if (pawn.Faction == Faction.OfPlayer && pawn.ageTracker.CurLifeStage.reproductive)
			{
				float percent = (float)ticksCounter / (float)(ticksInday * GestationIntervalDays);
				return percent.ToStringPercent();
			}
			return "";
		}

		public override IEnumerable<Gizmo> CompGetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: Complete gestation",
					action = delegate
					{
						EndGestation();
					}
				};
			}
		}

	}

}
