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

	public class HediffComp_GolemGestator : HediffComp
	{
		private readonly int ticksInday = 60000;
		// private readonly int ticksInday = 1500;

		private int ticksCounter = 0;

		private Pawn childOwner = null;

		public HediffCompProperties_GolemGestator Props => (HediffCompProperties_GolemGestator)props;

		protected int GestationIntervalDays => Props.gestationIntervalDays;

		// protected string customString => Props.customString;

		// protected bool produceEggs => Props.produceEggs;

		// protected string eggDef => Props.eggDef;

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
			// if (pawn != childOwner)
			// {
				// base.Pawn.Kill(null, parent);
				// return;
			// }
			if (golem.Map == null)
			{
				return;
			}
			 // || !pawn.ageTracker.CurLifeStage.reproductive
			if (golem.Faction != Faction.OfPlayer)
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
			if (MechanoidizationUtility.HasActiveGene(Props.geneDef, childOwner))
			{
				GenerateNewBornPawn(childOwner, golem, Props.completeMessage, Props.endogeneTransfer);
			}
			base.Pawn.Kill(null, parent);
			ticksCounter = 0;
		}

		public string GetLabel()
		{
			Pawn pawn = parent.pawn;
			// if (Props.isGreenGoo)
			// {
				// float f = (float)asexualFissionCounter / (float)(ticksInday * gestationIntervalDays);
				// return customString + f.ToStringPercent() + " (" + gestationIntervalDays + " days)";
			// }
			if (pawn.Faction == Faction.OfPlayer)
			{
				float percent = (float)ticksCounter / (float)(ticksInday * GestationIntervalDays);
				return percent.ToStringPercent(); // + " (" + GestationIntervalDays + " days)"
			}
			return "";
		}

		public static void GenerateNewBornPawn(Pawn pawn, Pawn spawnPawn, string completeMessage = "WVC_RB_Gene_MechaGestator", bool endogeneTransfer = true)
		{
			Pawn pawnParent = pawn;
			int litterSize = ((pawnParent.RaceProps.litterSizeCurve == null) ? 1 : Mathf.RoundToInt(Rand.ByCurve(pawnParent.RaceProps.litterSizeCurve)));
			if (litterSize < 1)
			{
				litterSize = 1;
			}
			PawnGenerationRequest generateNewBornPawn = new PawnGenerationRequest(pawnParent.kindDef, pawnParent.Faction, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: false, allowDead: false, allowDowned: true, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: true, allowAddictions: true, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, null, null, null, null, null, forceNoIdeo: false, forceNoBackstory: false, forbidAnyTitle: false, forceDead: false, null, null, null, null, null, 0f, DevelopmentalStage.Newborn);
			// Pawn pawnNewBornChild = null;
			for (int i = 0; i < litterSize; i++)
			{
				Pawn pawnNewBornChild = PawnGenerator.GeneratePawn(generateNewBornPawn);
				if (endogeneTransfer)
				{
					for (int numEndogenes = pawnNewBornChild.genes.Endogenes.Count - 1; numEndogenes >= 0; numEndogenes--)
					{
						pawnNewBornChild.genes.RemoveGene(pawnNewBornChild.genes.Endogenes[numEndogenes]);
					}
					List<Gene> list = pawnParent.genes?.Endogenes;
					foreach (Gene item in list)
					{
						pawnNewBornChild.genes?.AddGene(item.def, xenogene: false);
					}
					if (pawnParent.genes?.Xenotype != null)
					{
						pawnNewBornChild.genes?.SetXenotype(pawnParent.genes?.Xenotype);
					}
				}
				if (PawnUtility.TrySpawnHatchedOrBornPawn(pawnNewBornChild, spawnPawn))
				{
					if (pawnNewBornChild.playerSettings != null && pawnParent.playerSettings != null)
					{
						pawnNewBornChild.playerSettings.AreaRestriction = pawnParent.playerSettings.AreaRestriction;
					}
					if (pawnNewBornChild.RaceProps.IsFlesh)
					{
						pawnNewBornChild.relations.AddDirectRelation(PawnRelationDefOf.Parent, pawnParent);
					}
					if (pawnParent.Spawned)
					{
						pawnParent.GetLord()?.AddPawn(pawnNewBornChild);
					}
				}
				else
				{
					Find.WorldPawns.PassToWorld(pawnNewBornChild, PawnDiscardDecideMode.Discard);
				}
				TaleRecorder.RecordTale(TaleDefOf.GaveBirth, pawnParent, pawn);
			}
			if (spawnPawn.Spawned)
			{
				FilthMaker.TryMakeFilth(spawnPawn.Position, spawnPawn.Map, ThingDefOf.Filth_AmnioticFluid, spawnPawn.LabelIndefinite(), 5);
				if (spawnPawn.caller != null)
				{
					spawnPawn.caller.DoCall();
				}
				if (pawn.caller != null)
				{
					pawn.caller.DoCall();
				}
			}
			Messages.Message(completeMessage.Translate(pawn.LabelIndefinite().CapitalizeFirst()), pawn, MessageTypeDefOf.PositiveEvent);
		}
	}

}
