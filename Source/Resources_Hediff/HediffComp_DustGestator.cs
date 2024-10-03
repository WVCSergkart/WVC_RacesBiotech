using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class HediffCompProperties_ImmaculateConception : HediffCompProperties
	{
		public float requiredSeverity = 1.0f;

		// public float removeSeverity = 1.0f;

		public string completeMessage = "WVC_XaG_Gene_ImmaculateConception";

		public bool endogeneTransfer = true;

		public bool xenogeneTransfer = true;

		public HediffDef hediffDef;

		public HediffCompProperties_ImmaculateConception()
		{
			compClass = typeof(HediffComp_ImmaculateConception);
		}
	}

	public class HediffComp_ImmaculateConception : HediffComp
	{

		public HediffCompProperties_ImmaculateConception Props => (HediffCompProperties_ImmaculateConception)props;

		// private bool canBirth = true;

		// protected int GestationIntervalDays => Props.gestationIntervalDays;

		// public override string CompLabelInBracketsExtra => GetLabel();

		public override void CompPostTick(ref float severityAdjustment)
		{
			base.CompPostTick(ref severityAdjustment);
			if (!Pawn.IsHashIntervalTick(1500))
			{
				return;
			}
			// Pawn pawn = parent.pawn;
			if (Pawn.Map == null)
			{
				return;
			}
			if (!MiscUtility.PawnIsColonistOrSlave(Pawn, true))
			{
				RemoveHediff();
				return;
			}
			if (parent.Severity >= Props.requiredSeverity)
			{
				EndGestation();
			}
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

		private void EndGestation()
		{
			// canBirth = false;
			GestationUtility.GestateChild_WithGenes(parent.pawn, completeMessage: Props.completeMessage, endogenes: Props.endogeneTransfer, xenogenes: Props.xenogeneTransfer);
			RemoveHediff(true);
		}

		// public override void CompExposeData()
		// {
			// base.CompExposeData();
			// Scribe_Values.Look(ref canBirth, "canBirth");
		// }

		public override IEnumerable<Gizmo> CompGetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: Complete pregnancy",
					action = delegate
					{
						EndGestation();
					}
				};
			}
		}

	}

}
