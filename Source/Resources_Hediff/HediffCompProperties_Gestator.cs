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

		public int gestationIntervalDays;

		public XenotypeDef xenotypeDef;

		public HediffCompProperties_Gestator Props => (HediffCompProperties_Gestator)props;

		// protected Pawn Pawn => parent.pawn;

		public override string CompLabelInBracketsExtra => GetLabel();

		public override void CompPostMake()
		{
			if (Props.gestationIntervalDays > 0)
			{
				gestationIntervalDays = Props.gestationIntervalDays;
			}
			if (Props.xenotypeDef != null)
			{
				xenotypeDef = Props.xenotypeDef;
			}
		}

		public override void CompExposeData()
		{
			base.CompExposeData();
			Scribe_Values.Look(ref ticksCounter, "ticksCounter_" + Props.uniqueTag, 0);
			Scribe_Defs.Look(ref xenotypeDef, "xenotypeDef_" + Props.uniqueTag);
		}

		public override void CompPostTick(ref float severityAdjustment)
		{
			base.CompPostTick(ref severityAdjustment);
			// Pawn pawn = parent.pawn;
			// if (Props.gestationIntervalDays > 0)
			// {
				// gestationIntervalDays = Props.gestationIntervalDays;
				// return;
			// }
			if (Pawn.Map == null)
			{
				return;
			}
			if (!MiscUtility.PawnIsColonistOrSlave(Pawn, Props.shouldBeAdult))
			{
				RemoveHediff();
				return;
			}
			ticksCounter++;
			if (ticksCounter < ticksInday * gestationIntervalDays)
			{
				return;
			}
			if (xenotypeDef == null)
			{
				RemoveHediff();
				return;
			}
			EndGestation();
		}

		private void EndGestation()
		{
			// GestationUtility.GenerateNewBornPawn(parent.pawn, Props.completeMessage, Props.endogeneTransfer, Props.xenogeneTransfer);
			GestationUtility.GenerateNewBornPawn_WithChosenXenotype(Pawn, xenotypeDef, Props.completeLetterLabel, Props.completeLetterDesc, Props.xenogerminationComa);
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
			// Pawn pawn = parent.pawn;
			if (MiscUtility.PawnIsColonistOrSlave(Pawn, Props.shouldBeAdult))
			{
				float percent = (float)ticksCounter / (float)(ticksInday * gestationIntervalDays);
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
