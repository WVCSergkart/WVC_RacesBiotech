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

		public static bool SelectorFactionMap(Pawn pawn)
		{
			return Find.Selector.SelectedPawns.Count > 1 || pawn.Faction != Faction.OfPlayer || pawn.Map == null;
		}

		public static bool ActiveFactionMap(Pawn pawn, Gene gene)
		{
			return !gene.Active || pawn.Faction != Faction.OfPlayer || pawn.Map == null;
		}

		public static bool SelectorActiveFactionMap(Pawn pawn, Gene gene)
		{
			return Find.Selector.SelectedPawns.Count > 1 || ActiveFactionMap(pawn, gene);
		}

		public static bool SelectorActiveFactionMapMechanitor(Pawn pawn, Gene gene)
		{
			return SelectorActiveFactionMap(pawn, gene) || pawn.mechanitor == null;
		}

		public static bool SelectorDraftedActiveFactionMap(Pawn pawn, Gene gene)
		{
			return Find.Selector.SelectedPawns.Count > 1 || pawn.Drafted || ActiveFactionMap(pawn, gene);
		}

		public static bool SelectorActiveFaction(Pawn pawn, Gene gene)
		{
			return Find.Selector.SelectedPawns.Count > 1 || !gene.Active || pawn.Faction != Faction.OfPlayer;
		}

		public static bool SelectorDraftedActiveFaction(Pawn pawn, Gene gene)
		{
			return Find.Selector.SelectedPawns.Count > 1 || pawn.Drafted || !gene.Active || pawn.Faction != Faction.OfPlayer;
		}

		public static void ResetGenesInspectString(Pawn pawn)
		{
			// Log.Error("Check CompHumanlike");
			CompHumanlike humanlike = pawn.TryGetComp<CompHumanlike>();
			if (humanlike != null)
			{
				// Log.Error("CompHumanlike Reset");
				humanlike.ResetInspectString();
			}
		}

		public static bool Furskin_ShouldNotDrawNow(Pawn pawn)
		{
			return pawn.DevelopmentalStage != DevelopmentalStage.Adult || (pawn.Drawer?.renderer != null ? pawn.Drawer.renderer.CurRotDrawMode : RotDrawMode.Fresh) == RotDrawMode.Dessicated;
		}

		// Genepacks

		public static void GenerateName(GeneSet geneSet, RulePackDef rule)
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

		public static void SetGenesInPack(XaG_CountWithChance geneCount, GeneSet geneSet)
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

		public static bool CanAddGeneDuringGeneration(GeneDef gene, GeneSet geneSet, XaG_CountWithChance geneCount)
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

		public static void UpdateXenogermReplication(Pawn pawn, bool addXenogermReplicating = true, IntRange ticksToDisappear = new())
		{
			Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.XenogermReplicating);
			if (firstHediffOfDef != null)
			{
				pawn.health.RemoveHediff(firstHediffOfDef);
			}
			if (addXenogermReplicating)
			{
				// pawn.health.AddHediff(HediffDefOf.XenogermReplicating);
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

		// public static void XenogermRestoration(Pawn pawn)
		// {
			// List<HediffDef> list = new();
			// foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
			// {
				// list.AddRange(item.hediffsRemovedByGenesRestorationSerum);
			// }
			// HediffUtility.RemoveHediffsFromList(pawn, list);
		// }

		// Misc

		// public static bool IsBloodeater(this Pawn pawn)
		// {
			// return pawn?.genes?.GetFirstGeneOfType<Gene_Bloodeater>() != null;
		// }

		public static List<GeneDef> ConvertGenesInGeneDefs(List<Gene> genes)
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

		public static bool ConflictWith(GeneDef geneDef, List<GeneDef> geneDefs)
		{
			foreach (GeneDef item in geneDefs)
			{
				if (item == geneDef)
				{
					return true;
				}
				if (item.ConflictsWith(geneDef))
				{
					return true;
				}
			}
			return false;
		}

		public static bool TryRemoveAllConflicts(Pawn pawn, GeneDef geneDef)
		{
			try
			{
				foreach (Gene item in pawn.genes.GenesListForReading.ToList())
				{
					if (item.def == geneDef)
					{
						pawn.genes.RemoveGene(item);
					}
					if (item.def.ConflictsWith(geneDef))
					{
						pawn.genes.RemoveGene(item);
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

		// ============================= Checker =============================

		public static bool HasAnyActiveGene(List<GeneDef> geneDefs, Pawn pawn)
		{
			if (geneDefs.NullOrEmpty() || pawn?.genes == null)
			{
				return false;
			}
			List<Gene> genesListForReading = pawn.genes.GenesListForReading;
			for (int j = 0; j < genesListForReading.Count; j++)
			{
				if (!genesListForReading[j].Active)
				{
					continue;
				}
				for (int i = 0; i < geneDefs.Count; i++)
				{
					if (genesListForReading[j].def == geneDefs[i])
					{
						return true;
					}
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
			List<Gene> genesListForReading = pawn?.genes?.GenesListForReading;
			for (int i = 0; i < genesListForReading.Count; i++)
			{
				if (genesListForReading[i].Active == true && genesListForReading[i].def == geneDef)
				{
					return true;
				}
			}
			return false;
		}

		public static bool HasGene(GeneDef geneDef, Pawn pawn)
		{
			if (geneDef == null || pawn?.genes == null)
			{
				return false;
			}
			List<Gene> genesListForReading = pawn.genes.GenesListForReading;
			for (int i = 0; i < genesListForReading.Count; i++)
			{
				if (genesListForReading[i].def == geneDef)
				{
					return true;
				}
			}
			return false;
		}

		public static bool HasXenogene(GeneDef geneDef, Pawn pawn)
		{
			if (geneDef == null || pawn?.genes == null)
			{
				return false;
			}
			List<Gene> genesListForReading = pawn.genes.Xenogenes;
			for (int i = 0; i < genesListForReading.Count; i++)
			{
				if (genesListForReading[i].def == geneDef)
				{
					return true;
				}
			}
			return false;
		}

		public static bool HasEndogene(GeneDef geneDef, Pawn pawn)
		{
			if (geneDef == null || pawn?.genes == null)
			{
				return false;
			}
			List<Gene> genesListForReading = pawn.genes.Endogenes;
			for (int i = 0; i < genesListForReading.Count; i++)
			{
				if (genesListForReading[i].def == geneDef)
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

		public static bool AnyGeneDefIsSubGeneOf(List<GeneDef> geneDefs, GeneDef parentGeneDef)
		{
            foreach (GeneDef geneDef in geneDefs)
            {
                if (GeneDefIsSubGeneOf(geneDef, parentGeneDef))
                {
                    return true;
                }
            }
            return false;
		}

		public static bool GeneDefIsSubGeneOf(GeneDef childGeneDef, GeneDef parentGeneDef)
		{
			if (childGeneDef == parentGeneDef)
			{
				return true;
			}
			if (childGeneDef?.prerequisite != null)
			{
				return GeneDefIsSubGeneOf(childGeneDef.prerequisite, parentGeneDef);
			}
			return false;
		}

		public static GeneDef GetFirstGeneDefOfType(List<GeneDef> genes, Type type)
		{
			if (genes.NullOrEmpty())
			{
				return null;
			}
			for (int i = 0; i < genes.Count; i++)
			{
				if (genes[i].geneClass == type)
				{
					return genes[i];
				}
			}
			return null;
		}

		// public static Gene GetFirstGeneOfType(List<Gene> genes, Type type)
		// {
			// for (int i = 0; i < genes.Count; i++)
			// {
				// if (genes[i] is type)
				// {
					// return genes[i];
				// }
			// }
			// return null;
		// }

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

		public static int GetAllGenesCount(XenotypeDef xenotypeDef)
		{
			int genesCount = 0;
			List<XenotypeDef> xenotypes = GetXenotypeAndDoubleXenotypes(xenotypeDef);
			foreach (XenotypeDef xenotype in xenotypes)
			{
				genesCount += xenotype.genes.Count;
			}
			return genesCount;
		}

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

        // Xenotype Cost

        //public static float XenotypeCost(XenotypeDef xenotype)
        //{
        //	return (float)((GetXenotype_Arc(xenotype) * 1.2) + (GetXenotype_Cpx(xenotype) * 0.2) + (-1 * (GetXenotype_Met(xenotype) * 0.4)));
        //}

        //public static int GetXenotype_Cpx(XenotypeDef xenotypeDef)
        //{
        //	List<GeneDef> genes = xenotypeDef?.genes;
        //	if (genes.NullOrEmpty())
        //	{
        //		return 0;
        //	}
        //	int num = 0;
        //	foreach (GeneDef item in genes)
        //	{
        //		num += item.biostatCpx;
        //	}
        //	return num;
        //}

        //public static int GetXenotype_Met(XenotypeDef xenotypeDef)
        //{
        //	List<GeneDef> genes = xenotypeDef?.genes;
        //	if (genes.NullOrEmpty())
        //	{
        //		return 0;
        //	}
        //	int num = 0;
        //	foreach (GeneDef item in genes)
        //	{
        //		num += item.biostatMet;
        //	}
        //	return num;
        //}

        //public static int GetXenotype_Arc(XenotypeDef xenotypeDef)
        //{
        //	List<GeneDef> genes = xenotypeDef?.genes;
        //	if (genes.NullOrEmpty())
        //	{
        //		return 0;
        //	}
        //	int num = 0;
        //	foreach (GeneDef item in genes)
        //	{
        //		num += item.biostatArc;
        //	}
        //	return num;
        //}

        //public static int GetPawn_Arc(Pawn pawn)
        //{
        //	List<Gene> genes = pawn?.genes?.GenesListForReading;
        //	if (genes.NullOrEmpty())
        //	{
        //		return 0;
        //	}
        //	int num = 0;
        //	foreach (Gene item in genes)
        //	{
        //		num += item.def.biostatArc;
        //	}
        //	return num;
        //}

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

		// XaG test

		public static bool IsXenoGenesDef(this Def def)
		{
			return def?.modContentPack != null && def.modContentPack.PackageId.Contains("wvc.sergkart.races.biotech");
		}

	}
}
