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

		// In Dev
		public int ReqProgress
		{
			get
			{
				if (StaticCollectionsClass.oneManArmyMode)
				{
					return 2;
				}
				return 4;
			}
		}
		// In Dev

		public override void PostAdd()
		{
			base.PostAdd();
			if (ModsUtility.GameNotStarted())
			{
				nextTick = new IntRange(1, ReqProgress).RandomInRange;
			}
			else
			{
				nextTick = ReqProgress;
			}
		}

		private int nextTick;
		public override void TickMasterGene(int factorDelayTicks, int outTicks)
		{
			if (nextTick > 0)
			{
				nextTick--;
				return;
			}
			nextTick = ReqProgress;
			//if (!pawn.IsNestedHashIntervalTick(outTicks, 75000))
			//{
			//	return;
			//}
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

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref nextTick, "researchProgress", -1);
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				lastCachedResearch = -1;
				lastMessageTick = -1;
				cachedGeneDefs = null;
			}
		}

		public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
		{
			yield return new StatDrawEntry(StatCategoryDefOf.Genetics, "WVC_XaG_XenoResearcher_Progress".Translate().CapitalizeFirst(), (1f - (nextTick / ReqProgress)).ToStringPercent(), "WVC_XaG_XenoResearcher_ProgressDesc".Translate(), 670);
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
						cachedGeneDefs = null;
						TickMasterGene(1, 1);
					}
				};
			}
		}

		public List<GeneDef> GeneDefs => cachedGeneDefs;

		private static List<GeneDef> cachedGeneDefs;
		private static int lastCachedResearch = -1;
		private static int lastMessageTick = -1;

		private static HashSet<string> cachedUnlockedXenotypesDefs;
		private HashSet<string> cachedUnlockedXenotypesDefs_Local;
		private HashSet<string> UnlockedXenotypesDefs
		{
			get
			{
				if (Energyshifter.LocalMode)
				{
					if (cachedUnlockedXenotypesDefs_Local == null)
					{
						cachedUnlockedXenotypesDefs_Local = [.. Energyshifter.UnlockedXenotypes];
					}
					return cachedUnlockedXenotypesDefs_Local;
				}
				else
				{
					if (cachedUnlockedXenotypesDefs == null)
					{
						cachedUnlockedXenotypesDefs = [.. Energyshifter.UnlockedXenotypes];
					}
					return cachedUnlockedXenotypesDefs;
				}
			}
		}

		private static HashSet<GeneDef> cachedCollectedGeneDefs;
		private HashSet<GeneDef> cachedCollectedGeneDefs_Local;
		private HashSet<GeneDef> CollectedGeneDefs
		{
			get
			{
				if (Energyshifter.LocalMode)
				{
					if (cachedCollectedGeneDefs_Local == null)
					{
						cachedCollectedGeneDefs_Local = [.. Energyshifter.CollectedGenes];
					}
					return cachedCollectedGeneDefs_Local;
				}
				else
				{
					if (cachedCollectedGeneDefs == null)
					{
						cachedCollectedGeneDefs = [.. Energyshifter.CollectedGenes];
					}
					return cachedCollectedGeneDefs;
				}
			}
		}

		public static void ResetCollectedCache()
		{
			cachedCollectedGeneDefs = null;
			cachedUnlockedXenotypesDefs = null;
		}

		private void ResetCollectedCache_Local()
		{
			cachedCollectedGeneDefs_Local = null;
			cachedUnlockedXenotypesDefs_Local = null;
			ResetCollectedCache();
		}

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
				//phase = "add all genes from all dead pawns";
				//foreach (Pawn item in PawnsFinder.All_AliveOrDead)
				//{
				//	if (item.MapHeld != null && item.IsHuman() && item.MapHeld.mapPawns.AllHumanlikeSpawned.Any(humanlike => humanlike.Faction == Faction.OfPlayer && humanlike.genes?.GetFirstGeneOfType<Gene_Energyshifter>() != null))
				//	{
				//		AddGenesFromPawn(item);
				//	}
				//}
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
				if (CollectedGeneDefs.Contains(item))
				{
					continue;
				}
				newGeneDef = item;
			}
			if (newGeneDef != null)
			{
				phase = "reset cached unlocked genes and";
				ResetCollectedCache_Local();
				phase = "unlcok new gene";
				Energyshifter.UnlockGeneDef(newGeneDef);
				string newXenotypeName = null;
				phase = "try get new xenotype";
				List<GeneDef> pawnGenes = [.. CollectedGeneDefs];
				foreach (XenotypeHolder xenotypeHolder in ListsUtility.GetAllXenotypesHolders().Where(xenos => !UnlockedXenotypesDefs.Contains(xenos.Label)))
				{
					phase = "check holder genes: " + xenotypeHolder.Label;
					if (xenotypeHolder.genes.Empty() || XaG_GeneUtility.GenesIsMatch(pawnGenes, xenotypeHolder.genes, WVC_Biotech.settings.shapeshifer_reqMinBaseGenesMatch))
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
					//if (gene is Gene_Energyshifter gene_Energyshifter)
					//{
					//	list.AddRange(gene_Energyshifter.CollectedGenes);
					//}
				}
				cachedGeneDefs.AddRangeSafe(list);
			}
		}

	}

}
