using RimWorld;
using System;
using System.Collections.Generic;
using Verse;


namespace WVC_XenotypesAndGenes
{

	public static class XenotypeFilterUtility
	{

		public static List<BackstoryDef> BlackListedBackstoryForChanger()
		{
			List<BackstoryDef> list = new();
			foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
			{
				list.AddRange(item.blackListedBackstoryForChanger);
			}
			return list;
		}

		public static List<string> WhiteListedXenotypesForFilter()
		{
			List<string> whiteListedXenotypesFromDef = new();
			foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
			{
				whiteListedXenotypesFromDef.AddRange(item.whiteListedXenotypesForFilter);
			}
			return whiteListedXenotypesFromDef;
		}

		public static List<string> BlackListedXenotypesForSerums(bool addFromFilter = true)
		{
			List<string> list = new();
			foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
			{
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

		public static List<XenotypeDef> WhiteListedXenotypes(bool addFromFilter = false, bool addFromResurrectorFilter = false)
		{
			List<string> filterList = BlackListedXenotypesForSerums(addFromFilter);
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
					list.AddRange(item.whiteListedXenotypesForResurrectorSerums);
				}
			}
			return list;
		}

		[Obsolete]
		public static List<HediffDef> HediffsThatPreventUndeadResurrection()
		{
			List<HediffDef> list = new();
			foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
			{
				list.AddRange(item.hediffsThatPreventUndeadResurrection);
			}
			return list;
		}

		// public static List<XenotypeDef> WhiteListedXenotypes_Filtered()
		// {
		// List<string> blackListedXenotypesForSerums = XenotypeFilterUtility.BlackListedXenotypesForSerums();
		// List<XenotypeDef> genesListForReading = DefDatabase<XenotypeDef>.AllDefsListForReading;
		// List<XenotypeDef> whiteListedXenotypes = new();
		// for (int i = 0; i < genesListForReading.Count; i++)
		// {
		// if (!blackListedXenotypesForSerums.Contains(genesListForReading[i].defName))
		// {
		// whiteListedXenotypes.Add(genesListForReading[i]);
		// }
		// }
		// return whiteListedXenotypes;
		// }

		// public static List<XenotypeDef> BlackListedXenotypesForHybridSerums()
		// {
		// List<XenotypeDef> list = new();
		// foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
		// {
		// list.AddRange(item.blackListedXenotypesForHybridSerums);
		// }
		// return list;
		// }

		// public static List<XenotypeDef> BlackListedXenotypesForSingleSerums()
		// {
		// List<XenotypeDef> list = new();
		// foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
		// {
		// list.AddRange(item.blackListedXenotypesForSingleSerums);
		// }
		// return list;
		// }

		public static List<XenotypeDef> AllXenotypesExceptAndroids()
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

	}
}
