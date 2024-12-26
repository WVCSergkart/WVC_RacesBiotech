using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

    public class Gene_Chimera : Gene, IGeneBloodfeeder, IGeneOverridden
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
			Local_AddOrRemoveHediff();
			if (Current.ProgramState == ProgramState.Playing)
			{
				return;
			}
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
			GetToolGene();
		}

		public void GetToolGene()
		{
			if (WVC_Biotech.settings.enable_chimeraStartingTools && Props?.chimeraGenesTools != null && Props.chimeraGenesTools.Where((GeneDef geneDef) => !AllGenes.Contains(geneDef)).TryRandomElement(out GeneDef result))
			{
				TryAddGene(result);
				if (pawn.Spawned)
				{
					Messages.Message("WVC_XaG_GeneGeneticThief_GeneObtained".Translate(pawn.NameShortColored, result.label), pawn, MessageTypeDefOf.NeutralEvent, historical: false);
				}
			}
		}

		public void GetSuperToolGene()
		{
			if (WVC_Biotech.settings.enable_chimeraStartingTools && Props?.chimeraOneManArmyGenes != null && Props.chimeraOneManArmyGenes.Where((GeneDef geneDef) => !AllGenes.Contains(geneDef)).TryRandomElement(out GeneDef result))
			{
				TryAddGene(result);
				if (pawn.Spawned)
				{
					Messages.Message("WVC_XaG_GeneGeneticThief_GeneObtained".Translate(pawn.NameShortColored, result.label), pawn, MessageTypeDefOf.NeutralEvent, historical: false);
				}
			}
		}

		public void Local_AddOrRemoveHediff()
		{
			HediffUtility.TryAddOrRemoveHediff(Giver.hediffDefName, pawn, this, Giver.bodyparts);
		}

		public void Notify_OverriddenBy(Gene overriddenBy)
		{
			HediffUtility.TryRemoveHediff(Giver.hediffDefName, pawn);
		}

		public void Notify_Override()
		{
			Local_AddOrRemoveHediff();
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

		public bool TryAddGene(GeneDef geneDef)
		{
			if (!collectedGenes.Contains(geneDef))
			{
				collectedGenes.Add(geneDef);
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

		public override void Tick()
		{
			//base.Tick();
			if (!pawn.IsHashIntervalTick(63333))
			{
				return;
			}
			Local_AddOrRemoveHediff();
		}

		public override void PostRemove()
		{
			base.PostRemove();
			HediffUtility.TryRemoveHediff(Giver.hediffDefName, pawn);
		}

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
			}
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

		public void Notify_Bloodfeed(Pawn victim)
		{
			GetGeneFromHuman(victim);
		}

		private void GetGeneFromHuman(Pawn victim)
		{
			List<Gene> genes = victim?.genes?.GenesListForReading;
			if (genes.NullOrEmpty())
			{
				return;
			}
			if (TryGetRandomHumanGene(victim, out GeneDef result) || TryGetGene(XaG_GeneUtility.ConvertGenesInGeneDefs(genes), out result))
			{
				Messages.Message("WVC_XaG_GeneGeneticThief_GeneCopied".Translate(pawn.NameShortColored, result.label), pawn, MessageTypeDefOf.NeutralEvent, historical: false);
			}
		}

		private bool TryGetRandomHumanGene(Pawn victim, out GeneDef result)
		{
			result = null;
			if (Rand.Chance(0.5f) && victim?.genes?.Xenotype == XenotypeDefOf.Baseliner || Rand.Chance(0.12f))
			{
				if (!Props.humanBasicGenes.NullOrEmpty() && Props.humanBasicGenes.Where((GeneDef x) => !AllGenes.Contains(x)).TryRandomElement(out result))
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
			if (thing is Corpse corpse)
			{
				if (corpse.InnerPawn.IsHuman())
				{
					GetGeneFromHuman(corpse.InnerPawn);
				}
				else if (Rand.Chance(0.04f) || thing.def == PawnKindDefOf.Chimera.race)
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

		private void GetRandomGene()
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
				pawn.health.RemoveHediff(firstHediffOfDef);
			}
            int architeCount = implantedGenes.Where((geneDef) => geneDef.biostatArc != 0).ToList().Count;
            int nonArchiteCount = implantedGenes.Count - architeCount;
            int count = (nonArchiteCount + (architeCount * 3)) * 120000;
            //int count = (implantedGenes.Count + 1) * 180000;
			ReimplanterUtility.XenogermReplicating_WithCustomDuration(pawn, new((int)(count * 0.8f), (int)(count * 1.1f)));
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

		public virtual void UpdateMetabolism()
        {
			GeneResourceUtility.UpdMetabolism(pawn);
        }

        public void DoEffects()
		{
			if (pawn.Map == null)
			{
				return;
			}
			WVC_GenesDefOf.CocoonDestroyed.SpawnAttached(pawn, pawn.Map).Trigger(pawn, null);
			if (!Props.soundDefOnImplant.NullOrUndefined())
			{
				Props.soundDefOnImplant.PlayOneShot(SoundInfo.InMap(pawn));
			}
		}

	}

	//[Obsolete]
	//public class Gene_BloodChimera : Gene_Chimera
	//{

	//}

}
