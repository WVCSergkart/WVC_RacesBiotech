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

	public class HediffComp_Gestator : HediffComp
	{
		private readonly int ticksInday = 60000;

		private int ticksCounter = 0;

		public HediffCompProperties_Gestator Props => (HediffCompProperties_Gestator)props;

		protected int GestationIntervalDays => Props.gestationIntervalDays;

		// protected string customString => Props.customString;

		// protected bool produceEggs => Props.produceEggs;

		// protected string eggDef => Props.eggDef;

		public override string CompLabelInBracketsExtra => GetLabel();

		public override void CompExposeData()
		{
			base.CompExposeData();
			Scribe_Values.Look(ref ticksCounter, "asexualFissionCounter", 0);
		}

		public override void CompPostTick(ref float severityAdjustment)
		{
			base.CompPostTick(ref severityAdjustment);
			Pawn pawn = parent.pawn;
			if (pawn.Map == null)
			{
				return;
			}
			if (pawn.Faction != Faction.OfPlayer || !pawn.ageTracker.CurLifeStage.reproductive)
			{
				return;
			}
			ticksCounter++;
			if (ticksCounter < ticksInday * GestationIntervalDays)
			{
				return;
			}
			// if (Props.convertsIntoAnotherDef)
			// {
				// PawnGenerationRequest request = new PawnGenerationRequest(PawnKindDef.Named(Props.newDef), pawn.Faction, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: false, allowDead: true, allowDowned: false, canGeneratePawnRelations: false, mustBeCapableOfViolence: true, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: false, allowPregnant: true);
				// Pawn pawn2 = PawnGenerator.GeneratePawn(request);
				// PawnUtility.TrySpawnHatchedOrBornPawn(pawn2, pawn);
				// Messages.Message(Props.asexualHatchedMessage.Translate(pawn.LabelIndefinite().CapitalizeFirst()), pawn, MessageTypeDefOf.PositiveEvent);
				// asexualFissionCounter = 0;
				// return;
			// }
			GestationUtility.GenerateNewBornPawn(parent.pawn, Props.completeMessage, Props.endogeneTransfer, Props.xenogeneTransfer);
			ticksCounter = 0;
			base.Pawn.health.RemoveHediff(parent);
		}

		public string GetLabel()
		{
			Pawn pawn = parent.pawn;
			// if (Props.isGreenGoo)
			// {
				// float f = (float)asexualFissionCounter / (float)(ticksInday * gestationIntervalDays);
				// return customString + f.ToStringPercent() + " (" + gestationIntervalDays + " days)";
			// }
			if (pawn.Faction == Faction.OfPlayer && pawn.ageTracker.CurLifeStage.reproductive)
			{
				float percent = (float)ticksCounter / (float)(ticksInday * GestationIntervalDays);
				return percent.ToStringPercent(); // + " (" + GestationIntervalDays + " days)"
			}
			return "";
		}
	}

}
