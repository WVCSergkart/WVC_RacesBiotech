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

		public bool xenogermComaAfterShapeshift = true;
		public bool genesRegrowAfterShapeshift = true;

		public override void PostAdd()
		{
			base.PostAdd();
			if (pawn.genes.IsXenogene(this))
			{
				pawn.genes.RemoveGene(this);
				pawn.genes.AddGene(this.def, false);
				return;
			}
		}

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
			// if (WVC_Biotech.settings.shapeshifterGeneUnremovable)
			// {
				// pawn.genes.AddGene(this.def, false);
				// Gene_Shapeshifter newShifter = pawn.genes.GetFirstGeneOfType<Gene_Shapeshifter>();
				// newShifter.UpdateForNewGene(this);
			// }
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
					if (!xenotypeDef.inheritable && xenotypeDef.genes.Contains(gene.def))
					{
						continue;
					}
					RemoveGene(gene);
				}
			}
			if (xenotypeDef.inheritable || xenotypeDef == XenotypeDefOf.Baseliner)
			{
				foreach (Gene gene in recipientGenes.Endogenes.ToList())
				{
					if (xenotypeDef.inheritable && xenotypeDef.genes.Contains(gene.def))
					{
						continue;
					}
					RemoveGene(gene);
				}
			}
			foreach (GeneDef geneDef in xenotypeDef.genes)
			{
				if (!xenotypeDef.inheritable && XaG_GeneUtility.HasXenogene(geneDef, pawn) || xenotypeDef.inheritable && XaG_GeneUtility.HasEndogene(geneDef, pawn))
				{
					continue;
				}
				AddGene(geneDef, xenotypeDef.inheritable);
			}
			ReimplanterUtility.TrySetSkinAndHairGenes(pawn);
		}

		// Shapeshift

		public virtual void PreShapeshift(Gene_Shapeshifter shapeshiftGene, bool genesRegrowing)
		{
			if (!genesRegrowing)
			{
				GeneResourceUtility.Notify_PreShapeshift(shapeshiftGene);
			}
		}

		public virtual void PostShapeshift(Gene_Shapeshifter shapeshiftGene, bool genesRegrowing)
		{
			if (!genesRegrowing)
			{
				GeneResourceUtility.Notify_PostShapeshift(shapeshiftGene);
				GeneResourceUtility.Notify_PostShapeshift_Traits(shapeshiftGene);
			}
			ReimplanterUtility.PostImplantDebug(pawn);
		}

	}

}
