using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class HediffCompProperties_GolemGestator : HediffCompProperties
	{
		public int gestationIntervalDays = 1;

		public string completeMessage = "WVC_RB_Gene_MechaGestator";

		public bool endogeneTransfer = true;

		public bool xenogeneTransfer = true;

		public GeneDef geneDef;

		public HediffCompProperties_GolemGestator()
		{
			compClass = typeof(HediffComp_GolemGestator);
		}
	}

	public class HediffComp_GolemGestator : HediffComp
	{
		private readonly int ticksInday = 60000;

		private int ticksCounter = 0;

		private Pawn childOwner = null;

		public HediffCompProperties_GolemGestator Props => (HediffCompProperties_GolemGestator)props;

		protected int GestationIntervalDays => Props.gestationIntervalDays;

		public override string CompLabelInBracketsExtra => GetLabel();

		public override void CompExposeData()
		{
			base.CompExposeData();
			Scribe_Values.Look(ref ticksCounter, "ticksCounter", 0);
			Scribe_References.Look(ref childOwner, "childOwner");
		}

		private int checkTick = 0;

		public override void CompPostTickInterval(ref float severityAdjustment, int delta)
		{
			ticksCounter += delta;
			checkTick -= delta;
			if (checkTick <= 0)
			{
				Pawn pawn = Pawn.GetOverseer();
				if (pawn == null)
				{
					Pawn.health.RemoveHediff(parent);
					return;
				}
				if (childOwner == null)
				{
					childOwner = pawn;
				}
				checkTick = 4500;
			}
			if (ticksCounter < ticksInday * GestationIntervalDays)
			{
				return;
			}
			if (Pawn.Map == null)
			{
				return;
			}
			if (Pawn.Faction != Faction.OfPlayer)
			{
				Pawn.health.RemoveHediff(parent);
				return;
			}
			EndGestation();
			ticksCounter = 0;
		}

		private void EndGestation()
		{
			// Log.Error("1");
			if (XaG_GeneUtility.HasActiveGene(Props.geneDef, childOwner))
			{
				// Log.Error("2");
				GestationUtility.GestateChild_WithGenes(childOwner, motherOrEgg: parent.pawn, completeMessage: Props.completeMessage, endogenes: Props.endogeneTransfer, xenogenes: Props.xenogeneTransfer);
			}
			Pawn.health.RemoveHediff(parent);
			base.Pawn.Kill(null, parent);
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

		public string GetLabel()
		{
			Pawn pawn = parent.pawn;
			if (pawn.Faction == Faction.OfPlayer)
			{
				float percent = (float)ticksCounter / (float)(ticksInday * GestationIntervalDays);
				return percent.ToStringPercent();
			}
			return "";
		}
	}

}
