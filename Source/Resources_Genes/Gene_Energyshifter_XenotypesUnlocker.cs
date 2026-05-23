using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using static UnityEngine.GraphicsBuffer;

namespace WVC_XenotypesAndGenes
{
	public class Gene_Energyshifter_XenotypesUnlocker : Gene_Energyshifter_SubGene
	{

		public override void TickMasterGene(int factorDelayTicks, int outTicks)
		{
			string phase = "init";
			try
			{
				ResearchXenotype(ref phase);
			}
			catch (Exception arg)
			{
				Log.Error($"Failed unlock new gene/xenotype. On phase: {phase} Reason: {arg.Message}");
			}
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: TryResearchXenotype",
					action = delegate
					{
						TickMasterGene(1, 1);
					}
				};
			}
		}

		public List<GeneDef> GeneDefs => cachedGeneDefs;

		private List<GeneDef> cachedGeneDefs;
		private int lastCachedResearch = -1;
		private static int lastMessageTick = -1;

		private void ResearchXenotype(ref string phase)
		{
			if (pawn.Faction != Faction.OfPlayer || pawn.MapHeld == null || Energyshifter == null)
			{
				return;
			}
			phase = "cache pawns and do effects";
			if (cachedGeneDefs == null || lastCachedResearch < Find.TickManager.TicksGame)
			{
				if (cachedGeneDefs != null)
				{
					ThoughtUtility.PulseEffect(pawn);
				}
				cachedGeneDefs = new();
				foreach (Pawn spawnedPawn in PawnsFinder.AllMaps_Spawned)
				{
					if (spawnedPawn.IsHuman() && spawnedPawn.Map == pawn.MapHeld)
					{
						cachedGeneDefs.AddRangeSafe(spawnedPawn.genes?.GenesListForReading?.ConvertToDefs());
					}
				}
			}
			GeneDef newGeneDef = null;
			phase = "get new gene";
			foreach (GeneDef item in cachedGeneDefs)
			{
				if (newGeneDef != null)
				{
					break;
				}
				if (Energyshifter.CollectedGenes.Contains(item))
				{
					continue;
				}
				newGeneDef = item;
			}
			if (newGeneDef != null)
			{
				phase = "unlcok new gene";
				Energyshifter.UnlockGeneDef(newGeneDef);
				string newXenotypeName = null;
				phase = "try get new xenotype";
				foreach (XenotypeHolder xenotypeHolder in ListsUtility.GetAllXenotypesHolders().Where(xenos => !Energyshifter.UnlcokedXenotypes.Contains(xenos.Label)))
				{
					phase = "check holder genes: " + xenotypeHolder.Label;
					if (XaG_GeneUtility.HasAllGenes(xenotypeHolder.genes, Energyshifter.CollectedGenes))
					{
						newXenotypeName = xenotypeHolder.Label;
					}
					if (newXenotypeName != null)
					{
						break;
					}
				}
				phase = "unlock researched xenotype";
				if (newXenotypeName != null)
				{
					Energyshifter.UnlockXenotype(newXenotypeName);
				}
				phase = "send message";
				if (PawnUtility.ShouldSendNotificationAbout(pawn) && (lastMessageTick < Find.TickManager.TicksGame || newXenotypeName != null))
				{
					if (newXenotypeName == null)
					{
						Messages.Message("WVC_XaG_GeneResearchedMessage".Translate(Energyshifter.LabelCap, newGeneDef.label), pawn, MessageTypeDefOf.PositiveEvent);
					}
					else
					{
						Messages.Message("WVC_XaG_XenotypeResearchedMessage".Translate(Energyshifter.LabelCap, newXenotypeName), pawn, MessageTypeDefOf.PositiveEvent);
					}
					// Anti-spam tick
					lastMessageTick = 15000 + Find.TickManager.TicksGame;
				}
			}
			phase = "finalize";
			//shouldUpdate = false;
			lastCachedResearch = 60000 + Find.TickManager.TicksGame;
		}
	}

}
