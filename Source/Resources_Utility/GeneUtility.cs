using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Grammar;

namespace WVC_XenotypesAndGenes
{

	public static class XaG_GeneUtility
	{

		public static float GetAverageCpx
		{
			get
			{
				List<GeneDef> allDefsListForReading = DefDatabase<GeneDef>.AllDefsListForReading;
				float averageCpx = allDefsListForReading.Sum(gene => gene.biostatCpx) / allDefsListForReading.Count;
				if (averageCpx < 1)
				{
					return 1;
				}
				return averageCpx;
			}
		}

		public static bool ActiveDowned(Pawn pawn, Gene gene)
		{
			return !pawn.Downed || !gene.Active;
		}

		public static bool SelectorFactionMap(Pawn pawn)
		{
			return Find.Selector.SelectedPawns.Count > 1 || FactionMap(pawn);
		}

		public static bool SelectorDraftedFactionMap(Pawn pawn)
		{
			return Find.Selector.SelectedPawns.Count > 1 || pawn.Drafted || FactionMap(pawn);
		}

		public static bool FactionMap(Pawn pawn)
		{
			return pawn.Faction != Faction.OfPlayer || pawn.Map == null;
		}

		public static bool ActiveFactionMap(Pawn pawn, Gene gene)
		{
			return FactionMap(pawn) || !gene.Active;
		}

		public static bool SelectorActiveFactionMap(Pawn pawn, Gene gene)
		{
			return Find.Selector.SelectedPawns.Count > 1 || ActiveFactionMap(pawn, gene);
		}

		public static bool SelectorActiveFactionMapMechanitor(Pawn pawn, Gene gene)
		{
			return pawn.mechanitor == null || SelectorActiveFactionMap(pawn, gene);
		}

		public static bool SelectorDraftedActiveFactionMap(Pawn pawn, Gene gene)
		{
			return Find.Selector.SelectedPawns.Count > 1 || pawn.Drafted || ActiveFactionMap(pawn, gene);
		}

		public static bool SelectorActiveFaction(Pawn pawn, Gene gene)
		{
			return Find.Selector.SelectedPawns.Count > 1 || pawn.Faction != Faction.OfPlayer || !gene.Active;
		}

		public static bool SelectorDraftedActiveFaction(Pawn pawn, Gene gene)
		{
			return Find.Selector.SelectedPawns.Count > 1 || pawn.Drafted || pawn.Faction != Faction.OfPlayer || !gene.Active;
		}

		public static void ResetGenesInspectString(Pawn pawn)
		{
			pawn.HumanComponent()?.ResetInspectString();
		}

		[Obsolete]
		public static void Notify_GenesChanged(Pawn pawn)
		{
			PawnComponentsUtility.AddAndRemoveDynamicComponents(pawn);
			pawn.needs?.AddOrRemoveNeedsAsAppropriate();
			pawn.health?.hediffSet?.DirtyCache();
			pawn.skills?.DirtyAptitudes();
			pawn.Notify_DisabledWorkTypesChanged();
			ResetGenesInspectString(pawn);
		}

		public static bool IsObsolete(this GeneDef geneDef)
		{
			return geneDef.GetModExtension<GeneExtension_Obsolete>() != null;
		}

		public static bool Furskin_ShouldNotDrawNow(Pawn pawn)
		{
			return pawn.DevelopmentalStage != DevelopmentalStage.Adult || (pawn.Drawer?.renderer != null ? pawn.Drawer.renderer.CurRotDrawMode : RotDrawMode.Fresh) == RotDrawMode.Dessicated;
		}

		// Genepacks

		[Obsolete]
		public static void GenerateGenepackName(GeneSet geneSet, RulePackDef rule)
		{
			if (rule == null)
			{
				rule = RulePackDefOf.NamerGenepack;
			}
			if (geneSet.GenesListForReading.Any())
			{
				GrammarRequest request = default;
				request.Includes.Add(rule);
				request.Rules.Add(new Rule_String("geneWord", geneSet.GenesListForReading[0].LabelShortAdj));
				request.Rules.Add(new Rule_String("geneCountMinusOne", (geneSet.GenesListForReading.Count - 1).ToString()));
				request.Constants.Add("geneCount", geneSet.GenesListForReading.Count.ToString());
				Type typeFromHandle = typeof(GeneSet);
				FieldInfo field = typeFromHandle.GetField("name", BindingFlags.Instance | BindingFlags.NonPublic);
				field.SetValue(geneSet, GrammarResolver.Resolve("r_name", request, null, forceLog: false, null, null, null, capitalizeFirstSentence: false));
			}
		}

		[Obsolete]
		public static void SetGenesInPack(GeneralHolder geneCount, GeneSet geneSet)
		{
			List<GeneDef> geneDefs = DefDatabase<GeneDef>.AllDefsListForReading;
			for (int j = 0; j < geneCount.genesCount; j++)
			{
				if (geneDefs.Where((GeneDef x) => x.biostatArc == 0 && CanAddGeneDuringGeneration(x, geneSet, geneCount)).TryRandomElementByWeight((GeneDef x) => x.selectionWeight, out var result))
				{
					geneSet.AddGene(result);
				}
			}
			for (int i = 0; i < geneCount.architeCount; i++)
			{
				if (geneDefs.Where((GeneDef x) => x.biostatArc != 0 && CanAddGeneDuringGeneration(x, geneSet, geneCount)).TryRandomElementByWeight((GeneDef x) => x.selectionWeight, out var result))
				{
					geneSet.AddGene(result);
				}
			}
		}

		[Obsolete]
		public static bool CanAddGeneDuringGeneration(GeneDef gene, GeneSet geneSet, GeneralHolder geneCount)
		{
			if (!gene.IsXenoGenesDef())
			{
				return false;
			}
			if (!gene.canGenerateInGeneSet || gene.selectionWeight <= 0f)
			{
				return false;
			}
			if (geneCount.prerequisitesOnly && gene.prerequisite == null)
			{
				return false;
			}
			if (geneCount.cosmeticOnly && !IsCosmeticGene(gene))
			{
				return false;
			}
			if (!geneCount.allowedGeneCategoryDefs.NullOrEmpty() && !geneCount.allowedGeneCategoryDefs.Contains(gene.displayCategory))
			{
				return false;
			}
			List<GeneDef> genes = geneSet.GenesListForReading;
			if (genes.Contains(gene))
			{
				return false;
			}
			if (genes.Count > 0 && !GeneTuning.BiostatRange.Includes(gene.biostatMet + geneSet.MetabolismTotal))
			{
				return false;
			}
			// if (gene.prerequisite != null && !genes.Contains(gene.prerequisite))
			// {
			// return false;
			// }
			for (int i = 0; i < genes.Count; i++)
			{
				if (gene.ConflictsWith(genes[i]))
				{
					return false;
				}
			}
			return true;
		}

		public static bool IsCosmeticGene(GeneDef gene)
		{
			if (gene.skinColorBase != null || gene.skinColorOverride != null || gene.hairColorOverride != null)
			{
				return true;
			}
			if (gene.soundCall != null || gene.soundDeath != null || gene.soundWounded != null)
			{
				return true;
			}
			if (gene.fur != null || !gene.renderNodeProperties.NullOrEmpty())
			{
				return true;
			}
			if (gene.forcedHair != null || !gene.forcedHeadTypes.NullOrEmpty())
			{
				return true;
			}
			if (gene.hairTagFilter != null || gene.beardTagFilter != null)
			{
				return true;
			}
			if (gene.bodyType.HasValue)
			{
				return true;
			}
			return false;
		}

		// Misc

		//private static GeneDef deathrestGeneDef;
		//public static GeneDef GeneDeathrest
		//{
		//	get
		//	{
		//		if (deathrestGeneDef == null)
		//		{
		//			deathrestGeneDef = DefDatabase<GeneDef>.AllDefsListForReading.Where((GeneDef def) => def.IsVanillaDef() && def.IsGeneDefOfType<Gene_Deathrest>()).FirstOrDefault();
		//		}
		//		return deathrestGeneDef;
		//	}
		//}

		public static void Notify_GenesConflicts(Pawn pawn, GeneDef geneDef, Gene thisGene = null)
		{
			foreach (Gene gene in pawn.genes.GenesListForReading)
			{
				if (gene.def == geneDef)
				{
					continue;
				}
				if (thisGene != null && gene.overriddenByGene != null && gene.overriddenByGene != thisGene)
				{
					continue;
				}
				if (geneDef.ConflictsWith(gene.def))
				{
					gene.OverrideBy(thisGene);
				}
			}
		}

		public static bool TryAddOrRemoveGene(this Pawn pawn, Gene ignoredGene = null, Gene removeGene = null, GeneDef geneDefToAdd = null, bool inheritable = true)
		{
			if (geneDefToAdd != null && (ignoredGene == null || !geneDefToAdd.ConflictsWith(ignoredGene.def)) && (inheritable && !XaG_GeneUtility.HasEndogene(geneDefToAdd, pawn) || !XaG_GeneUtility.HasXenogene(geneDefToAdd, pawn)))
			{
				pawn.genes.AddGene(geneDefToAdd, !inheritable);
				return true;
			}
			if (removeGene != null && (ignoredGene == null || removeGene != ignoredGene))
			{
				pawn.genes.RemoveGene(removeGene);
				return true;
			}
			return false;
		}

		public static bool IsHairGeneDef(GeneDef geneDef)
		{
			return geneDef.hairColorOverride != null;
		}

		public static bool IsSkinGeneDef(GeneDef geneDef)
		{
			return geneDef.skinColorBase != null || geneDef.skinColorOverride != null;
		}

		public static void ImplantChimeraEvolveGeneSet(Pawn pawn, GeneDef geneDef, bool saveOldGeneSet = true)
		{
			//List<GeneDef> removedGenes = geneDef?.GetModExtension<GeneExtension_Undead>()?.removedGenes;
			//List<GeneDef> addedGenes = geneDef?.GetModExtension<GeneExtension_Undead>()?.addedGenes;
			//if (addedGenes == null || removedGenes == null)
			//{
			//	return;
			//}
			//Gene_Chimera chimera = pawn.genes?.GetFirstGeneOfType<Gene_Chimera>();
			//if (chimera == null)
			//         {
			//	return;
			//}
			//foreach (Gene gene in pawn.genes.Endogenes.ToList())
			//{
			//	if (removedGenes.Contains(gene.def))
			//	{
			//		if (saveOldGeneSet)
			//		{
			//			chimera.TryAddGene(gene.def);
			//		}
			//		pawn.genes.RemoveGene(gene);
			//	}
			//}
			//foreach (GeneDef addedGeneDef in addedGenes)
			//{
			//	pawn.genes.AddGene(addedGeneDef, false);
			//}
			//if (pawn.SpawnedOrAnyParentSpawned)
			//{
			//	chimera.DoEffects();
			//	Messages.Message("WVC_XaG_GeneChimera_EntityImplant".Translate(), pawn, MessageTypeDefOf.NeutralEvent, historical: false);
			//}
			ImplantChimeraEvolveGeneSet(pawn, geneDef?.GetModExtension<GeneExtension_Undead>()?.xenotypeDef, saveOldGeneSet);
		}

		public static void AddGene(this Pawn pawn, GeneDef geneDef, bool xenogene, Gene caller = null)
		{
			if (caller != null && caller.def.ConflictsWith(geneDef))
			{
				return;
			}
			if (!xenogene && !XaG_GeneUtility.HasGene(geneDef, pawn) || !XaG_GeneUtility.HasXenogene(geneDef, pawn))
			{
				pawn.genes.AddGene(geneDef, xenogene);
			}
		}

		public static void RemoveGene(this Pawn pawn, Gene gene, Gene caller = null)
		{
			if (gene == caller)
			{
				return;
			}
			pawn.genes.RemoveGene(gene);
		}

		public static void ImplantChimeraEvolveGeneSet(Pawn pawn, XenotypeDef xenotypeDef, bool saveOldGeneSet = true)
		{
			if (xenotypeDef == null)
			{
				return;
			}
			Gene_Chimera chimera = pawn.genes?.GetFirstGeneOfType<Gene_Chimera>();
			if (chimera == null)
			{
				return;
			}
			ReimplanterUtility.SetXenotypeDirect(null, pawn, xenotypeDef);
			foreach (Gene gene in pawn.genes.Endogenes.ToList())
			{
				if (!xenotypeDef.genes.Contains(gene.def))
				{
					if (saveOldGeneSet)
					{
						chimera.TryAddGene(gene.def);
					}
					pawn.genes.RemoveGene(gene);
				}
			}
			foreach (GeneDef addedGeneDef in xenotypeDef.genes)
			{
				pawn.genes.AddGene(addedGeneDef, false);
			}
			if (pawn.SpawnedOrAnyParentSpawned)
			{
				chimera.DoEffects();
				Messages.Message("WVC_XaG_GeneChimera_EntityImplant".Translate(), pawn, MessageTypeDefOf.NeutralEvent, historical: false);
			}
		}

		public static void AddGeneToChimera(Pawn pawn, GeneDef geneDef)
		{
			XaG_GeneUtility.AddGenesToChimera(pawn, new() { geneDef });
			if (PawnUtility.ShouldSendNotificationAbout(pawn))
			{
				Messages.Message("WVC_XaG_GeneGeneticThief_GeneObtained".Translate(pawn.NameShortColored, geneDef.label), pawn, MessageTypeDefOf.NeutralEvent, historical: false);
			}
		}

		public static void AddGenesToChimera(Pawn p, List<GeneDef> chimeraGeneDefs, bool clearGenes = false)
		{
			Gene_Chimera chimera = p.genes?.GetFirstGeneOfType<Gene_Chimera>();
			if (chimera == null)
			{
				return;
			}
			if (clearGenes)
			{
				chimera.Debug_ClearAllGenes();
			}
			foreach (GeneDef geneDef in chimeraGeneDefs)
			{
				if (chimera.TryAddGene(geneDef))
				{
					p.TryAddGeneIfNone(geneDef, true);
				}
			}
			chimera.UpdateMetabolism();
			//ReimplanterUtility.PostImplantDebug(p);
		}

		public static void UpdateXenogermReplication(Pawn pawn, bool addXenogermReplicating = true, IntRange ticksToDisappear = new())
		{
			Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.XenogermReplicating);
			if (firstHediffOfDef != null)
			{
				pawn.health.RemoveHediff(firstHediffOfDef);
			}
			if (addXenogermReplicating)
			{
				Hediff cooldownHediff = HediffMaker.MakeHediff(HediffDefOf.XenogermReplicating, pawn);
				HediffComp_Disappears hediffComp_Disappears = cooldownHediff.TryGetComp<HediffComp_Disappears>();
				int ticks = ticksToDisappear.RandomInRange;
				if (hediffComp_Disappears != null && ticks > 0)
				{
					hediffComp_Disappears.ticksToDisappear = ticks;
				}
				pawn.health.AddHediff(cooldownHediff);
			}
		}

		// Gene Restoration

		public static bool ContainsAll(List<GeneDef> genesToCheck, List<GeneDef> genesContainer)
		{
			foreach (GeneDef item in genesToCheck)
			{
				if (!genesContainer.Contains(item))
				{
					return false;
				}
			}
			return true;
		}

		public static List<GeneDef> ConvertToDefs(this List<Gene> genes)
		{
			List<GeneDef> geneDefs = new();
			if (genes.NullOrEmpty())
			{
				return geneDefs;
			}
			foreach (Gene item in genes)
			{
				if (geneDefs.Contains(item.def))
				{
					continue;
				}
				geneDefs.Add(item.def);
			}
			return geneDefs;
		}

		public static List<XenotypeHolder> ConvertToHolder(this List<XenotypeDef> genes)
		{
			List<XenotypeHolder> geneDefs = new();
			if (genes.NullOrEmpty())
			{
				return geneDefs;
			}
			foreach (XenotypeDef item in genes)
			{
				geneDefs.Add(new(item));
			}
			return geneDefs;
		}

		public static bool AbilityIsGeneAbility(Ability ability)
		{
			List<GeneDef> genes = DefDatabase<GeneDef>.AllDefsListForReading;
			for (int i = 0; i < genes.Count; i++)
			{
				if (!genes[i].abilities.NullOrEmpty() && genes[i].abilities.Contains(ability.def))
				{
					return true;
				}
			}
			return false;
		}

		// ============================= Anti-Bug =============================

		public static void Debug_ImplantAllGenes(Pawn pawn, List<GeneDef> geneDefs)
		{
			foreach (GeneDef geneDef in geneDefs)
			{
				pawn.genes.AddGene(geneDef, true);
			}
		}

		public static XenotypeDef GetRandomXenotypeFromList(List<XenotypeDef> xenotypeDefs, List<XenotypeDef> exclude)
		{
			XenotypeDef xenotypeDef = null;
			if (!xenotypeDefs.NullOrEmpty())
			{
				if (exclude == null)
				{
					exclude = new();
				}
				xenotypeDefs?.Where((XenotypeDef xenos) => !exclude.Contains(xenos))?.TryRandomElement(out xenotypeDef);
			}
			return xenotypeDef;
		}

		public static XenotypeDef GetRandomXenotypeFromXenotypeChances(List<XenotypeChance> xenotypeChances, List<XenotypeDef> exclude)
		{
			XenotypeDef xenotypeDef = null;
			if (!xenotypeChances.NullOrEmpty())
			{
				if (exclude == null)
				{
					exclude = new();
				}
				if (xenotypeChances.Where((XenotypeChance xenos) => !exclude.Contains(xenos.xenotype)).TryRandomElementByWeight((XenotypeChance chance) => chance.chance, out XenotypeChance xenotypeChance))
				{
					xenotypeDef = xenotypeChance.xenotype;
				}
			}
			return xenotypeDef;
		}

		public static bool HasGeneDefOfType<T>(this List<GeneDef> geneDefs)
		{
			if (geneDefs.NullOrEmpty())
			{
				return false;
			}
			for (int i = 0; i < geneDefs.Count; i++)
			{
				if (geneDefs[i].IsGeneDefOfType<T>())
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsGeneDefOfType(this GeneDef geneDef, Type geneClass)
		{
			return geneDef.geneClass == geneClass || geneClass.IsAssignableFrom(geneDef.geneClass);
		}

		public static bool IsGeneDefOfType<T>(this GeneDef geneDef)
		{
			return geneDef.geneClass == typeof(T) || typeof(T).IsAssignableFrom(geneDef.geneClass);
		}

		public static bool ConflictWith(GeneDef geneDef, List<GeneDef> geneDefs)
		{
			foreach (GeneDef item in geneDefs)
			{
				if (item.ConflictsWith(geneDef))
				{
					return true;
				}
			}
			return false;
		}

		public static bool ConflictWith(GeneDef geneDef, List<Gene> geneDefs)
		{
			foreach (Gene item in geneDefs)
			{
				if (item.def.ConflictsWith(geneDef))
				{
					return true;
				}
			}
			return false;
		}

		public static bool TryRemoveAllConflicts(Pawn pawn, GeneDef geneDef, bool xenogene, List<GeneDef> exceptions = null)
		{
			try
			{
				if (xenogene)
				{
					foreach (Gene item in pawn.genes.Xenogenes.ToList())
					{
						RemoveConflictingGene(pawn, geneDef, exceptions, item);
					}
				}
				else
				{
					foreach (Gene item in pawn.genes.Endogenes.ToList())
					{
						RemoveConflictingGene(pawn, geneDef, exceptions, item);
					}
				}
				return true;
			}
			catch
			{
				Log.Error("Failed remove conflict genes from pawn: " + pawn.LabelShort);
			}
			return false;
		}

		private static void RemoveConflictingGene(Pawn pawn, GeneDef geneDef, List<GeneDef> exceptions, Gene item)
		{
			if (!item.def.ConflictsWith(geneDef))
			{
				return;
			}
			if (exceptions != null && exceptions.Contains(item.def))
			{
				return;
			}
			pawn.genes.RemoveGene(item);
		}

		public static bool TryRemoveAllConflicts(Pawn pawn, GeneDef geneDef, List<GeneDef> exceptions = null)
		{
			try
			{
				foreach (Gene item in pawn.genes.GenesListForReading.ToList())
				{
					RemoveConflictingGene(pawn, geneDef, exceptions, item);
				}
				return true;
			}
			catch
			{
				Log.Error("Failed remove conflict genes from pawn: " + pawn.LabelShort);
			}
			return false;
		}

		private static List<GeneDef> cachedAndroidGenes;
		public static List<GeneDef> AndroidGenes
		{
			get
			{
				if (cachedAndroidGenes == null)
				{
					List<GeneDef> list = new();
					List<GeneDef> dataBase = DefDatabase<GeneDef>.AllDefsListForReading;
					foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
					{
						if (item.androidGenes.NullOrEmpty())
						{
							continue;
						}
						foreach (GeneDef androidGene in dataBase)
						{
							if (item.androidGenes.Contains(androidGene.defName))
							{
								list.Add(androidGene);
							}
						}
					}
					cachedAndroidGenes = list;
				}
				return cachedAndroidGenes;
			}
		}

		public static bool IsAndroid(this GeneDef geneDef)
		{
			if (IsAndroidGeneCycly(geneDef))
			{
				return true;
			}
			return false;
		}

		private static bool IsAndroidGeneCycly(GeneDef geneDef, int currentCycle = 0)
		{
			if (AndroidGenes.Contains(geneDef))
			{
				return true;
			}
			if (geneDef.prerequisite == null)
			{
				return false;
			}
			if (currentCycle > 100)
			{
				return false;
			}
			return IsAndroidGeneCycly(geneDef.prerequisite, currentCycle++);
		}

		public static bool IsSubGeneOfThisCycly(GeneDef masterGeneDef, GeneDef subGeneDef, int currentCycle = 0)
		{
			if (subGeneDef?.prerequisite == null)
			{
				return false;
			}
			if (masterGeneDef == subGeneDef || masterGeneDef == subGeneDef.prerequisite)
			{
				return true;
			}
			if (currentCycle > 100)
			{
				return false;
			}
			return IsSubGeneOfThisCycly(masterGeneDef, subGeneDef.prerequisite, currentCycle++);
		}

		public static bool IsAndroid(this Pawn pawn)
		{
			if (pawn?.genes == null || AndroidGenes.Empty())
			{
				return false;
			}
			List<Gene> genesListForReading = pawn.genes.GenesListForReading;
			for (int i = 0; i < genesListForReading.Count; i++)
			{
				//if (genesListForReading[i].def.defName.Contains("VREA_SyntheticBody"))
				//{
				//	return true;
				//}
				if (AndroidGenes.Contains(genesListForReading[i].def))
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsAndroid(this XenotypeDef xenotypeDef)
		{
			if (AndroidGenes.Empty())
			{
				return false;
			}
			List<GeneDef> genesListForReading = xenotypeDef?.genes;
			if (genesListForReading.NullOrEmpty())
			{
				return false;
			}
			for (int i = 0; i < genesListForReading.Count; i++)
			{
				//if (genesListForReading[i].defName.Contains("VREA_SyntheticBody"))
				//{
				//	return true;
				//}
				if (AndroidGenes.Contains(genesListForReading[i]))
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsAndroid(this CustomXenotype xenotypeDef)
		{
			if (AndroidGenes.Empty())
			{
				return false;
			}
			List<GeneDef> genesListForReading = xenotypeDef?.genes;
			if (genesListForReading.NullOrEmpty())
			{
				return false;
			}
			for (int i = 0; i < genesListForReading.Count; i++)
			{
				//if (genesListForReading[i].defName.Contains("VREA_SyntheticBody"))
				//{
				//	return true;
				//}
				if (AndroidGenes.Contains(genesListForReading[i]))
				{
					return true;
				}
			}
			return false;
		}

		public static bool PawnIsBaseliner(Pawn pawn)
		{
			if (pawn?.genes == null)
			{
				return true;
			}
			if (pawn.IsBaseliner())
			{
				if (pawn.genes.UniqueXenotype)
				{
					return false;
				}
				return true;
			}
			return false;
		}

		public static bool TryAddGeneIfNone(this Pawn pawn, GeneDef geneDef, bool xenogene)
		{
			if (HasEndogene(geneDef, pawn))
			{
				return false;
			}
			if (HasXenogene(geneDef, pawn))
			{
				return false;
			}
			pawn.genes.AddGene(geneDef, xenogene);
			return true;
		}

		//public static XenotypeDef GetXenotype(this Pawn pawn, bool xenogene)
		//{
		//	XenotypeDef xenotypeDef = null;
		//	List<Gene> genes;
		//	if (xenogene)
		//	{
		//		genes = pawn.genes.Xenogenes;
		//	}
		//	else
		//	{
		//		genes = pawn.genes.Endogenes;
		//	}
		//	List<XenotypeDef> xenoDataBase = ListsUtility.GetAllXenotypesExceptAndroids();
		//	foreach (XenotypeDef item in xenoDataBase)
		//	{
		//		if (GenesIsMatch(genes, item.genes, 1f))
		//		{
		//			xenotypeDef = item;
		//			break;
		//		}
		//	}
		//	return xenotypeDef;
		//}

		// ============================= Checker =============================

		public static bool HasAllGenes(List<GeneDef> geneDefs, Pawn pawn)
		{
			if (geneDefs.NullOrEmpty())
			{
				return false;
			}
			//List<GeneDef> matchedGenes = new();
			//foreach (Gene gene in pawn.genes.GenesListForReading)
			//{
			//	if (geneDefs.Contains(gene.def) && !matchedGenes.Contains(gene.def))
			//	{
			//		matchedGenes.Add(gene.def);
			//	}
			//}
			//return geneDefs.Count == matchedGenes.Count;
			foreach (GeneDef geneDef in geneDefs)
			{
				if (!XaG_GeneUtility.HasGene(geneDef, pawn))
				{
					return false;
				}
			}
			return true;
		}

		public static bool HasAnyGene(List<GeneDef> geneDefs, Pawn pawn)
		{
			if (geneDefs.NullOrEmpty() || pawn?.genes == null)
			{
				return false;
			}
			return pawn.genes.GenesListForReading.Any((Gene gene) => geneDefs.Contains(gene.def));
		}

		public static bool HasAnyActiveGene(List<GeneDef> geneDefs, Pawn pawn)
		{
			if (geneDefs.NullOrEmpty() || pawn?.genes == null)
			{
				return false;
			}
			//List<Gene> genesListForReading = pawn.genes.GenesListForReading;
			//for (int j = 0; j < genesListForReading.Count; j++)
			//{
			//	if (!genesListForReading[j].Active)
			//	{
			//		continue;
			//	}
			//	for (int i = 0; i < geneDefs.Count; i++)
			//	{
			//		if (genesListForReading[j].def == geneDefs[i])
			//		{
			//			return true;
			//		}
			//	}
			//}
			return pawn.genes.GenesListForReading.Any((Gene gene) => geneDefs.Contains(gene.def) && gene.Active);
		}

		public static Gene GetFirstSkinColorOverrideGene(Pawn pawn)
		{
			if (pawn?.genes == null)
			{
				return null;
			}
			List<Gene> xenogenes = pawn.genes.Xenogenes;
			if (!xenogenes.NullOrEmpty())
			{
				for (int i = 0; i < xenogenes.Count; i++)
				{
					if (xenogenes[i].Active && xenogenes[i].def.skinColorOverride != null)
					{
						return xenogenes[i];
					}
				}
			}
			List<Gene> endogenes = pawn.genes.Endogenes;
			for (int i = 0; i < endogenes.Count; i++)
			{
				if (endogenes[i].Active && endogenes[i].def.skinColorOverride != null)
				{
					return endogenes[i];
				}
			}
			return null;
		}

		public static Gene GetFirstHairColorGene(Pawn pawn)
		{
			if (pawn?.genes == null)
			{
				return null;
			}
			List<Gene> xenogenes = pawn.genes.Xenogenes;
			if (!xenogenes.NullOrEmpty())
			{
				for (int i = 0; i < xenogenes.Count; i++)
				{
					if (xenogenes[i].Active && xenogenes[i].def.hairColorOverride != null)
					{
						return xenogenes[i];
					}
				}
			}
			List<Gene> endogenes = pawn.genes.Endogenes;
			for (int i = 0; i < endogenes.Count; i++)
			{
				if (endogenes[i].Active && endogenes[i].def.hairColorOverride != null)
				{
					return endogenes[i];
				}
			}
			return null;
		}

		public static Gene GetGene(GeneDef geneDef, Pawn pawn)
		{
			if (geneDef == null || pawn?.genes == null)
			{
				return null;
			}
			List<Gene> genesListForReading = pawn.genes.GenesListForReading;
			for (int i = 0; i < genesListForReading.Count; i++)
			{
				if (genesListForReading[i].def == geneDef)
				{
					return genesListForReading[i];
				}
			}
			return null;
		}

		public static Gene GetXenogene(GeneDef geneDef, Pawn pawn)
		{
			if (geneDef == null || pawn?.genes == null)
			{
				return null;
			}
			List<Gene> genesListForReading = pawn.genes.Xenogenes;
			for (int i = 0; i < genesListForReading.Count; i++)
			{
				if (genesListForReading[i].def == geneDef)
				{
					return genesListForReading[i];
				}
			}
			return null;
		}

		public static Gene GetEndogene(GeneDef geneDef, Pawn pawn)
		{
			if (geneDef == null || pawn?.genes == null)
			{
				return null;
			}
			List<Gene> genesListForReading = pawn.genes.Endogenes;
			for (int i = 0; i < genesListForReading.Count; i++)
			{
				if (genesListForReading[i].def == geneDef)
				{
					return genesListForReading[i];
				}
			}
			return null;
		}

		public static bool HasXenogene(GeneDef geneDef, Pawn pawn)
		{
			return GetXenogene(geneDef, pawn) != null;
		}

		public static bool HasEndogene(GeneDef geneDef, Pawn pawn)
		{
			return GetEndogene(geneDef, pawn) != null;
		}

		public static bool HasGene(GeneDef geneDef, Pawn pawn)
		{
			return GetGene(geneDef, pawn) != null;
		}

		public static bool HasActiveXenogene(GeneDef geneDef, Pawn pawn)
		{
			if (geneDef == null || pawn?.genes == null)
			{
				return false;
			}
			List<Gene> genesListForReading = pawn.genes.Xenogenes;
			for (int i = 0; i < genesListForReading.Count; i++)
			{
				if (genesListForReading[i].def == geneDef && genesListForReading[i].Active)
				{
					return true;
				}
			}
			return false;
		}

		public static bool HasActiveEndogene(GeneDef geneDef, Pawn pawn)
		{
			if (geneDef == null || pawn?.genes == null)
			{
				return false;
			}
			List<Gene> genesListForReading = pawn.genes.Endogenes;
			for (int i = 0; i < genesListForReading.Count; i++)
			{
				if (genesListForReading[i].def == geneDef && genesListForReading[i].Active)
				{
					return true;
				}
			}
			return false;
		}

		public static bool HasActiveGene(GeneDef geneDef, Pawn pawn)
		{
			if (geneDef == null || pawn?.genes == null)
			{
				return false;
			}
			List<Gene> genesListForReading = pawn.genes.GenesListForReading;
			for (int i = 0; i < genesListForReading.Count; i++)
			{
				if (genesListForReading[i].def == geneDef && genesListForReading[i].Active)
				{
					return true;
				}
			}
			return false;
		}

		public static bool HasGeneOfType(Gene gene, Pawn pawn)
		{
			if (gene == null || pawn?.genes == null)
			{
				return false;
			}
			List<Gene> genesListForReading = pawn.genes.GenesListForReading;
			for (int i = 0; i < genesListForReading.Count; i++)
			{
				if (genesListForReading[i].Active && genesListForReading[i].def.geneClass == gene.def.geneClass)
				{
					return true;
				}
			}
			return false;
		}

		public static bool GenesIsMatchForPawns(List<Pawn> pawns, List<GeneDef> xenotypeGenes, float percent)
		{
			if (xenotypeGenes.NullOrEmpty() || percent <= 0f)
			{
				return true;
			}
			List<Gene> genes = new();
			foreach (Pawn pawn in pawns)
			{
				List<Gene> genesListForReading = pawn?.genes?.GenesListForReading;
				if (!genesListForReading.NullOrEmpty())
				{
					foreach (Gene gene in pawn.genes.GenesListForReading)
					{
						if (!genes.Contains(gene))
						{
							genes.Add(gene);
						}
					}
				}
			}
			if (GenesIsMatch(genes, xenotypeGenes, percent))
			{
				return true;
			}
			return false;
		}

		//public static bool GenesIsMatch(List<Gene> pawnGenes, List<Gene> otherGenes, float percent)
		//{
		//	if (otherGenes.NullOrEmpty() || percent <= 0f)
		//	{
		//		return true;
		//	}
		//	if (pawnGenes.NullOrEmpty())
		//	{
		//		return false;
		//	}
		//	int matchingGenes = 0;
		//	foreach (Gene gene in pawnGenes)
		//	{
		//		foreach (Gene otherGene in otherGenes)
		//		{
		//			if (gene.def == otherGene.def)
		//			{
		//				matchingGenes++;
		//				break;
		//			}
		//		}
		//	}
		//	if (matchingGenes >= otherGenes.Count * percent)
		//	{
		//		return true;
		//	}
		//	return false;
		//}

		public static bool GenesIsMatch(List<Gene> pawnGenes, List<GeneDef> xenotypeGenes, float percent)
		{
			if (xenotypeGenes.NullOrEmpty() || percent <= 0f)
			{
				return true;
			}
			if (pawnGenes.NullOrEmpty())
			{
				return false;
			}
			List<GeneDef> matchingGenes = GetMatchingGenesList(pawnGenes, xenotypeGenes);
			if (matchingGenes.Count >= xenotypeGenes.Count * percent)
			{
				return true;
			}
			return false;
		}

		public static bool GenesIsMatch(List<GeneDef> pawnGenes, List<GeneDef> xenotypeGenes, float percent)
		{
			if (xenotypeGenes.NullOrEmpty() || percent <= 0f)
			{
				return true;
			}
			if (pawnGenes.NullOrEmpty())
			{
				return false;
			}
			List<GeneDef> matchingGenes = GetMatchingGenesList(pawnGenes, xenotypeGenes);
			if (matchingGenes.Count >= xenotypeGenes.Count * percent)
			{
				return true;
			}
			return false;
		}

		// public static bool PawnIsBaseliner(Pawn pawn)
		// {
		// if (pawn.genes == null)
		// {
		// return true;
		// }
		// if (pawn.genes.CustomXenotype != null)
		// {
		// return false;
		// }
		// if (pawn.genes.Xenotype == XenotypeDefOf.Baseliner)
		// {
		// return true;
		// }
		// return false;
		// }

		// ============================= Getter =============================

		//[Obsolete]
		//public static bool AnyGeneDefIsSubGeneOf(List<GeneDef> geneDefs, GeneDef parentGeneDef)
		//{
		//          foreach (GeneDef geneDef in geneDefs)
		//          {
		//              if (GeneDefIsSubGeneOf(geneDef, parentGeneDef))
		//              {
		//                  return true;
		//              }
		//          }
		//          return false;
		//}

		//[Obsolete]
		//public static bool GeneDefIsSubGeneOf(GeneDef childGeneDef, GeneDef parentGeneDef)
		//{
		//	if (childGeneDef == parentGeneDef)
		//	{
		//		return true;
		//	}
		//	if (childGeneDef?.prerequisite != null)
		//	{
		//		return GeneDefIsSubGeneOf(childGeneDef.prerequisite, parentGeneDef);
		//	}
		//	return false;
		//}

		public static bool GeneDefHasSubGenes_WithCount(GeneDef parentGeneDef, ref int deepness)
		{
			if (parentGeneDef?.prerequisite != null)
			{
				deepness++;
				return GeneDefHasSubGenes_WithCount(parentGeneDef.prerequisite, ref deepness);
			}
			return false;
		}

		public static List<XenotypeDef> GetAllMatchedXenotypes_ForPawns(List<Pawn> pawns, List<XenotypeDef> xenotypeDefs, float percent = 0.6f)
		{
			if (pawns.NullOrEmpty() || xenotypeDefs.NullOrEmpty())
			{
				return null;
			}
			List<XenotypeDef> allMatched = new();
			// foreach (Pawn item in pawns)
			// {
			// List<XenotypeDef> matched = GetAllMatchedXenotypes(item, xenotypeDefs, percent);
			// foreach (XenotypeDef xeno in matched)
			// {
			// if (!allMatched.Contains(xeno))
			// {
			// allMatched.Add(xeno);
			// }
			// }
			// }
			List<Gene> genes = new();
			foreach (Pawn pawn in pawns)
			{
				List<Gene> genesListForReading = pawn?.genes?.GenesListForReading;
				if (!genesListForReading.NullOrEmpty())
				{
					foreach (Gene gene in pawn.genes.GenesListForReading)
					{
						if (!genes.Contains(gene))
						{
							genes.Add(gene);
						}
					}
				}
			}
			foreach (XenotypeDef item in xenotypeDefs)
			{
				if (GenesIsMatch(genes, item.genes, percent))
				{
					if (!allMatched.Contains(item))
					{
						allMatched.Add(item);
					}
				}
			}
			return allMatched;
		}

		public static List<XenotypeDef> GetAllMatchedXenotypes(Pawn pawn, List<XenotypeDef> xenotypeDefs, float percent = 0.6f)
		{
			List<Gene> pawnGenes = pawn?.genes?.GenesListForReading;
			if (pawnGenes.NullOrEmpty() || xenotypeDefs.NullOrEmpty())
			{
				return null;
			}
			List<XenotypeDef> matched = new();
			foreach (XenotypeDef item in xenotypeDefs)
			{
				if (GenesIsMatch(pawnGenes, item.genes, percent))
				{
					matched.Add(item);
				}
			}
			return matched;
		}

		public static List<CustomXenotype> GetAllMatchedCustomXenotypes(Pawn pawn, List<CustomXenotype> xenotypeDefs, float percent = 0.6f)
		{
			List<Gene> pawnGenes = pawn?.genes?.GenesListForReading;
			if (pawnGenes.NullOrEmpty() || xenotypeDefs.NullOrEmpty())
			{
				return null;
			}
			List<CustomXenotype> matched = new();
			foreach (CustomXenotype item in xenotypeDefs)
			{
				if (GenesIsMatch(pawnGenes, item.genes, percent))
				{
					matched.Add(item);
				}
			}
			return matched;
		}

		public static List<GeneDef> GetMatchingGenesList(List<Gene> pawnGenes, List<GeneDef> xenotypeGenes)
		{
			if (pawnGenes.NullOrEmpty() || xenotypeGenes.NullOrEmpty())
			{
				return null;
			}
			List<Gene> genes = new();
			foreach (Gene item in pawnGenes)
			{
				genes.Add(item);
			}
			List<GeneDef> geneDef = new();
			foreach (Gene item in genes)
			{
				if (xenotypeGenes.Contains(item.def))
				{
					geneDef.Add(item.def);
				}
			}
			return geneDef;
		}

		public static List<GeneDef> GetMatchingGenesList(List<GeneDef> pawnGenes, List<GeneDef> xenotypeGenes)
		{
			if (pawnGenes.NullOrEmpty() || xenotypeGenes.NullOrEmpty())
			{
				return null;
			}
			List<GeneDef> geneDef = new();
			foreach (GeneDef item in pawnGenes)
			{
				if (xenotypeGenes.Contains(item))
				{
					geneDef.Add(item);
				}
			}
			return geneDef;
		}

		public static GeneDef GetAnyActiveGeneFromList(List<GeneDef> geneDefs, Pawn pawn)
		{
			for (int i = 0; i < geneDefs.Count; i++)
			{
				if (geneDefs[i] == null)
				{
					continue;
				}
				List<Gene> genesListForReading = pawn.genes.GenesListForReading;
				for (int j = 0; j < genesListForReading.Count; j++)
				{
					if (genesListForReading[j].def == geneDefs[i] && genesListForReading[j].Active == true)
					{
						return geneDefs[i];
					}
				}
			}
			return null;
		}

		public static void DevGetMatchingList(Pawn pawn, float percent = 0.6f)
		{
			List<XenotypeDef> xenotypesDef = XaG_GeneUtility.GetAllMatchedXenotypes(pawn, ListsUtility.GetAllXenotypesExceptAndroids(), percent);
			if (!xenotypesDef.NullOrEmpty())
			{
				Log.Error("All matched xenotypes:" + "\n" + xenotypesDef.Select((XenotypeDef x) => x.defName).ToLineList("- "));
			}
			else
			{
				Log.Error("Match list is null");
			}
		}

		// =============================== Getter ===============================

		//public static int GetAllGenesCount(XenotypeDef xenotypeDef)
		//{
		//	int genesCount = 0;
		//	List<XenotypeDef> xenotypes = GetXenotypeAndDoubleXenotypes(xenotypeDef);
		//	foreach (XenotypeDef xenotype in xenotypes)
		//	{
		//		genesCount += xenotype.genes.Count;
		//	}
		//	return genesCount;
		//}

		public static List<XenotypeDef> GetXenotypeAndDoubleXenotypes(XenotypeDef xenotypeDef)
		{
			List<XenotypeDef> xenotypes = new();
			xenotypes.Add(xenotypeDef);
			if (!xenotypeDef.doubleXenotypeChances.NullOrEmpty())
			{
				foreach (XenotypeChance item in xenotypeDef.doubleXenotypeChances)
				{
					xenotypes.Add(item.xenotype);
				}
			}
			return xenotypes;
		}

		public static void GetBiostatsFromList(List<Gene> genes, out int cpx, out int met, out int arc)
		{
			cpx = 0;
			met = 0;
			arc = 0;
			if (genes.NullOrEmpty())
			{
				return;
			}
			foreach (Gene item in genes)
			{
				if (item.Overridden)
				{
					continue;
				}
				cpx += item.def.biostatCpx;
				met += item.def.biostatMet;
				arc += item.def.biostatArc;
			}
		}

		public static void GetBiostatsFromList(List<GeneDef> genes, out int cpx, out int met, out int arc)
		{
			cpx = 0;
			met = 0;
			arc = 0;
			if (genes.NullOrEmpty())
			{
				return;
			}
			foreach (GeneDef item in genes)
			{
				cpx += item.biostatCpx;
				met += item.biostatMet;
				arc += item.biostatArc;
			}
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

		public static bool XenotypeHasArchites(XenotypeHolder xenotypeDef)
		{
			if (xenotypeDef.genes.NullOrEmpty())
			{
				return false;
			}
			return xenotypeDef.genes.Any((GeneDef geneDef) => geneDef.biostatArc != 0);
		}

		// XaG test

		public static bool IsXenoGenesDef(this Def def)
		{
			return def?.modContentPack != null && def.modContentPack.PackageId.Contains("wvc.sergkart.races.biotech");
			//return def?.modContentPack != null && def.modContentPack == InitialUtility.GetMod.Content;
		}

		public static bool IsVanillaDef(this Def def)
		{
			return def?.modContentPack != null && def.modContentPack.IsOfficialMod;
		}

		// Wiki
		public static string GetDescriptionFull_Wiki(GeneDef geneDef)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(" " + geneDef.LabelCap);
			sb.AppendLine();
			sb.AppendLine("defName: `" + geneDef.defName + "` | geneClass: `" + geneDef.geneClass + "`");
			sb.AppendLine();
			if (!geneDef.description.NullOrEmpty())
			{
				sb.AppendLine("> " + geneDef.description).AppendLine().AppendLine();
			}
			bool flag = false;
			if (geneDef.prerequisite != null)
			{
				if (geneDef.prerequisite.IsXenoGenesDef())
				{
					sb.AppendLine("- " + "Requires".Translate() + ": " + "[" + geneDef.prerequisite.LabelCap + "](https://github.com/WVCSergkart/WVC_RacesBiotech/wiki/Genes-DUMP#" + geneDef.prerequisite.label.Replace(" ", "-") + ")" + " ([Genes-WIP](https://github.com/WVCSergkart/WVC_RacesBiotech/wiki/Genes-(WIP)#" + geneDef.prerequisite.label.Replace(" ", "-") + "))");
				}
				else
				{
					sb.AppendLine("- " + "Requires".Translate() + ": " + geneDef.prerequisite.LabelCap);
				}
				flag = true;
			}
			if (geneDef.minAgeActive > 0f)
			{
				sb.AppendLine(string.Concat("- " + "TakesEffectAfterAge".Translate() + ": ", geneDef.minAgeActive.ToString()));
				flag = true;
			}
			if (flag)
			{
				sb.AppendLine();
			}
			bool flag2 = false;
			if (geneDef.biostatCpx != 0)
			{
				sb.AppendLineTagged("- " + "Complexity".Translate().Colorize(GeneUtility.GCXColor) + ": " + geneDef.biostatCpx.ToStringWithSign());
				flag2 = true;
			}
			if (geneDef.biostatMet != 0)
			{
				sb.AppendLineTagged("- " + "Metabolism".Translate().CapitalizeFirst().Colorize(GeneUtility.METColor) + ": " + geneDef.biostatMet.ToStringWithSign());
				flag2 = true;
			}
			if (geneDef.biostatArc != 0)
			{
				sb.AppendLineTagged("- " + "ArchitesRequired".Translate().Colorize(GeneUtility.ARCColor) + ": " + geneDef.biostatArc.ToStringWithSign());
				flag2 = true;
			}
			if (flag2)
			{
				sb.AppendLine();
			}
			if (geneDef.forcedTraits != null)
			{
				sb.AppendLineTagged(("- " + "ForcedTraits".Translate() + ":").Colorize(ColoredText.TipSectionTitleColor));
				sb.Append(geneDef.forcedTraits.Select((GeneticTraitData x) => x.def.DataAtDegree(x.degree).label).ToLineList("	* ", capitalizeItems: true)).AppendLine().AppendLine();
			}
			if (geneDef.suppressedTraits != null)
			{
				sb.AppendLineTagged(("- " + "SuppressedTraits".Translate() + ":").Colorize(ColoredText.TipSectionTitleColor));
				sb.Append(geneDef.suppressedTraits.Select((GeneticTraitData x) => x.def.DataAtDegree(x.degree).label).ToLineList("	* ", capitalizeItems: true)).AppendLine().AppendLine();
			}
			if (geneDef.aptitudes != null)
			{
				sb.AppendLineTagged(("- " + "Aptitudes".Translate().CapitalizeFirst() + ":").Colorize(ColoredText.TipSectionTitleColor));
				sb.Append(geneDef.aptitudes.Select((Aptitude x) => x.skill.LabelCap.ToString() + " " + x.level.ToStringWithSign()).ToLineList("	* ", capitalizeItems: true)).AppendLine().AppendLine();
			}
			bool effectsTitleWritten = false;
			if (geneDef.passionMod != null)
			{
				switch (geneDef.passionMod.modType)
				{
					case PassionMod.PassionModType.AddOneLevel:
						AppendEffectLine("PassionModAdd".Translate(geneDef.passionMod.skill));
						break;
					case PassionMod.PassionModType.DropAll:
						AppendEffectLine("PassionModDrop".Translate(geneDef.passionMod.skill));
						break;
				}
			}
			if (!geneDef.statFactors.NullOrEmpty())
			{
				for (int num = 0; num < geneDef.statFactors.Count; num++)
				{
					StatModifier statModifier = geneDef.statFactors[num];
					if (statModifier.stat.CanShowWithLoadedMods())
					{
						AppendEffectLine(statModifier.stat.LabelCap + " " + statModifier.ToStringAsFactor);
					}
				}
			}
			if (!geneDef.conditionalStatAffecters.NullOrEmpty())
			{
				for (int num2 = 0; num2 < geneDef.conditionalStatAffecters.Count; num2++)
				{
					if (geneDef.conditionalStatAffecters[num2].statFactors.NullOrEmpty())
					{
						continue;
					}
					for (int num3 = 0; num3 < geneDef.conditionalStatAffecters[num2].statFactors.Count; num3++)
					{
						StatModifier statModifier2 = geneDef.conditionalStatAffecters[num2].statFactors[num3];
						if (statModifier2.stat.CanShowWithLoadedMods())
						{
							AppendEffectLine(statModifier2.stat.LabelCap + " " + statModifier2.ToStringAsFactor + " (" + geneDef.conditionalStatAffecters[num2].Label + ")");
						}
					}
				}
			}
			if (!geneDef.statOffsets.NullOrEmpty())
			{
				for (int num4 = 0; num4 < geneDef.statOffsets.Count; num4++)
				{
					StatModifier statModifier3 = geneDef.statOffsets[num4];
					if (statModifier3.stat.CanShowWithLoadedMods())
					{
						AppendEffectLine(statModifier3.stat.LabelCap + " " + statModifier3.ValueToStringAsOffset);
					}
				}
			}
			if (!geneDef.conditionalStatAffecters.NullOrEmpty())
			{
				for (int num5 = 0; num5 < geneDef.conditionalStatAffecters.Count; num5++)
				{
					if (geneDef.conditionalStatAffecters[num5].statOffsets.NullOrEmpty())
					{
						continue;
					}
					for (int num6 = 0; num6 < geneDef.conditionalStatAffecters[num5].statOffsets.Count; num6++)
					{
						StatModifier statModifier4 = geneDef.conditionalStatAffecters[num5].statOffsets[num6];
						if (statModifier4.stat.CanShowWithLoadedMods())
						{
							AppendEffectLine(statModifier4.stat.LabelCap + " " + statModifier4.ValueToStringAsOffset + " (" + geneDef.conditionalStatAffecters[num5].Label.UncapitalizeFirst() + ")");
						}
					}
				}
			}
			if (!geneDef.capMods.NullOrEmpty())
			{
				for (int num7 = 0; num7 < geneDef.capMods.Count; num7++)
				{
					PawnCapacityModifier pawnCapacityModifier = geneDef.capMods[num7];
					if (pawnCapacityModifier.offset != 0f)
					{
						AppendEffectLine(pawnCapacityModifier.capacity.GetLabelFor().CapitalizeFirst() + " " + (pawnCapacityModifier.offset * 100f).ToString("+#;-#") + "%");
					}
					if (pawnCapacityModifier.postFactor != 1f)
					{
						AppendEffectLine(pawnCapacityModifier.capacity.GetLabelFor().CapitalizeFirst() + " x" + pawnCapacityModifier.postFactor.ToStringPercent());
					}
					if (pawnCapacityModifier.setMax != 999f)
					{
						AppendEffectLine(pawnCapacityModifier.capacity.GetLabelFor().CapitalizeFirst() + " " + "max".Translate().CapitalizeFirst() + ": " + pawnCapacityModifier.setMax.ToStringPercent());
					}
				}
			}
			if (!geneDef.customEffectDescriptions.NullOrEmpty())
			{
				foreach (string customEffectDescription in geneDef.customEffectDescriptions)
				{
					AppendEffectLine(customEffectDescription.ResolveTags());
				}
			}
			if (!geneDef.damageFactors.NullOrEmpty())
			{
				for (int num8 = 0; num8 < geneDef.damageFactors.Count; num8++)
				{
					AppendEffectLine("DamageType".Translate(geneDef.damageFactors[num8].damageDef.label).CapitalizeFirst() + " x" + geneDef.damageFactors[num8].factor.ToStringPercent());
				}
			}
			if (geneDef.resourceLossPerDay != 0f && !geneDef.resourceLabel.NullOrEmpty())
			{
				AppendEffectLine("ResourceLossPerDay".Translate(geneDef.resourceLabel.Named("RESOURCE"), (-Mathf.RoundToInt(geneDef.resourceLossPerDay * 100f)).ToStringWithSign().Named("OFFSET")).CapitalizeFirst());
			}
			if (!Mathf.Approximately(geneDef.painFactor, 1f))
			{
				AppendEffectLine("Pain".Translate() + " x" + geneDef.painFactor.ToStringPercent());
			}
			if (geneDef.painOffset != 0f)
			{
				AppendEffectLine("Pain".Translate() + " " + (geneDef.painOffset * 100f).ToString("+###0;-###0") + "%");
			}
			if (geneDef.chemical != null)
			{
				if (!Mathf.Approximately(geneDef.addictionChanceFactor, 1f))
				{
					if (geneDef.addictionChanceFactor <= 0f)
					{
						AppendEffectLine("AddictionImmune".Translate(geneDef.chemical).CapitalizeFirst());
					}
					else
					{
						AppendEffectLine("AddictionChanceFactor".Translate(geneDef.chemical).CapitalizeFirst() + " x" + geneDef.addictionChanceFactor.ToStringPercent());
					}
				}
				if (geneDef.overdoseChanceFactor != 1f)
				{
					AppendEffectLine("OverdoseChanceFactor".Translate(geneDef.chemical).CapitalizeFirst() + " x" + geneDef.overdoseChanceFactor.ToStringPercent());
				}
				if (geneDef.toleranceBuildupFactor != 1f)
				{
					AppendEffectLine("ToleranceBuildupFactor".Translate(geneDef.chemical).CapitalizeFirst() + " x" + geneDef.toleranceBuildupFactor.ToStringPercent());
				}
			}
			if (!geneDef.enablesNeeds.NullOrEmpty())
			{
				if (geneDef.enablesNeeds.Count == 1)
				{
					AppendEffectLine(string.Format("{0}: {1}", "AddsNeed".Translate(), geneDef.enablesNeeds[0].LabelCap));
				}
				else
				{
					AppendEffectLine(string.Format("{0}: {1}", "AddsNeeds".Translate(), geneDef.enablesNeeds.Select((NeedDef x) => x.label).ToCommaList().CapitalizeFirst()));
				}
			}
			if (!geneDef.disablesNeeds.NullOrEmpty())
			{
				if (geneDef.disablesNeeds.Count == 1)
				{
					AppendEffectLine(string.Format("{0}: {1}", "DisablesNeed".Translate(), geneDef.disablesNeeds[0].LabelCap));
				}
				else
				{
					AppendEffectLine(string.Format("{0}: {1}", "DisablesNeeds".Translate(), geneDef.disablesNeeds.Select((NeedDef x) => x.label).ToCommaList().CapitalizeFirst()));
				}
			}
			if (geneDef.missingGeneRomanceChanceFactor != 1f)
			{
				AppendEffectLine("MissingGeneRomanceChance".Translate(geneDef.label.Named("GENE")) + " x" + geneDef.missingGeneRomanceChanceFactor.ToStringPercent());
			}
			if (geneDef.ignoreDarkness)
			{
				AppendEffectLine("UnaffectedByDarkness".Translate());
			}
			if (geneDef.foodPoisoningChanceFactor != 1f)
			{
				if (geneDef.foodPoisoningChanceFactor <= 0f)
				{
					AppendEffectLine("FoodPoisoningImmune".Translate());
				}
				else
				{
					AppendEffectLine("Stat_Hediff_FoodPoisoningChanceFactor_Name".Translate() + " x" + geneDef.foodPoisoningChanceFactor.ToStringPercent());
				}
			}
			if (geneDef.socialFightChanceFactor != 1f)
			{
				if (geneDef.socialFightChanceFactor <= 0f)
				{
					AppendEffectLine("WillNeverSocialFight".Translate());
				}
				else
				{
					AppendEffectLine("SocialFightChanceFactor".Translate() + " x" + geneDef.socialFightChanceFactor.ToStringPercent());
				}
			}
			if (geneDef.aggroMentalBreakSelectionChanceFactor != 1f)
			{
				if (geneDef.aggroMentalBreakSelectionChanceFactor >= 999f)
				{
					AppendEffectLine("AlwaysAggroMentalBreak".Translate());
				}
				else if (geneDef.aggroMentalBreakSelectionChanceFactor <= 0f)
				{
					AppendEffectLine("NeverAggroMentalBreak".Translate());
				}
				else
				{
					AppendEffectLine("AggroMentalBreakSelectionChanceFactor".Translate() + " x" + geneDef.aggroMentalBreakSelectionChanceFactor.ToStringPercent());
				}
			}
			if (geneDef.prisonBreakMTBFactor != 1f)
			{
				if (geneDef.prisonBreakMTBFactor < 0f)
				{
					AppendEffectLine("WillNeverPrisonBreak".Translate());
				}
				else
				{
					AppendEffectLine("PrisonBreakIntervalFactor".Translate() + " x" + geneDef.prisonBreakMTBFactor.ToStringPercent());
				}
			}
			bool flag3 = effectsTitleWritten;
			if (!geneDef.makeImmuneTo.NullOrEmpty())
			{
				if (flag3)
				{
					sb.AppendLine();
				}
				sb.AppendLineTagged(("- " + "ImmuneTo".Translate() + ":").Colorize(ColoredText.TipSectionTitleColor));
				sb.AppendLine(geneDef.makeImmuneTo.Select((HediffDef x) => x.label).ToLineList("	* ", capitalizeItems: true));
				flag3 = true;
			}
			if (!geneDef.hediffGiversCannotGive.NullOrEmpty())
			{
				if (flag3)
				{
					sb.AppendLine();
				}
				sb.AppendLineTagged(("- " + "ImmuneTo".Translate() + ":").Colorize(ColoredText.TipSectionTitleColor));
				sb.AppendLine(geneDef.hediffGiversCannotGive.Select((HediffDef x) => x.label).ToLineList("	* ", capitalizeItems: true));
				flag3 = true;
			}
			if (geneDef.biologicalAgeTickFactorFromAgeCurve != null)
			{
				if (flag3)
				{
					sb.AppendLine();
				}
				sb.AppendLineTagged(("- " + "AgeFactors".Translate().CapitalizeFirst() + " curve" + ":").Colorize(ColoredText.TipSectionTitleColor));
				sb.AppendLine(geneDef.biologicalAgeTickFactorFromAgeCurve.Select((CurvePoint p) => "PeriodYears".Translate(p.x).ToString() + ": x" + p.y.ToStringPercent()).ToLineList("	* ", capitalizeItems: true));
				flag3 = true;
			}
			if (geneDef.disabledWorkTags != WorkTags.None)
			{
				if (flag3)
				{
					sb.AppendLine();
				}
				IEnumerable<WorkTypeDef> source = DefDatabase<WorkTypeDef>.AllDefsListForReading.Where((WorkTypeDef x) => (geneDef.disabledWorkTags & x.workTags) != 0);
				sb.AppendLineTagged(("- " + "DisabledWorkLabel".Translate().CapitalizeFirst() + ":").Colorize(ColoredText.TipSectionTitleColor));
				sb.AppendLine("	* " + source.Select((WorkTypeDef x) => x.labelShort).ToCommaList().CapitalizeFirst());
				if (geneDef.disabledWorkTags.ExactlyOneWorkTagSet())
				{
					sb.AppendLine("	* " + geneDef.disabledWorkTags.LabelTranslated().CapitalizeFirst());
				}
				flag3 = true;
			}
			if (!geneDef.abilities.NullOrEmpty())
			{
				if (flag3)
				{
					sb.AppendLine();
				}
				if (geneDef.abilities.Count == 1)
				{
					sb.AppendLineTagged(("- " + "GivesAbility".Translate().CapitalizeFirst() + ":").Colorize(ColoredText.TipSectionTitleColor));
				}
				else
				{
					sb.AppendLineTagged(("- " + "GivesAbilities".Translate().CapitalizeFirst() + ":").Colorize(ColoredText.TipSectionTitleColor));
				}
				sb.AppendLine(geneDef.abilities.Select((AbilityDef x) => x.label).ToLineList("	* ", capitalizeItems: true));
				flag3 = true;
			}
			IEnumerable<ThoughtDef> enumerable = DefDatabase<ThoughtDef>.AllDefs.Where((ThoughtDef x) => (x.requiredGenes.NotNullAndContains(geneDef) || x.nullifyingGenes.NotNullAndContains(geneDef)) && x.stages != null && x.stages.Any((ThoughtStage y) => y.baseMoodEffect != 0f));
			if (enumerable.Any())
			{
				if (flag3)
				{
					sb.AppendLine();
				}
				sb.AppendLineTagged(("- " + "Mood".Translate().CapitalizeFirst() + ":").Colorize(ColoredText.TipSectionTitleColor));
				foreach (ThoughtDef item in enumerable)
				{
					ThoughtStage thoughtStage = item.stages.FirstOrDefault((ThoughtStage x) => x.baseMoodEffect != 0f);
					if (thoughtStage != null)
					{
						string text = thoughtStage.LabelCap + ": " + thoughtStage.baseMoodEffect.ToStringWithSign();
						if (item.requiredGenes.NotNullAndContains(geneDef))
						{
							sb.AppendLine("	* " + text);
						}
						else if (item.nullifyingGenes.NotNullAndContains(geneDef))
						{
							sb.AppendLine("	* " + "Removes".Translate() + ": " + text);
						}
					}
				}
			}
			sb.AppendLine();
			sb.AppendLine("**Selection weight (Genepack chance)**: " + (geneDef.selectionWeight * 100) + "% or " + geneDef.selectionWeight);
			List<XenotypeDef> xenotypeDefs = DefDatabase<XenotypeDef>.AllDefsListForReading.Where(xeno => xeno.genes.Any(gene => gene == geneDef)).ToList();
			if (!xenotypeDefs.NullOrEmpty())
			{
				sb.AppendLine();
				string xenosList = "";
				foreach (XenotypeDef xenotypeDef in xenotypeDefs)
				{
					xenosList += "\n - [" + xenotypeDef.LabelCap + "](https://github.com/WVCSergkart/WVC_RacesBiotech/wiki/Xenotypes#" + xenotypeDef.label.Replace(" ", "-") + ")";
				}
				sb.AppendLine("**In use by**:" + xenosList);
				//sb.AppendLine(xenotypeDefs.Select(xenos => xenos.label).ToLineList(" - ", capitalizeItems: true));
			}
			//if (geneDef.hairColorOverride != null)
			//{
			//	sb.AppendLine();
			//	sb.AppendLine("**Hair color override**: " + geneDef.hairColorOverride.Value.ToString());
			//}
			//if (geneDef.skinColorOverride != null)
			//{
			//	sb.AppendLine();
			//	sb.AppendLine("**Skin color override**: " + geneDef.skinColorOverride.Value.ToString());
			//}
			//if (geneDef.skinColorBase != null)
			//{
			//	sb.AppendLine();
			//	sb.AppendLine("**Skin color base**: " + geneDef.skinColorBase.Value.ToString());
			//}
			return sb.ToString().TrimEndNewlines();
			void AppendEffectLine(string text2)
			{
				if (!effectsTitleWritten)
				{
					sb.AppendLineTagged(("- " + "Effects".Translate().CapitalizeFirst() + ":").Colorize(ColoredText.TipSectionTitleColor));
					effectsTitleWritten = true;
				}
				sb.AppendLine("	* " + text2);
			}
		}

	}
}
