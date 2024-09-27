using RimWorld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{

	public static class ListsUtility
	{

		public static List<XaG_CountWithChance> GetIdenticalGeneDefs()
		{
			List<XaG_CountWithChance> list = new();
			foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
			{
				if (item.identicalGeneDefs.NullOrEmpty())
				{
					continue;
				}
				list.AddRange(item.identicalGeneDefs);
			}
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

		public static List<string> GetWhiteListedXenotypesForFilter()
		{
			List<string> whiteListedXenotypesFromDef = new();
			foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
			{
				if (item.whiteListedXenotypesForFilter.NullOrEmpty())
				{
					continue;
				}
				whiteListedXenotypesFromDef.AddRange(item.whiteListedXenotypesForFilter);
			}
			return whiteListedXenotypesFromDef;
		}

		public static List<string> GetBlackListedXenotypesForSerums(bool addFromFilter = true)
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
				if (WVC_Biotech.cachedXenotypesFilter.NullOrEmpty())
				{
					return list;
				}
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
			List<XenotypeDef> genesListForReading = DefDatabase<XenotypeDef>.AllDefsListForReading;
			List<XenotypeDef> list = new();
			for (int i = 0; i < genesListForReading.Count; i++)
			{
				if (!filterList.Contains(genesListForReading[i].defName))
				{
					list.Add(genesListForReading[i]);
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

		public static List<XenotypeDef> GetAllXenotypesExceptAndroids()
		{
			List<XenotypeDef> list = new();
			foreach (XenotypeDef item in DefDatabase<XenotypeDef>.AllDefsListForReading)
			{
				if (!XaG_GeneUtility.XenotypeIsAndroid(item))
				{
					list.Add(item);
				}
			}
			return list;
		}
		public static List<CustomXenotype> GetAllCustomXenotypesExceptAndroids()
		{
			List<CustomXenotype> list = new();
			foreach (CustomXenotype item in ListsUtility.GetCustomXenotypesList())
			{
				if (!XaG_GeneUtility.XenotypeIsAndroid(item))
				{
					list.Add(item);
				}
			}
			return list;
		}

		public static List<XenotypeDef> GetTrueFormXenotypesFromList(List<XenotypeDef> xenotypes)
		{
			List<XenotypeDef> list = new();
			foreach (XenotypeDef item in xenotypes)
			{
				foreach (GeneDef geneDef in item.genes)
				{
					if (geneDef.geneClass == typeof(Gene_Shapeshift_TrueForm))
					{
						list.Add(item);
						break;
					}
				}
			}
			return list;
		}
		public static List<CustomXenotype> GetTrueFormXenotypesFromList(List<CustomXenotype> xenotypes)
		{
			List<CustomXenotype> list = new();
			foreach (CustomXenotype item in xenotypes)
			{
				foreach (GeneDef geneDef in item.genes)
				{
					if (geneDef.geneClass == typeof(Gene_Shapeshift_TrueForm))
					{
						list.Add(item);
						break;
					}
				}
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

		public static List<PawnKindDef> GetAllGolemPawnkinds()
		{
			List<PawnKindDef> list = new();
			foreach (PawnKindDef item in DefDatabase<PawnKindDef>.AllDefsListForReading)
			{
				if (!item.IsGolemlike())
				{
					continue;
				}
				list.Add(item);
			}
			return list;
		}

		public static List<Thing> GetAllStoneChunksOnMap(Map map, Pawn pawn)
		{
			List<Thing> list = new();
			// Log.Error("0");
			List<Thing> mapThings = map.listerThings.AllThings;
			// Log.Error("1");
			foreach (Thing item in mapThings)
			{
				// Log.Error("2");
				if (item.def.thingCategories.NullOrEmpty() || !item.def.thingCategories.Contains(ThingCategoryDefOf.StoneChunks))
				{
					continue;
				}
				if (item.Position.Fogged(item.Map))
				{
					continue;
				}
				if (!pawn.CanReserveAndReach(item, PathEndMode.OnCell, pawn.NormalMaxDanger()))
				{
					continue;
				}
				// Log.Error("3");
				// Log.Error("4");
				list.Add(item);
			}
			return list;
		}

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
						if (!XaG_GeneUtility.XenotypeIsAndroid(xenotype))
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
