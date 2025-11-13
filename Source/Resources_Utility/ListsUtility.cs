using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public static class ListsUtility
	{

		public static List<Pawn> AllPlayerPawns_MapsOrCaravans_Alive
		{
			get
			{
				return PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive?.Where((pawn) => pawn.Faction == Faction.OfPlayer)?.ToList() ?? new();
			}
		}

		private static List<XenotypeDef> cachedChimerkinXenotypes;
		public static List<XenotypeDef> ChimeraXenotypes
		{
			get
			{
				if (cachedChimerkinXenotypes == null)
				{
					cachedChimerkinXenotypes = ListsUtility.GetAllXenotypesExceptAndroids().Where((xenotypeDef) => xenotypeDef.inheritable && xenotypeDef?.genes?.Any((geneDef) => geneDef.IsGeneDefOfType<Gene_Chimera>()) == true).ToList();
				}
				return cachedChimerkinXenotypes;
			}
		}

		private static List<GeneDef> cachedHivemindGenes;
		public static List<GeneDef> HivemindGenes
		{
			get
			{
				if (cachedHivemindGenes == null)
				{
					List<GeneDef> list = new();
					List<GeneDef> dataBase = DefDatabase<GeneDef>.AllDefsListForReading;
					foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
					{
						if (item.hivemindGenes.NullOrEmpty())
						{
							continue;
						}
						foreach (GeneDef geneDef in dataBase)
						{
							if (item.hivemindGenes.Contains(geneDef.defName))
							{
								list.Add(geneDef);
							}
						}
					}
					cachedHivemindGenes = list;
				}
				return cachedHivemindGenes;
			}
		}

		private static List<Type> cachedHivemindGeneTypes;
		public static List<Type> HivemindGeneTypes
		{
			get
			{
				if (cachedHivemindGeneTypes == null)
				{
					List<Type> list = new();
					foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
					{
						if (item.hivemindGeneTypes.NullOrEmpty())
						{
							continue;
						}
						list.AddRangeSafe(item.hivemindGeneTypes);
					}
					cachedHivemindGeneTypes = list;
				}
				return cachedHivemindGeneTypes;
			}
		}

		//[Obsolete]
		//public static List<XaG_CountWithChance> GetIdenticalGeneDefs()
		//{
		//	List<XaG_CountWithChance> list = new();
		//	foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
		//	{
		//		if (item.identicalGeneDefs.NullOrEmpty())
		//		{
		//			continue;
		//		}
		//		list.AddRange(item.identicalGeneDefs);
		//	}
		//	return list;
		//}

		private static List<GeneDef> cachedBasicGeneDefs;
		public static List<GeneDef> GetHumanGeneDefs()
		{
			if (cachedBasicGeneDefs != null)
			{
				return cachedBasicGeneDefs;
			}
			List<GeneDef> list = new();
			foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
			{
				if (item.humanGeneDefs.NullOrEmpty())
				{
					continue;
				}
				list.AddRange(item.humanGeneDefs);
			}
			cachedBasicGeneDefs = list;
			return list;
		}

		public static List<BackstoryDef> GetBlackListedBackstoryForChanger()
		{
			List<BackstoryDef> list = new();
			foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
			{
				if (item.blackListedBackstoryForChanger.NullOrEmpty())
				{
					continue;
				}
				list.AddRange(item.blackListedBackstoryForChanger);
			}
			return list;
		}

		//public static List<string> GetWhiteListedXenotypesForFilter()
		//{
		//	List<string> whiteListedXenotypesFromDef = new();
		//	foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
		//	{
		//		if (item.whiteListedXenotypesForFilter.NullOrEmpty())
		//		{
		//			continue;
		//		}
		//		whiteListedXenotypesFromDef.AddRange(item.whiteListedXenotypesForFilter);
		//	}
		//	return whiteListedXenotypesFromDef;
		//}

		private static List<string> GetBlackListedXenotypesForSerums(bool addFromFilter = true)
		{
			List<string> list = new();
			foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
			{
				if (item.blackListedXenotypesForSerums.NullOrEmpty())
				{
					continue;
				}
				list.AddRange(item.blackListedXenotypesForSerums);
			}
			if (addFromFilter)
			{
				//if (WVC_Biotech.cachedXenotypesFilter.NullOrEmpty())
				//{
				//	return list;
				//}
				InitialUtility.SetValues();
				// WVC_Biotech.settings.Write();
				foreach (var item in WVC_Biotech.cachedXenotypesFilter)
				{
					if (item.Value == false)
					{
						list.Add(item.Key);
					}
				}
			}
			return list;
		}

		public static List<XenotypeDef> GetWhiteListedXenotypes(bool addFromFilter = false, bool addFromResurrectorFilter = false)
		{
			List<string> filterList = GetBlackListedXenotypesForSerums(addFromFilter);
			// foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
			// {
			// blackListedXenotypesForSerums.AddRange(item.blackListedXenotypesForSerums);
			// }
			List<XenotypeDef> allXenotypesList = GetAllXenotypesExceptAndroids();
			List<XenotypeDef> list = new();
			for (int i = 0; i < allXenotypesList.Count; i++)
			{
				if (!filterList.Contains(allXenotypesList[i].defName))
				{
					list.Add(allXenotypesList[i]);
				}
			}
			if (addFromResurrectorFilter)
			{
				foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
				{
					if (item.whiteListedXenotypesForResurrectorSerums.NullOrEmpty())
					{
						continue;
					}
					list.AddRange(item.whiteListedXenotypesForResurrectorSerums);
				}
			}
			return list;
		}

		public static List<XenotypeHolder> GetWhiteListedXenotypesHolders(bool addFromFilter = false, bool addFromResurrectorFilter = false)
		{
			List<XenotypeHolder> xenotypeHolders = GetAllXenotypesHolders();
			if (xenotypeHolders.NullOrEmpty())
			{
				return new();
			}
			List<XenotypeDef> xenotypes = GetWhiteListedXenotypes(addFromFilter, addFromResurrectorFilter);
			if (xenotypes.NullOrEmpty())
			{
				return xenotypeHolders;
			}
			return xenotypeHolders.Where((XenotypeHolder holder) => holder.CustomXenotype || xenotypes.Contains(holder.xenotypeDef)).ToList();
		}

		public static List<XenotypeHolder> GetAllThrallHolders()
		{
			List<XenotypeHolder> list = new();
			foreach (ThrallDef item in DefDatabase<ThrallDef>.AllDefsListForReading)
			{
				ThrallHolder newHolder = new();
				newHolder.xenotypeDef = XenotypeDefOf.Baseliner;
				newHolder.iconDef = item.xenotypeIconDef;
				newHolder.thrallDef = item;
				newHolder.name = item.label;
				newHolder.genes = item.genes;
				list.Add(newHolder);
			}
			return list;
		}

		//private static List<XenotypeHolder> cachedXenotypeHoldersWithXenotypeDefs;
		public static List<XenotypeHolder> GetAllXenotypesHolders()
		{
			List<XenotypeHolder> list = new();
			foreach (XenotypeDef item in GetAllXenotypesExceptAndroids())
			{
				//if (!StaticCollectionsClass.currentGameComponent.HasKnownXenotype(item))
				//{
				//	continue;
				//}
				XenotypeHolder newHolder = new();
				if (item == XenotypeDefOf.Baseliner || item.genes.NullOrEmpty())
				{
					newHolder.shouldSkip = true;
				}
				newHolder.xenotypeDef = item;
				newHolder.genes = item.genes;
				newHolder.displayPriority = item.displayPriority;
				newHolder.inheritable = item.inheritable;
				list.Add(newHolder);
			}
			foreach (CustomXenotype item in GetCustomXenotypesList())
			{
				//if (!StaticCollectionsClass.currentGameComponent.HasKnownXenotype(item))
				//{
				//	continue;
				//}
				XenotypeHolder newHolder = new();
				if (item.genes.NullOrEmpty())
				{
					newHolder.shouldSkip = true;
				}
				newHolder.name = item.fileName;
				newHolder.iconDef = item.iconDef;
				newHolder.genes = item.genes;
				newHolder.xenotypeDef = XenotypeDefOf.Baseliner;
				newHolder.displayPriority = -1 * (10000 + list.Count);
				newHolder.inheritable = item.inheritable;
				list.Add(newHolder);
			}
			return list;
		}

		private static List<XenotypeDef> allDevXenotypeDefs = null;
		private static List<XenotypeDef> GetDevXenotypeDefs()
		{
			if (allDevXenotypeDefs == null)
			{
				allDevXenotypeDefs = new();
				foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
				{
					if (item.devXenotypeDefs.NullOrEmpty())
					{
						continue;
					}
					allDevXenotypeDefs.AddRange(item.devXenotypeDefs);
				}
			}
			return allDevXenotypeDefs;
		}

		private static List<XenotypeDef> cachedAllXenotypesExceptAndroids;
		public static List<XenotypeDef> GetAllXenotypesExceptAndroids()
		{
			if (cachedAllXenotypesExceptAndroids != null)
			{
				return cachedAllXenotypesExceptAndroids;
			}
			List<XenotypeDef> list = new();
			List<XenotypeDef> devXenotypes = GetDevXenotypeDefs();
			bool gameStarted = MiscUtility.GameStarted();
			foreach (XenotypeDef item in DefDatabase<XenotypeDef>.AllDefsListForReading)
			{
				if (devXenotypes.Contains(item))
				{
					continue;
				}
				if (gameStarted && item.Icon == null)
				{
					Log.Error("Failed find xenotype icon for mod " + (item.modContentPack?.ModMetaData?.Name).ToString() + ". Contact the " + (item.modContentPack?.ModMetaData?.AuthorsString).ToString() + ". " + item.defName + " skipped.");
					continue;
				}
				if (!XaG_GeneUtility.IsAndroid(item))
				{
					list.Add(item);
				}
			}
			if (gameStarted)
			{
				cachedAllXenotypesExceptAndroids = list;
			}
			return list;
		}

		public static List<GeneDef> GetAnomalyExceptions()
		{
			List<GeneDef> list = new();
			foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
			{
				if (item.anomalyXenoGenesExceptions.NullOrEmpty())
				{
					continue;
				}
				list.AddRange(item.anomalyXenoGenesExceptions);
			}
			return list;
		}

		public static List<MutantDef> GetMutantsExceptions()
		{
			List<MutantDef> list = new();
			foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
			{
				if (item.xenoGenesMutantsExceptions.NullOrEmpty())
				{
					continue;
				}
				list.AddRange(item.xenoGenesMutantsExceptions);
			}
			return list;
		}

		public static List<GauranlenTreeModeDef> GetGauranlenTreeModeDefExceptions()
		{
			List<GauranlenTreeModeDef> list = new();
			foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
			{
				if (item.ignoredGauranlenTreeModeDefs.NullOrEmpty())
				{
					continue;
				}
				list.AddRange(item.ignoredGauranlenTreeModeDefs);
			}
			return list;
		}

		public static List<GolemModeDef> GetAllGolemModeDefs()
		{
			List<GolemModeDef> list = new();
			foreach (GolemModeDef item in DefDatabase<GolemModeDef>.AllDefsListForReading)
			{
				if (!item.canBeAnimated)
				{
					continue;
				}
				list.Add(item);
			}
			return list;
		}

		public static List<PawnKindDef> GetAllSummonableGolems()
		{
			List<PawnKindDef> list = new();
			foreach (GolemModeDef item in DefDatabase<GolemModeDef>.AllDefsListForReading)
			{
				if (!item.CanBeSummoned)
				{
					continue;
				}
				list.Add(item.pawnKindDef);
			}
			return list;
		}

		//public static List<Thing> GetAllStoneChunksOnMap(Map map, Pawn pawn)
		//{
		//	List<Thing> list = new();
		//	List<Thing> mapThings = map.listerThings.AllThings;
		//	foreach (Thing item in mapThings)
		//	{
		//		if (item.def.thingCategories.NullOrEmpty() || !item.def.thingCategories.Contains(ThingCategoryDefOf.StoneChunks))
		//		{
		//			continue;
		//		}
		//		if (item.Position.Fogged(item.Map))
		//		{
		//			continue;
		//		}
		//		if (!pawn.CanReserveAndReach(item, PathEndMode.OnCell, pawn.NormalMaxDanger()))
		//		{
		//			continue;
		//		}
		//		list.Add(item);
		//	}
		//	return list;
		//}

		public static List<CustomXenotype> GetCustomXenotypesList()
		{
			List<CustomXenotype> xenotypes = new();
			foreach (FileInfo item in GenFilePaths.AllCustomXenotypeFiles.OrderBy((FileInfo f) => f.LastWriteTime))
			{
				string filePath = GenFilePaths.AbsFilePathForXenotype(Path.GetFileNameWithoutExtension(item.Name));
				PreLoadUtility.CheckVersionAndLoad(filePath, ScribeMetaHeaderUtility.ScribeHeaderMode.Xenotype, delegate
				{
					if (GameDataSaveLoader.TryLoadXenotype(filePath, out var xenotype))
					{
						if (!XaG_GeneUtility.IsAndroid(xenotype))
						{
							xenotypes.Add(xenotype);
						}
					}
				}, skipOnMismatch: true);
			}
			return xenotypes;
		}

		// public static List<GeneDef> GetShapeshifterHeritableGenes()
		// {
		// List<GeneDef> list = new();
		// foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
		// {
		// if (item.shapeshifterHeritableGenes.NullOrEmpty())
		// {
		// continue;
		// }
		// list.AddRange(item.shapeshifterHeritableGenes);
		// }
		// return list;
		// }

	}
}
