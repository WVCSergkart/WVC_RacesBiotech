using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.AI.Group;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	public class HediffCompProperties_ImmaculateConception : HediffCompProperties
	{
		public float requiredSeverity = 1.0f;

		public string completeMessage = "WVC_XaG_Gene_ImmaculateConception";

		public bool endogeneTransfer = true;

		public bool xenogeneTransfer = true;

		public HediffCompProperties_ImmaculateConception()
		{
			compClass = typeof(HediffComp_ImmaculateConception);
		}
	}

	public class HediffComp_ImmaculateConception : HediffComp
	{
		public HediffCompProperties_ImmaculateConception Props => (HediffCompProperties_ImmaculateConception)props;

		// protected int GestationIntervalDays => Props.gestationIntervalDays;

		// public override string CompLabelInBracketsExtra => GetLabel();

		public override void CompPostTick(ref float severityAdjustment)
		{
			base.CompPostTick(ref severityAdjustment);
			if (!Pawn.IsHashIntervalTick(6000))
			{
				return;
			}
			Pawn pawn = parent.pawn;
			if (pawn.Map == null)
			{
				return;
			}
			if (pawn.Faction != Faction.OfPlayer)
			{
				base.Pawn.health.RemoveHediff(parent);
				return;
			}
			if (parent.Severity >= Props.requiredSeverity)
			{
				GestationUtility.GenerateNewBornPawn(parent.pawn, Props.completeMessage, Props.endogeneTransfer, Props.xenogeneTransfer);
				base.Pawn.health.RemoveHediff(parent);
			}
		}

		// public string GetLabel()
		// {
			// Pawn pawn = parent.pawn;
			// if (pawn.Faction == Faction.OfPlayer && pawn.ageTracker.CurLifeStage.reproductive)
			// {
				// float percent = (float)ticksCounter / (float)(ticksInday * GestationIntervalDays);
				// return percent.ToStringPercent();
			// }
			// return "";
		// }
	}

}