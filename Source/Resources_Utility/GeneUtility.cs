using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;
using Verse.Grammar;

namespace WVC_XenotypesAndGenes
{

	public static class XaG_GeneUtility
	{

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
			pawn.TryGetComp<CompHumanlike>()?.ResetInspectString();
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

		public static bool Furskin_ShouldNotDrawNow(Pawn pawn)
		{
			return pawn.DevelopmentalStage != DevelopmentalStage.Adult || (pawn.Drawer?.renderer != null ? pawn.Drawer.renderer.CurRotDrawMode : RotDrawMode.Fresh) == RotDrawMode.Dessicated;
		}

		// Genepacks

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
			if (!geneCount.allowedGeneCategoryDefs.NullOrEmpty() && !WVC_Biotech.settings.hideXaGGenes && !geneCount.allowedGeneCategoryDefs.Contains(gene.displayCategory))
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
            if (pawn.Spawned && pawn.Faction == Faction.OfPlayer)
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
				geneDefs.Add(item.def);
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

		public static bool TryRemoveAllConflicts(Pawn pawn, GeneDef geneDef, List<GeneDef> exceptions = null)
		{
			try
			{
				foreach (Gene item in pawn.genes.GenesListForReading.ToList())
				{
					if (!item.def.ConflictsWith(geneDef))
					{
						continue;
					}
					if (exceptions != null && exceptions.Contains(item.def))
					{
						continue;
					}
					pawn.genes.RemoveGene(item);
				}
				return true;
			}
			catch
			{
				Log.Error("Failed remove conflict genes from pawn: " + pawn.LabelShort);
			}
			return false;
		}

		public static bool IsAndroid(this Pawn pawn)
		{
			if (pawn?.genes == null)
			{
				return false;
			}
			List<Gene> genesListForReading = pawn.genes.GenesListForReading;
			for (int i = 0; i < genesListForReading.Count; i++)
			{
				if (genesListForReading[i].def.defName.Contains("VREA_SyntheticBody"))
				{
					return true;
				}
			}
			return false;
		}

		public static bool XenotypeIsAndroid(XenotypeDef xenotypeDef)
		{
			List<GeneDef> genesListForReading = xenotypeDef?.genes;
			if (genesListForReading.NullOrEmpty())
			{
				return false;
			}
			for (int i = 0; i < genesListForReading.Count; i++)
			{
				if (genesListForReading[i].defName.Contains("VREA_SyntheticBody"))
				{
					return true;
				}
			}
			return false;
		}

		public static bool XenotypeIsAndroid(CustomXenotype xenotypeDef)
		{
			List<GeneDef> genesListForReading = xenotypeDef?.genes;
			if (genesListForReading.NullOrEmpty())
			{
				return false;
			}
			for (int i = 0; i < genesListForReading.Count; i++)
			{
				if (genesListForReading[i].defName.Contains("VREA_SyntheticBody"))
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

		// ============================= Checker =============================

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
				if (geneDefs[i] != null)
				{
					List<Gene> genesListForReading = pawn.genes.GenesListForReading;
					for (int j = 0; j < genesListForReading.Count; j++)
					{
						if (genesListForReading[j].Active == true && genesListForReading[j].def == geneDefs[i])
						{
							return geneDefs[i];
						}
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
				Log.Error("All matched xenotypes:" + "\n" + xenotypesDef.Select((XenotypeDef x) => x.defName).ToLineList(" - "));
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
		}

		public static bool IsVanillaDef(this Def def)
		{
			return def?.modContentPack != null && def.modContentPack.IsOfficialMod;
		}

	}
}
