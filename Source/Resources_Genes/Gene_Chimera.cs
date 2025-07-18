using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

    public class Gene_Chimera : Gene, IGeneBloodfeeder, IGeneOverridden, IGeneWithEffects, IGeneMetabolism
	{

		public GeneExtension_Undead Props => def?.GetModExtension<GeneExtension_Undead>();

		public GeneExtension_Giver Giver => def.GetModExtension<GeneExtension_Giver>();

		private List<GeneDef> collectedGenes = new();

		private List<GeneDef> consumedGenes = new();

		private List<GeneDef> destroyedGenes = new();

		public List<GeneSetPresets> geneSetPresets = new();

		// private float minCopyChance = WVC_Biotech.settings.chimeraMinGeneCopyChance;

		//public float MinCopyChance
		//{
		//	get
		//	{
		//		return WVC_Biotech.settings.chimeraMinGeneCopyChance;
		//	}
		//}

		public List<GeneDef> AllGenes
		{
			get
			{
				List<GeneDef> genes = new();
				genes.AddRange(consumedGenes);
				genes.AddRange(collectedGenes);
				genes.AddRange(destroyedGenes);
				return genes;
			}
		}

		public List<GeneDef> EatedGenes => consumedGenes;
		public List<GeneDef> CollectedGenes => collectedGenes;
		public List<GeneDef> DestroyedGenes => destroyedGenes;

		public override void PostAdd()
        {
            base.PostAdd();
            UpdateMetabolism();
            if (MiscUtility.GameNotStarted())
            {
                StarterPackSetup();
                NPC_RandomGeneSetSetup();
            }
        }

        private void StarterPackSetup()
        {
            List<GeneDef> geneDefs = DefDatabase<GeneDef>.AllDefsListForReading;
            int cycleTry = 0;
            while (collectedGenes.Count < WVC_Biotech.settings.chimeraStartingGenes)
            {
                if (geneDefs.Where((GeneDef x) => x.endogeneCategory == EndogeneCategory.None && x.selectionWeight > 0f && x.canGenerateInGeneSet && x.passOnDirectly && !AllGenes.Contains(x)).TryRandomElementByWeight((GeneDef gene) => (gene.selectionWeight * (gene.biostatArc != 0 ? 0.01f : 1f)) + (gene.prerequisite == def && gene.GetModExtension<GeneExtension_General>() != null ? gene.GetModExtension<GeneExtension_General>().selectionWeight : 0f), out GeneDef result))
                {
                    TryAddGene(result);
                }
                if (cycleTry > WVC_Biotech.settings.chimeraStartingGenes + 3)
                {
                    break;
                }
                cycleTry++;
			}
			TryGetToolGene();
		}

        public void NPC_RandomGeneSetSetup(bool forceSetup = false)
		{
			if (pawn.Faction == Faction.OfPlayer)
			{
				return;
			}
			if (!forceSetup && !Rand.Chance(0.22f))
            {
                return;
            }
            List<GeneralHolder> chimeraConditionalGenes = pawn.genes?.Xenotype?.GetModExtension<GeneExtension_Undead>()?.chimeraConditionalGenes;
            if (chimeraConditionalGenes != null && chimeraConditionalGenes.TryRandomElementByWeight((count) => count.chance, out GeneralHolder geneSet))
            {
                XaG_GeneUtility.AddGenesToChimera(pawn, geneSet.genes);
            }
        }

        public bool TryGetToolGene()
		{
			if (WVC_Biotech.settings.enable_chimeraStartingTools && Props?.chimeraGenesTools != null && Props.chimeraGenesTools.Where((GeneDef geneDef) => !AllGenes.Contains(geneDef)).TryRandomElement(out GeneDef result))
			{
				TryAddGene(result);
				if (pawn.SpawnedOrAnyParentSpawned)
				{
					Messages.Message("WVC_XaG_GeneGeneticThief_GeneObtained".Translate(pawn.NameShortColored, result.label), pawn, MessageTypeDefOf.NeutralEvent, historical: false);
				}
				return true;
			}
			return false;
		}

		public bool TryGetUniqueGene()
        {
            if (!WVC_Biotech.settings.enable_chimeraStartingTools || (Props?.chimeraConditionalGenes) == null)
            {
                return false;
            }
			List<GeneDefWithChance> genesWithChance = new();
            foreach (GeneralHolder counter in Props.chimeraConditionalGenes)
            {
                if (!counter.CanAddGene(pawn))
                {
                    continue;
                }
                foreach (GeneDef geneDef in counter.genes)
                {
                    if (AllGenes.Contains(geneDef))
                    {
                        continue;
                    }
                    GeneDefWithChance newChance = new();
                    newChance.geneDef = geneDef;
                    newChance.chance = counter.chance;
                    genesWithChance.Add(newChance);
                }
            }
            if (genesWithChance.NullOrEmpty())
            {
				return false;
            }
            if (genesWithChance.TryRandomElementByWeight((geneWithChance) => geneWithChance.chance, out GeneDefWithChance result))
            {
                TryAddGene(result.geneDef);
                if (pawn.SpawnedOrAnyParentSpawned)
                {
                    Messages.Message("WVC_XaG_GeneGeneticThief_GeneObtained".Translate(pawn.NameShortColored, result.geneDef.label), pawn, MessageTypeDefOf.NeutralEvent, historical: false);
                }
                return true;
            }
            return false;
        }

		public void Notify_OverriddenBy(Gene overriddenBy)
		{
			HediffUtility.TryRemoveHediff(Giver.metHediffDef, pawn);
		}

		public void Notify_Override()
		{
			UpdateMetabolism();
		}

		private Gizmo genesGizmo;

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (XaG_GeneUtility.SelectorDraftedActiveFactionMap(pawn, this))
			{
				yield break;
			}
			if (genesGizmo == null)
			{
				genesGizmo = (Gizmo)Activator.CreateInstance(def.resourceGizmoType, this);
			}
			yield return genesGizmo;
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: GetRandomGene",
					action = delegate
					{
						GetRandomGene();
					}
				};
				// yield return new Command_Action
				// {
				// defaultLabel = "DEV: GetAllGenes",
				// action = delegate
				// {
				// foreach (GeneDef geneDef in DefDatabase<GeneDef>.AllDefsListForReading)
				// {
				// AddGene(geneDef);
				// }
				// }
				// };
			}
		}

		//private int lastGeneObtainedTick = -1;

		public bool TryAddGene(GeneDef geneDef)
		{
			if (!AllGenes.Contains(geneDef))
			{
				collectedGenes.Add(geneDef);
				//lastGeneObtainedTick = Find.TickManager.TicksGame;
				return true;
			}
			return false;
		}

		public bool TryEatGene(GeneDef geneDef)
		{
			if (!consumedGenes.Contains(geneDef))
			{
				consumedGenes.Add(geneDef);
				RemoveGene(geneDef);
				return true;
			}
			return false;
		}
		public void RemoveGene(GeneDef geneDef)
		{
			if (collectedGenes.Contains(geneDef))
			{
				collectedGenes.Remove(geneDef);
			}
		}
		public void DestroyGene(GeneDef geneDef)
		{
			if (consumedGenes.Contains(geneDef))
			{
				destroyedGenes.Add(geneDef);
				consumedGenes.Remove(geneDef);
			}
		}
		public void RemoveDestroyedGene(GeneDef geneDef)
		{
			if (destroyedGenes.Contains(geneDef))
			{
				destroyedGenes.Remove(geneDef);
			}
		}

		public override void TickInterval(int delta)
		{
			//if (!pawn.IsHashIntervalTick(63333))
			//{
			//	return;
			//}
			//UpdateMetabolism();
		}

		public override void PostRemove()
		{
			base.PostRemove();
			HediffUtility.TryRemoveHediff(Giver.metHediffDef, pawn);
		}

		public bool gizmoCollapse = WVC_Biotech.settings.geneGizmosDefaultCollapse;

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Collections.Look(ref consumedGenes, "eatedGenes", LookMode.Def);
			Scribe_Collections.Look(ref collectedGenes, "stolenGenes", LookMode.Def);
			Scribe_Collections.Look(ref destroyedGenes, "destroyedGenes", LookMode.Def);
			Scribe_Collections.Look(ref geneSetPresets, "geneSetPresets", LookMode.Deep);
			if (Scribe.mode == LoadSaveMode.LoadingVars && ((consumedGenes != null && consumedGenes.RemoveAll((GeneDef x) => x == null) > 0) || (collectedGenes != null && collectedGenes.RemoveAll((GeneDef x) => x == null) > 0) || (destroyedGenes != null && destroyedGenes.RemoveAll((GeneDef x) => x == null) > 0)))
			{
				Log.Warning("Removed null geneDef(s)");
			}
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				if (geneSetPresets == null)
				{
					geneSetPresets = new();
				}
				if (collectedGenes == null)
				{
					collectedGenes = new();
				}
				if (consumedGenes == null)
				{
					consumedGenes = new();
				}
				if (destroyedGenes == null)
				{
					destroyedGenes = new();
				}
				Debug_RemoveDupes();
			}
			Scribe_Values.Look(ref gizmoCollapse, "gizmoCollapse", WVC_Biotech.settings.geneGizmosDefaultCollapse);
			//Scribe_Values.Look(ref lastGeneObtainedTick, "lastGeneObtainedTick", -1);
		}

		public void Debug_RemoveDupes()
        {
			bool anyGeneRemoved = false;
			foreach (GeneDef geneDef in collectedGenes)
            {
				if (consumedGenes.Contains(geneDef))
                {
					consumedGenes.Remove(geneDef);
					anyGeneRemoved = true;
				}
			}
			foreach (GeneDef geneDef in consumedGenes)
			{
				if (destroyedGenes.Contains(geneDef))
				{
					destroyedGenes.Remove(geneDef);
					anyGeneRemoved = true;
				}
			}
			if (anyGeneRemoved)
			{
				Log.Warning("Removed duped geneDef(s)");
			}
		}

		public void Debug_ClearAllGenes()
		{
			collectedGenes = new();
			consumedGenes = new();
			destroyedGenes = new();
		}

		// public static float GetGeneWeight(GeneDef geneDef)
		// {
		// float weight = 1f / (geneDef.biostatCpx + geneDef.biostatMet + geneDef.biostatArc + 1f);
		// if (weight < 0f)
		// {
		// weight *= -1;
		// }
		// if (weight == 0f)
		// {
		// weight += 1f;
		// }
		// return weight;
		// }

		// public void Notify_PostStart(Gene_Shapeshifter shapeshiftGene)
		// {
		// }

		// public void Notify_PostShapeshift(Gene_Shapeshifter newShapeshiftGene)
		// {
		// foreach (Gene gene in pawn.genes.GenesListForReading)
		// {
		// AddGene(gene.def);
		// }
		// }

		public bool TryAddGenesFromList(List<Gene> genes)
		{
			if (genes.NullOrEmpty())
            {
				return false;
            }
			foreach (Gene gene in genes)
			{
				TryAddGene(gene.def);
			}
			return true;
		}

		public bool TryAddGenesFromList(List<GeneDef> genes)
		{
			if (genes.NullOrEmpty())
			{
				return false;
			}
			foreach (GeneDef gene in genes)
			{
				TryAddGene(gene);
			}
			return true;
		}

		public bool TryGetGene(List<GeneDef> genes, out GeneDef result)
		{
			result = null;
			if (genes.Where((GeneDef x) => !AllGenes.Contains(x)).TryRandomElementByWeight((GeneDef gene) => 1f + gene.selectionWeight, out result))
			{
				TryAddGene(result);
				return true;
			}
			return false;
		}

		public bool TryGetGene(Pawn victim, out GeneDef result)
		{
			return TryGetGene(XaG_GeneUtility.ConvertToDefs(victim.genes.GenesListForReading), out result);
		}

		public void Notify_Bloodfeed(Pawn victim)
		{
			GetGeneFromHuman(victim);
		}

		public void GetGeneFromHuman(Pawn victim)
		{
			List<Gene> genes = victim?.genes?.GenesListForReading;
			if (genes.NullOrEmpty())
			{
				return;
			}
			if (TryGetRandomHumanGene(victim, out GeneDef result) || TryGetGene(XaG_GeneUtility.ConvertToDefs(genes), out result))
			{
				Messages.Message("WVC_XaG_GeneGeneticThief_GeneCopied".Translate(pawn.NameShortColored, result.label), pawn, MessageTypeDefOf.NeutralEvent, historical: false);
			}
		}

		private bool TryGetRandomHumanGene(Pawn victim, out GeneDef result)
		{
			result = null;
			if (Rand.Chance(0.5f) && victim?.genes?.Xenotype == XenotypeDefOf.Baseliner || Rand.Chance(0.12f))
			{
				//if (!Props.humanBasicGenes.NullOrEmpty() && Props.humanBasicGenes.Where((GeneDef x) => !AllGenes.Contains(x)).TryRandomElement(out result))
				//{
				//	TryAddGene(result);
				//	return true;
				//}
				if (ListsUtility.GetHumanGeneDefs().Where((GeneDef x) => !AllGenes.Contains(x)).TryRandomElement(out result))
				{
					TryAddGene(result);
					return true;
				}
				return false;
			}
			return false;
		}

		public override void Notify_IngestedThing(Thing thing, int numTaken)
		{
			// base.Notify_IngestedThing(thing, numTaken);
			if (!Active)
            {
				return;
            }
			if (thing is Corpse corpse)
			{
				if (corpse.InnerPawn.IsHuman())
				{
					GetGeneFromHuman(corpse.InnerPawn);
				}
				else if (thing.def == MainDefOf.Devourer?.race)
				{
					GeneDef geneDef = DefDatabase<GeneDef>.AllDefsListForReading?.Where((geneDef) => geneDef.IsGeneDefOfType<Gene_Devourer>())?.RandomElement();
					if (TryAddGene(geneDef))
					{
						Messages.Message("WVC_XaG_GeneGeneticThief_GeneCopied".Translate(pawn.NameShortColored, geneDef.label), pawn, MessageTypeDefOf.NeutralEvent, historical: false);
					}
				}
				else if (Rand.Chance(0.04f) || thing.def == PawnKindDefOf.Chimera?.race)
				{
					GetRandomGene();
				}
				return;
			}
			if (thing.def.IsMeat)
			{
				if (Rand.Chance(0.01f))
				{
					GetRandomGene();
				}
				return;
			}
			CompIngredients compIngredients = thing.TryGetComp<CompIngredients>();
			if (compIngredients?.ingredients.NullOrEmpty() == false)
			{
				if (Rand.Chance(0.004f) && compIngredients.ingredients.Any((ThingDef thing) => thing.IsMeat))
				{
					GetRandomGene();
				}
			}
		}

		public void GetRandomGene()
		{
			GeneDef result = DefDatabase<GeneDef>.AllDefsListForReading.Where((GeneDef x) => x.biostatArc == 0 && x.selectionWeight > 0f && x.canGenerateInGeneSet && !AllGenes.Contains(x)).RandomElement();
			TryAddGene(result);
			Messages.Message("WVC_XaG_GeneGeneticThief_GeneObtained".Translate(pawn.NameShortColored, result.label), pawn, MessageTypeDefOf.NeutralEvent, historical: false);
		}

        //public static bool IsHumanCosmetic(GeneDef geneDef)
        //{
        //    return geneDef.IsVanillaDef() && !geneDef.canGenerateInGeneSet && geneDef.biostatCpx == 0 && geneDef.biostatMet == 0 && geneDef.biostatArc == 0 && !XaG_GeneUtility.IsCosmeticGene(geneDef);
        //}

        public virtual void UpdateChimeraXenogerm(List<GeneDef> implantedGenes)
		{
			Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.XenogermReplicating);
			if (firstHediffOfDef != null)
			{
				List<Ability> xenogenesAbilities = MiscUtility.GetXenogenesAbilities(pawn);
				foreach (Ability ability in xenogenesAbilities)
				{
					if (!ability.HasCooldown)
					{
						continue;
					}
					ability.StartCooldown(ability.def.cooldownTicksRange.RandomInRange);
				}
				//pawn.health.RemoveHediff(firstHediffOfDef);
			}
			if (implantedGenes.Empty())
			{
				return;
			}
			XaG_GeneUtility.GetBiostatsFromList(implantedGenes, out int cpx, out int met, out int _);
			int architeCount = implantedGenes.Where((geneDef) => geneDef.biostatArc != 0).ToList().Count;
            int nonArchiteCount = implantedGenes.Count - architeCount;
            int days = Mathf.Clamp(nonArchiteCount + (architeCount * 3) - met + (int)(cpx * 0.2f), 0, 999);
            int count = days * 120000;
            //int count = (implantedGenes.Count + 1) * 180000;
			ReimplanterUtility.XenogermReplicating_WithCustomDuration(pawn, new((int)(count * 0.8f), (int)(count * 1.1f)), firstHediffOfDef);
			// pawn.health.AddHediff(HediffDefOf.XenogermReplicating);
		}

        // public virtual void ClearChimeraXenogerm()
        // {
        // Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.XenogermReplicating);
        // bool clearXenogerm = true;
        // if (firstHediffOfDef != null)
        // {
        // List<Ability> xenogenesAbilities = MiscUtility.GetXenogenesAbilities(pawn);
        // foreach (Ability ability in xenogenesAbilities)
        // {
        // if (ability.OnCooldown)
        // {
        // clearXenogerm = false;
        // break;
        // }
        // }
        // if (clearXenogerm)
        // {
        // pawn.health.RemoveHediff(firstHediffOfDef);
        // }
        // }
        // }

        // =================

        public int XenogenesLimit
        {
            get
            {
                return (int)pawn.GetStatValue(Giver.statDef);
            }
		}

		public int GetStatFromStatModifiers(StatDef statDef, List<StatModifier> statOffsets, List<StatModifier> statFactors)
		{
			float value = 0;
			if (statOffsets != null)
			{
				foreach (StatModifier statModifier in statOffsets)
				{
					if (statModifier.stat == statDef)
					{
						value += statModifier.value;
					}
				}
			}
			if (statFactors != null)
			{
				foreach (StatModifier statModifier in statFactors)
				{
					if (statModifier.stat == statDef)
					{
						value *= statModifier.value;
					}
				}
			}
			return (int)value;
		}

		public bool CanBeUsed
		{
			get
			{
				return XenogenesLimit > 0;
			}
		}

		public bool CanImplantGenes
		{
			get
			{
				return XenogenesLimit > pawn.genes.Xenogenes.Count;
			}
		}

		public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
		{
			StatDef stat = Giver.statDef;
			string supportedGenes = DefDatabase<GeneDef>.AllDefsListForReading.Where((GeneDef x) => x.statFactors != null && x.statFactors.StatListContains(stat) || x.statOffsets != null && x.statOffsets.StatListContains(stat)).Select((GeneDef x) => x.LabelCap.ToString()).ToLineList(" - ");
			yield return new StatDrawEntry(StatCategoryDefOf.Genetics, stat.LabelCap, pawn.GetStatValue(stat).ToString(), stat.description + "\n\n" + supportedGenes, stat.displayPriorityInCategory);
		}

		// =================

		public virtual void UpdateMetabolism()
		{
			HediffUtility.TryAddOrUpdMetabolism(Giver.metHediffDef, pawn, this);
		}

        public virtual void DoEffects()
		{
			if (pawn.Map == null)
			{
				return;
			}
			//WVC_GenesDefOf.CocoonDestroyed.SpawnAttached(pawn, pawn.Map).Trigger(pawn, null);
			MiscUtility.DoShapeshiftEffects_OnPawn(pawn);
			if (!Props.soundDefOnImplant.NullOrUndefined())
			{
				Props.soundDefOnImplant.PlayOneShot(SoundInfo.InMap(pawn));
			}
		}

		public void DoEffects(Pawn pawn)
		{
			DoEffects();
		}

	}

	//[Obsolete]
	//public class Gene_BloodChimera : Gene_Chimera
	//{

	//}

}
