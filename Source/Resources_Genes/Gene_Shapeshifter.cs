using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Shapeshifter : Gene, IGeneOverridden, IGenePregnantHuman
	{

		public GeneExtension_Undead Props => def?.GetModExtension<GeneExtension_Undead>();

		public GeneExtension_Giver Giver => def?.GetModExtension<GeneExtension_Giver>();

		// [Obsolete]
		public bool xenogermComaAfterShapeshift = true;
		public bool genesRegrowAfterShapeshift = true;

		// private ShapeshiftModeDef currentMode;

		// private List<ShapeshiftModeDef> unlockedModes;

		// public List<ShapeshiftModeDef> UnlockedModes
		// {
			// get
			// {
				// if (unlockedModes == null)
				// {
					// unlockedModes = new();
				// }
				// return unlockedModes;
			// }
		// }

		// public ShapeshiftModeDef ShiftMode => currentMode;

		// public void SetMode(ShapeshiftModeDef newMode)
		// {
			// if (currentMode != null)
			// {
				// HediffUtility.RemoveHediffsFromList(pawn, currentMode.hediffDefs);
			// }
			// currentMode = newMode;
			// if (currentMode != null)
			// {
				// foreach (HediffDef hediffDef in currentMode.hediffDefs)
				// {
					// HediffUtility.TryAddHediff(hediffDef, pawn, def);
				// }
			// }
		// }

		// public void UnlockMode(ShapeshiftModeDef newMode)
		// {
			// if (!unlockedModes.Contains(newMode))
			// {
				// unlockedModes.Add(newMode);
			// }
		// }

		public override void PostAdd()
		{
			base.PostAdd();
			if (pawn.genes.IsXenogene(this))
			{
				pawn.genes.RemoveGene(this);
				pawn.genes.AddGene(this.def, false);
				return;
			}
			//if (!pawn.Spawned)
			//{
			//	UndeadUtility.AddRandomTraitFromListWithChance(pawn, Props);
			//}
			// Reset();
		}

		// HeritGenes

		// public int maxEvolveGenes = 0;
		// public int currentEvolveGenes = 0;
		// public List<GeneDef> heritableGenes;
		// private List<GeneDef> oldHeritableGenes;

		// public void AddHeritableGenes()
		// {
			// if (heritableGenes.NullOrEmpty())
			// {
				// return;
			// }
			// List<GeneDef> pawnGenes = XaG_GeneUtility.ConvertGenesInGeneDefs(pawn.genes.GenesListForReading);
			// foreach (GeneDef heritableGeneDef in heritableGenes)
			// {
				// if (Dialog_CreateChimera.ConflictWith(heritableGeneDef, pawnGenes))
				// {
					// continue;
				// }
				// pawn.genes.AddGene(heritableGeneDef, false);
				// oldHeritableGenes.Add(heritableGeneDef);
			// }
		// }

		// public void RemoveHeritableGenes()
		// {
			// if (oldHeritableGenes.NullOrEmpty())
			// {
				// return;
			// }
			// foreach (GeneDef heritableGeneDef in oldHeritableGenes)
			// {
				// Gene gene = pawn.genes.GetGene(heritableGeneDef);
				// if (gene != null)
				// {
					// continue;
				// }
				// pawn.genes.RemoveGene(gene);
			// }
			// oldHeritableGenes = new();
		// }

		// HeritGenes

		// public override void Reset()
		// {
			// SetMode(Props.defaultShapeMode);
			// if (!xenogermComaAfterShapeshift)
			// {
				// UnlockMode(ShapeshiftModeDefOf.WVC_Safeshift);
				// xenogermComaAfterShapeshift = true;
			// }
			// if (HediffUtility.HasAnyHediff(Props.duplicateHediffs, pawn))
			// {
				// UnlockMode(ShapeshiftModeDefOf.WVC_Duplicate);
			// }
		// }

		private Gizmo gizmo;

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (XaG_GeneUtility.SelectorActiveFactionMap(pawn, this))
			{
				yield break;
			}
			if (gizmo == null)
			{
				gizmo = (Gizmo)Activator.CreateInstance(def.resourceGizmoType, this);
			}
			yield return gizmo;
		}

		public void Notify_OverriddenBy(Gene overriddenBy)
		{
			RemoveHediffs();
		}

		public void RemoveHediffs()
		{
			// if (currentMode != null)
			// {
				// HediffUtility.RemoveHediffsFromList(pawn, currentMode.hediffDefs);
			// }
			HediffUtility.RemoveHediffsFromList(pawn, Giver?.hediffDefs);
		}

		public void Notify_Override()
		{
		}

		public void Notify_PregnancyStarted(Hediff_Pregnant pregnancy)
		{
			GeneSet geneSet = pregnancy.geneSet;
			geneSet.AddGene(def);
			geneSet.SortGenes();
		}

		public override void PostRemove()
		{
			base.PostRemove();
			// RemoveHeritableGenes();
			if (WVC_Biotech.settings.shapeshifterGeneUnremovable)
			{
				pawn.genes.AddGene(this.def, false);
				// UndeadUtility.TryTransferGeneStats(this, pawn.genes.GetFirstGeneOfType<Gene_Shapeshifter>());
				Gene_Shapeshifter newShifter = pawn.genes.GetFirstGeneOfType<Gene_Shapeshifter>();
				newShifter.UpdateForNewGene(this);
			}
			RemoveHediffs();
		}

		public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
		{
			yield return new StatDrawEntry(StatCategoryDefOf.Genetics, "WVC_XaG_GeneShapeshifter_XenogermComaAfterShapeshift_Label".Translate(), xenogermComaAfterShapeshift.ToStringYesNo(), "WVC_XaG_GeneShapeshifter_XenogermComaAfterShapeshift_Desc".Translate(), 200);
			yield return new StatDrawEntry(StatCategoryDefOf.Genetics, "WVC_XaG_GeneShapeshifter_GenesRegrowAfterShapeshift_Label".Translate(), genesRegrowAfterShapeshift.ToStringYesNo(), "WVC_XaG_GeneShapeshifter_GenesRegrowAfterShapeshift_Desc".Translate(), 220);
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref xenogermComaAfterShapeshift, "xenogerminationComaAfterShapeshift", defaultValue: true);
			Scribe_Values.Look(ref genesRegrowAfterShapeshift, "genesRegrowAfterShapeshift", defaultValue: true);
			// Scribe_Values.Look(ref maxEvolveGenes, "maxEvolveGenes", 0);
		}

		public virtual void UpdateForNewGene(Gene_Shapeshifter oldShapeshifter)
		{
			xenogermComaAfterShapeshift = oldShapeshifter.xenogermComaAfterShapeshift;
			genesRegrowAfterShapeshift = oldShapeshifter.genesRegrowAfterShapeshift;
			// maxEvolveGenes = oldShapeshifter.maxEvolveGenes;
		}

		// Reimplanter

		public virtual void AddGene(GeneDef geneDef, bool inheritable)
		{
			if (!geneDef.ConflictsWith(this.def))
			{
				pawn.genes.AddGene(geneDef, !inheritable);
			}
		}

		public virtual void RemoveGene(Gene gene)
		{
			if (gene != this)
			{
				pawn?.genes?.RemoveGene(gene);
			}
		}

		public virtual void Shapeshift(XenotypeDef xenotypeDef, bool xenogenes = true, bool doubleXenotypes = true)
		{
			if (doubleXenotypes && !xenotypeDef.doubleXenotypeChances.NullOrEmpty() && Rand.Value < xenotypeDef.doubleXenotypeChances.Sum((XenotypeChance x) => x.chance) && xenotypeDef.doubleXenotypeChances.TryRandomElementByWeight((XenotypeChance x) => x.chance, out var result))
			{
				Reimplant(result.xenotype, false);
			}
			Reimplant(xenotypeDef, xenogenes);
			if (xenogermComaAfterShapeshift)
			{
				pawn.health.AddHediff(HediffDefOf.XenogerminationComa);
			}
			if (genesRegrowAfterShapeshift)
			{
				GeneUtility.UpdateXenogermReplication(pawn);
			}
			if (ModLister.IdeologyInstalled)
			{
				Find.HistoryEventsManager.RecordEvent(new HistoryEvent(WVC_GenesDefOf.WVC_Shapeshift, pawn.Named(HistoryEventArgsNames.Doer)));
			}
			DoEffects();
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

		private void Reimplant(XenotypeDef xenotypeDef, bool xenogenes = true)
		{
			Pawn_GeneTracker recipientGenes = pawn.genes;
			if (recipientGenes.Xenogenes.NullOrEmpty() || xenogenes)
			{
				ReimplanterUtility.SetXenotypeDirect(null, pawn, xenotypeDef, true);
			}
			if (xenogenes || !xenotypeDef.inheritable || xenotypeDef == XenotypeDefOf.Baseliner)
			{
				foreach (Gene gene in recipientGenes.Xenogenes.ToList())
				{
					RemoveGene(gene);
				}
			}
			if (xenotypeDef.inheritable || xenotypeDef == XenotypeDefOf.Baseliner)
			{
				foreach (Gene gene in recipientGenes.Endogenes.ToList())
				{
					RemoveGene(gene);
				}
			}
			foreach (GeneDef geneDef in xenotypeDef.genes)
			{
				AddGene(geneDef, xenotypeDef.inheritable);
			}
			ReimplanterUtility.TrySetSkinAndHairGenes(pawn);
		}

		// Shapeshift

		// - Shapeshift now takes body size changes into account. The pawn's hunger will increase or decrease relative to the difference in size.
			// * Please note that it is the actual body size that is taken into account, not the visual one.
		// - Shapeshift now transfers resource genes values between xenotypes. But only if the new xeno has this resources.
			// * This only affects resource genes, such as hemogen. Deathrest capacity and similar ones will not be transferred.
		// - Shapeshift now transfers genes needs between xenotypes. But only if the new xeno has this needs.

		public virtual void PreShapeshift(Gene_Shapeshifter shapeshiftGene, bool genesRegrowing)
		{
			// ResourceCache();
			if (!genesRegrowing)
			{
				UndeadUtility.Notify_PreShapeshift(shapeshiftGene);
			}
		}

		// private float cachedPawnBodySize = 1f;
		// private Dictionary<Type, float> cachedPawnGeneResources;
		// private Dictionary<Need, float> cachedPawnNeeds;

		// private void ResourceCache()
		// {
		// cachedPawnBodySize = pawn.BodySize;
		// cachedPawnGeneResources = new();
		// cachedPawnNeeds = new();
		// foreach (Gene gene in pawn.genes.GenesListForReading)
		// {
		// if (gene is Gene_Resource resource && gene.Active)
		// {
		// cachedPawnGeneResources[resource.def.geneClass] = resource.Value;
		// }
		// }
		// foreach (Need need in pawn.needs.AllNeeds)
		// {
		// cachedPawnNeeds[need] = need.CurLevel;
		// }
		// }

		public virtual void PostShapeshift(Gene_Shapeshifter shapeshiftGene, bool genesRegrowing)
		{
			// ResourceTransfer();
			if (!genesRegrowing)
			{
				UndeadUtility.Notify_PostShapeshift(shapeshiftGene);
				UndeadUtility.Notify_PostShapeshift_Traits(shapeshiftGene);
			}
			XaG_GeneUtility.CheckAllOverrides(pawn);
		}

		// private void ResourceTransfer()
		// {
		// foreach (Need need in pawn.needs.AllNeeds)
		// {
		// foreach (var item in cachedPawnNeeds)
		// {
		// if (need.def == item.Key.def)
		// {
		// need.CurLevel = item.Value;
		// }
		// }
		// }
		// if (cachedPawnBodySize != pawn.BodySize)
		// {
		// UndeadUtility.OffsetNeedFood(pawn, cachedPawnBodySize - pawn.BodySize);
		// }
		// foreach (Gene gene in pawn.genes.GenesListForReading)
		// {
		// if (!gene.Active)
		// {
		// continue;
		// }
		// if (gene is Gene_Resource resource)
		// {
		// foreach (var item in cachedPawnGeneResources)
		// {
		// if (resource.def.geneClass == item.Key)
		// {
		// resource.Value = item.Value;
		// }
		// }
		// }
		// }
		// cachedPawnBodySize = pawn.BodySize;
		// cachedPawnNeeds = null;
		// cachedPawnGeneResources = null;
		// }

	}

}
