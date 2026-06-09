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

		private static List<GeneDef> cachedGeneDefs;
		private static int lastCachedResearch = -1;
		private static int lastMessageTick = -1;

		private void ResearchXenotype(ref string phase)
		{
			if (pawn.Faction != Faction.OfPlayer || Energyshifter == null)
			{
				return;
			}
			phase = "cache genes and do effects";
			if (cachedGeneDefs == null || lastCachedResearch < Find.TickManager.TicksGame)
			{
				if (cachedGeneDefs != null)
				{
					EffectsUtility.PulseEffect(pawn);
				}
				cachedGeneDefs = new();
				//foreach (Pawn spawnedPawn in PawnsFinder.AllMaps_Spawned)
				//{
				//	if (spawnedPawn.IsHuman()) //  && spawnedPawn.Map == pawn.MapHeld
				//	{
				//		cachedGeneDefs.AddRangeSafe(spawnedPawn.genes?.GenesListForReading?.ConvertToDefs());
				//	}
				//}
				phase = "find maps";
				foreach (Map map in Find.Maps)
				{
					phase = "add all pawns genes";
					foreach (Pawn targetPawn in map.mapPawns.AllPawns)
					{
						if (targetPawn.IsHuman())
						{
							phase = "add genes from pawn";
							AddGenesFromPawn(targetPawn);
						}
					}
					phase = "add all corpses genes";
					foreach (Thing thing in map.spawnedThings)
					{
						if (thing is Corpse corpse && corpse.InnerPawn != null && corpse.InnerPawn.IsHuman())
						{
							phase = "add genes from corpse";
							AddGenesFromPawn(corpse.InnerPawn);
						}
					}
				}
				lastCachedResearch = 60000 + Find.TickManager.TicksGame;
			}
			cachedGeneDefs.Shuffle();
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
					if (xenotypeHolder.genes.Empty() || XaG_GeneUtility.GenesIsMatch(Energyshifter.CollectedGenes, xenotypeHolder.genes, WVC_Biotech.settings.shapeshifer_reqMinBaseGenesMatch))
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
				if (PawnUtility.ShouldSendNotificationAbout(pawn) && pawn.Spawned && (lastMessageTick < Find.TickManager.TicksGame || newXenotypeName != null))
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
					lastMessageTick = 2500 + Find.TickManager.TicksGame;
				}
			}

			static void AddGenesFromPawn(Pawn targetPawn)
			{
				List<GeneDef> list = new();
				foreach (Gene gene in targetPawn.genes.GenesListForReading)
				{
					list.Add(gene.def);
					if (gene is Gene_Energyshifter gene_Energyshifter)
					{
						list.AddRange(gene_Energyshifter.CollectedGenes);
					}
				}
				cachedGeneDefs.AddRangeSafe(list);
			}
		}

	}

}
