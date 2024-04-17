using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

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

		public override void CompPostTick(ref float severityAdjustment)
		{
			base.CompPostTick(ref severityAdjustment);
			Pawn golem = parent.pawn;
			Pawn pawn = golem.GetOverseer();
			if (pawn == null)
			{
				base.Pawn.Kill(null, parent);
				return;
			}
			if (childOwner == null)
			{
				childOwner = pawn;
			}
			if (golem.Map == null)
			{
				return;
			}
			if (golem.Faction != Faction.OfPlayer)
			{
				return;
			}
			ticksCounter++;
			if (ticksCounter < ticksInday * GestationIntervalDays)
			{
				return;
			}
			EndGestation();
			ticksCounter = 0;
		}

		private void EndGestation()
		{
			if (XaG_GeneUtility.HasActiveGene(Props.geneDef, childOwner))
			{
				GestationUtility.GenerateNewBornPawn(childOwner, Props.completeMessage, Props.endogeneTransfer, Props.xenogeneTransfer, spawnPawn: parent.pawn);
			}
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
