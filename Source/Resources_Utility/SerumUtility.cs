using RimWorld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public static class SerumUtility
	{

		// Ideology Hook

		public static void PostSerumUsedHook(Pawn pawn)
		{
			if (ModLister.IdeologyInstalled)
			{
				Find.HistoryEventsManager.RecordEvent(new HistoryEvent(WVC_GenesDefOf.WVC_XenotypeSerumUsed, pawn.Named(HistoryEventArgsNames.Doer)));
			}
		}

		// Checks

		// Basic check excluding all non-humans
		// For example, GeneRestoration uses exactly this
		[Obsolete]
		public static bool PawnIsHuman(Pawn pawn)
		{
			return pawn.IsHuman();
		}

		public static bool IsHuman(this Pawn pawn)
		{
			if (pawn?.RaceProps?.Humanlike != true || pawn.IsAndroid())
			{
				return false;
			}
			return true;
		}

		public static bool IsAnomaly(this Pawn pawn)
		{
			if (ModsConfig.AnomalyActive)
			{
				if (pawn.IsMutant)
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsMutantOfDef(this Pawn pawn, MutantDef mutantDef)
		{
			if (mutantDef == null || pawn.mutant == null)
			{
				return false;
			}
			return mutantDef == pawn.mutant.Def;
		}

		// HumanityCheck is serums use only
		// I already forgot why it is separate
		public static bool HumanityCheck(Pawn pawn)
		{
			if (!IsHuman(pawn))
			{
				pawn.health.AddHediff(WVC_GenesDefOf.WVC_IncompatibilityComa);
				Messages.Message("WVC_PawnIsAndroidCheck".Translate(), pawn, MessageTypeDefOf.RejectInput, historical: false);
				return true;
			}
			return false;
		}

		public static bool XenotypeHasArchites(XenotypeDef xenotypeDef)
		{
			List<GeneDef> genesListForReading = xenotypeDef.genes;
			for (int i = 0; i < genesListForReading.Count; i++)
			{
				if (genesListForReading[i].biostatArc > 0)
				{
					return true;
				}
			}
			return false;
		}

		// ============================================

		public static void XenotypeSerum(Pawn pawn, List<string> blackListedXenotypes, XenotypeDef xenotypeDef, bool removeEndogenes, bool removeXenogenes)
		{
			XenotypeDef xenotype = DefDatabase<XenotypeDef>.AllDefsListForReading.Where((XenotypeDef randomXenotypeDef) => randomXenotypeDef != pawn.genes.Xenotype && !blackListedXenotypes.Contains(randomXenotypeDef.defName)).RandomElement();
			if (xenotypeDef != null)
			{
				xenotype = xenotypeDef;
			}
			// ReimplanterUtility.SetXenotype(pawn, xenotype);
			Pawn_GeneTracker genes = pawn.genes;
			if (removeXenogenes || xenotypeDef == null)
			{
				for (int numXenogenes = genes.Xenogenes.Count - 1; numXenogenes >= 0; numXenogenes--)
				{
					pawn.genes?.RemoveGene(genes.Xenogenes[numXenogenes]);
				}
			}
			if (removeEndogenes || xenotype.inheritable)
			{
				for (int numEndogenes = genes.Endogenes.Count - 1; numEndogenes >= 0; numEndogenes--)
				{
					pawn.genes?.RemoveGene(genes.Endogenes[numEndogenes]);
				}
			}
			pawn.genes?.SetXenotypeDirect(xenotype);
			bool xenotypeHasSkinColor = false;
			bool xenotypeHasHairColor = false;
			for (int i = 0; i < xenotype.genes.Count; i++)
			{
				pawn.genes?.AddGene(xenotype.genes[i], !xenotype.inheritable);
				if (xenotype.genes[i].skinColorBase != null || xenotype.genes[i].skinColorOverride != null)
				{
					xenotypeHasSkinColor = true;
				}
				if (xenotype.genes[i].hairColorOverride != null)
				{
					xenotypeHasHairColor = true;
				}
			}
			if (xenotype.inheritable && !xenotypeHasSkinColor || xenotype == XenotypeDefOf.Baseliner)
			{
				pawn.genes?.AddGene(WVC_GenesDefOf.Skin_SheerWhite, xenogene: false);
			}
			if (xenotype.inheritable && !xenotypeHasHairColor || xenotype == XenotypeDefOf.Baseliner)
			{
				pawn.genes?.AddGene(WVC_GenesDefOf.Hair_SnowWhite, xenogene: false);
			}
		}

		public static void HybridXenotypeSerum(Pawn pawn, List<string> blackListedXenotypes, XenotypeDef xenotypeDef)
		{
			// Set random xenotype
			Pawn_GeneTracker genes = pawn.genes;
			XenotypeDef endoXenotype = DefDatabase<XenotypeDef>.AllDefsListForReading.Where((XenotypeDef randomXenotypeDef) => randomXenotypeDef != pawn.genes.Xenotype && !blackListedXenotypes.Contains(randomXenotypeDef.defName) && randomXenotypeDef.inheritable).RandomElement();
			XenotypeDef xenoXenotype = DefDatabase<XenotypeDef>.AllDefsListForReading.Where((XenotypeDef randomXenotypeDef) => randomXenotypeDef != pawn.genes.Xenotype && !blackListedXenotypes.Contains(randomXenotypeDef.defName) && !randomXenotypeDef.inheritable).RandomElement();
			// Check props
			if (xenotypeDef != null && xenotypeDef.inheritable)
			{
				endoXenotype = xenotypeDef;
			}
			if (xenotypeDef != null && !xenotypeDef.inheritable)
			{
				xenoXenotype = xenotypeDef;
			}
			// remove all genes
			for (int numXenogenes = genes.Xenogenes.Count - 1; numXenogenes >= 0; numXenogenes--)
			{
				pawn.genes?.RemoveGene(genes.Xenogenes[numXenogenes]);
			}
			for (int numEndogenes = genes.Endogenes.Count - 1; numEndogenes >= 0; numEndogenes--)
			{
				pawn.genes?.RemoveGene(genes.Endogenes[numEndogenes]);
			}
			// Add genes
			pawn.genes?.SetXenotypeDirect(xenoXenotype);
			bool xenotypeHasSkinColor = false;
			bool xenotypeHasHairColor = false;
			for (int i = 0; i < endoXenotype.genes.Count; i++)
			{
				pawn.genes?.AddGene(endoXenotype.genes[i], xenogene: false);
				if (endoXenotype.genes[i].skinColorBase != null || endoXenotype.genes[i].skinColorOverride != null)
				{
					xenotypeHasSkinColor = true;
				}
				if (endoXenotype.genes[i].hairColorOverride != null)
				{
					xenotypeHasHairColor = true;
				}
			}
			if (!xenotypeHasSkinColor || endoXenotype == XenotypeDefOf.Baseliner)
			{
				pawn.genes?.AddGene(WVC_GenesDefOf.Skin_SheerWhite, xenogene: false);
			}
			if (!xenotypeHasHairColor || endoXenotype == XenotypeDefOf.Baseliner)
			{
				pawn.genes?.AddGene(WVC_GenesDefOf.Hair_SnowWhite, xenogene: false);
			}
			for (int i = 0; i < xenoXenotype.genes.Count; i++)
			{
				pawn.genes?.AddGene(xenoXenotype.genes[i], xenogene: true);
			}
		}

		public static void CustomXenotypeSerum(Pawn pawn, List<string> blackListedXenotypes)
		{
			// Search for custom xenotypes || Ищем кастомные ксенотипы
			List<CustomXenotype> xenotypes = CustomXenotypesList();
			// Проверяме есть ли ксенотипы. Если нету даём рандомный
			if (xenotypes.Count > 0)
			{
				// Назначаем картинку и название ксенотипа, а так же создаём его переменную
				CustomXenotype customXenotype = xenotypes.RandomElement();
				pawn.genes.xenotypeName = customXenotype.name;
				pawn.genes.iconDef = customXenotype.IconDef;
				Pawn_GeneTracker genes = pawn.genes;
				// Remove Xenogenes
				for (int numXenogenes = genes.Xenogenes.Count - 1; numXenogenes >= 0; numXenogenes--)
				{
					pawn.genes?.RemoveGene(genes.Xenogenes[numXenogenes]);
				}
				// Remove Endogenes
				if (customXenotype.inheritable)
				{
					for (int numEndogenes = genes.Endogenes.Count - 1; numEndogenes >= 0; numEndogenes--)
					{
						pawn.genes?.RemoveGene(genes.Endogenes[numEndogenes]);
					}
				}
				// Добавляем флаги для цветов
				bool xenotypeHasSkinColor = false;
				bool xenotypeHasHairColor = false;
				for (int i = 0; i < customXenotype.genes.Count; i++)
				{
					// Добавляем гены ксенотипа
					pawn.genes?.AddGene(customXenotype.genes[i], !customXenotype.inheritable);
					// Проверяме есть ли гены с цветами кожи и волос
					if (customXenotype.genes[i].skinColorBase != null || customXenotype.genes[i].skinColorOverride != null)
					{
						xenotypeHasSkinColor = true;
					}
					if (customXenotype.genes[i].hairColorOverride != null)
					{
						xenotypeHasHairColor = true;
					}
				}
				// Проверяме флаги и наследуется ли ксенотип. Если ксенотип наследуется но не имеет цвета, то добавляем свой
				if (customXenotype.inheritable && !xenotypeHasSkinColor)
				{
					pawn.genes?.AddGene(WVC_GenesDefOf.Skin_SheerWhite, xenogene: false);
				}
				if (customXenotype.inheritable && !xenotypeHasHairColor)
				{
					pawn.genes?.AddGene(WVC_GenesDefOf.Hair_SnowWhite, xenogene: false);
				}
			}
			else
			{
				// Используем стандартный тип сыворотки, чтоб был хоть какой-то эффект
				XenotypeSerum(pawn, blackListedXenotypes, null, false, true);
			}
		}

		public static void CustomHybridXenotypeSerum(Pawn pawn, List<string> blackListedXenotypes)
		{
			List<CustomXenotype> xenotypes = CustomXenotypesList();
			CustomXenotype endoCustomXenotype = xenotypes.Where((CustomXenotype randomXenotypeDef) => randomXenotypeDef.name != pawn.genes.xenotypeName && randomXenotypeDef.inheritable).RandomElement();
			CustomXenotype xenoCustomXenotype = xenotypes.Where((CustomXenotype randomXenotypeDef) => randomXenotypeDef.name != pawn.genes.xenotypeName && !randomXenotypeDef.inheritable).RandomElement();
			// if (xenotypes.Count > 1)
			if (endoCustomXenotype != null && xenoCustomXenotype != null)
			{
				// CustomXenotype customXenotype = xenotypes.RandomElement();
				// CustomXenotype xenoCustomXenotype = xenotypes.RandomElement();
				pawn.genes.xenotypeName = xenoCustomXenotype.name;
				pawn.genes.iconDef = xenoCustomXenotype.IconDef;
				Pawn_GeneTracker genes = pawn.genes;
				for (int numXenogenes = genes.Xenogenes.Count - 1; numXenogenes >= 0; numXenogenes--)
				{
					pawn.genes?.RemoveGene(genes.Xenogenes[numXenogenes]);
				}
				for (int numEndogenes = genes.Endogenes.Count - 1; numEndogenes >= 0; numEndogenes--)
				{
					pawn.genes?.RemoveGene(genes.Endogenes[numEndogenes]);
				}
				bool xenotypeHasSkinColor = false;
				bool xenotypeHasHairColor = false;
				for (int i = 0; i < endoCustomXenotype.genes.Count; i++)
				{
					pawn.genes?.AddGene(endoCustomXenotype.genes[i], xenogene: false);
					if (endoCustomXenotype.genes[i].skinColorBase != null || endoCustomXenotype.genes[i].skinColorOverride != null)
					{
						xenotypeHasSkinColor = true;
					}
					if (endoCustomXenotype.genes[i].hairColorOverride != null)
					{
						xenotypeHasHairColor = true;
					}
				}
				if (!xenotypeHasSkinColor)
				{
					pawn.genes?.AddGene(WVC_GenesDefOf.Skin_SheerWhite, xenogene: false);
				}
				if (!xenotypeHasHairColor)
				{
					pawn.genes?.AddGene(WVC_GenesDefOf.Hair_SnowWhite, xenogene: false);
				}
				for (int i = 0; i < xenoCustomXenotype.genes.Count; i++)
				{
					pawn.genes?.AddGene(xenoCustomXenotype.genes[i], xenogene: true);
				}
			}
			else
			{
				HybridXenotypeSerum(pawn, blackListedXenotypes, null);
			}
		}

		// ============================================

		public static List<CustomXenotype> CustomXenotypesList()
		{
			List<CustomXenotype> xenotypes = new();
			// Используем ванильные методы загрузки и создания списка ксенотипов
			foreach (FileInfo item in GenFilePaths.AllCustomXenotypeFiles.OrderBy((FileInfo f) => f.LastWriteTime))
			{
				// Присваеваем путь
				string filePath = GenFilePaths.AbsFilePathForXenotype(Path.GetFileNameWithoutExtension(item.Name));
				// Чекаем версию и список модов. Скипаем несоответствия
				PreLoadUtility.CheckVersionAndLoad(filePath, ScribeMetaHeaderUtility.ScribeHeaderMode.Xenotype, delegate
				{
					if (GameDataSaveLoader.TryLoadXenotype(filePath, out var xenotype))
					{
						if (!CustomXenotypeIsAndroid(xenotype))
						{
							xenotypes.Add(xenotype);
						}
					}
				}, skipOnMismatch: true);
			}
			return xenotypes;
		}

		public static bool CustomXenotypeIsAndroid(CustomXenotype xenotype)
		{
			if (xenotype?.genes == null)
			{
				return false;
			}
			List<GeneDef> genesListForReading = xenotype.genes;
			for (int i = 0; i < genesListForReading.Count; i++)
			{
				if (genesListForReading[i].defName.Contains("VREA_SyntheticBody"))
				{
					return true;
				}
			}
			return false;
		}

		// ============================================

		public static void DoubleXenotypeSerum(Pawn pawn, XenotypeDef endotypeDef, XenotypeDef xenotypeDef)
		{
			// remove genes
			Pawn_GeneTracker genes = pawn.genes;
			for (int numXenogenes = genes.Xenogenes.Count - 1; numXenogenes >= 0; numXenogenes--)
			{
				pawn.genes?.RemoveGene(genes.Xenogenes[numXenogenes]);
			}
			for (int numEndogenes = genes.Endogenes.Count - 1; numEndogenes >= 0; numEndogenes--)
			{
				pawn.genes?.RemoveGene(genes.Endogenes[numEndogenes]);
			}
			// Add genes
			pawn.genes?.SetXenotypeDirect(xenotypeDef);
			bool xenotypeHasSkinColor = false;
			bool xenotypeHasHairColor = false;
			for (int i = 0; i < endotypeDef.genes.Count; i++)
			{
				pawn.genes?.AddGene(endotypeDef.genes[i], xenogene: false);
				if (endotypeDef.genes[i].skinColorBase != null || endotypeDef.genes[i].skinColorOverride != null)
				{
					xenotypeHasSkinColor = true;
				}
				if (endotypeDef.genes[i].hairColorOverride != null)
				{
					xenotypeHasHairColor = true;
				}
			}
			if (!xenotypeHasSkinColor || endotypeDef == XenotypeDefOf.Baseliner)
			{
				pawn.genes?.AddGene(WVC_GenesDefOf.Skin_SheerWhite, xenogene: false);
			}
			if (!xenotypeHasHairColor || endotypeDef == XenotypeDefOf.Baseliner)
			{
				pawn.genes?.AddGene(WVC_GenesDefOf.Hair_SnowWhite, xenogene: false);
			}
			for (int i = 0; i < xenotypeDef.genes.Count; i++)
			{
				pawn.genes?.AddGene(xenotypeDef.genes[i], xenogene: true);
			}
		}

	}
}
