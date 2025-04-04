using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Shapeshifter : Gene, IGeneOverridden, IGenePregnantHuman, IGeneWithEffects
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

		public override void Tick()
		{

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

		public virtual void Shapeshift(XenotypeHolder xenotypeHolder, bool xenogenes = true)
		{
			//if (doubleXenotypes && !xenotypeHolder.xenotypeDef.doubleXenotypeChances.NullOrEmpty() && Rand.Value < xenotypeHolder.xenotypeDef.doubleXenotypeChances.Sum((XenotypeChance x) => x.chance) && xenotypeHolder.xenotypeDef.doubleXenotypeChances.TryRandomElementByWeight((XenotypeChance x) => x.chance, out var result))
			//{
			//	Reimplant(result.xenotype, false);
			//}
			Reimplant(xenotypeHolder, xenogenes);
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
				Find.HistoryEventsManager.RecordEvent(new HistoryEvent(WVC_HistoryEventDefOf.WVC_Shapeshift, pawn.Named(HistoryEventArgsNames.Doer)));
			}
			DoEffects();
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

		private void Reimplant(XenotypeHolder xenotypeHolder, bool xenogenes = true)
		{
			Pawn_GeneTracker recipientGenes = pawn.genes;
			if (recipientGenes.Xenogenes.NullOrEmpty() || xenogenes)
			{
				ReimplanterUtility.SetXenotypeDirect(null, pawn, xenotypeHolder.xenotypeDef, true);
				if (!xenotypeHolder.name.NullOrEmpty() || xenotypeHolder.iconDef != null)
				{
					pawn.genes.xenotypeName = xenotypeHolder.name;
					pawn.genes.iconDef = xenotypeHolder.iconDef;
				}
			}
			if (xenogenes || !xenotypeHolder.inheritable || xenotypeHolder.Baseliner)
			{
				foreach (Gene gene in recipientGenes.Xenogenes.ToList())
				{
					if (!xenotypeHolder.inheritable && xenotypeHolder.genes.Contains(gene.def))
					{
						continue;
					}
					RemoveGene(gene);
				}
			}
			if (xenotypeHolder.inheritable || xenotypeHolder.Baseliner)
			{
				foreach (Gene gene in recipientGenes.Endogenes.ToList())
				{
					if (xenotypeHolder.inheritable && xenotypeHolder.genes.Contains(gene.def))
					{
						continue;
					}
					RemoveGene(gene);
				}
			}
			foreach (GeneDef geneDef in xenotypeHolder.genes)
			{
				if (!xenotypeHolder.inheritable && XaG_GeneUtility.HasXenogene(geneDef, pawn) || xenotypeHolder.inheritable && XaG_GeneUtility.HasEndogene(geneDef, pawn))
				{
					continue;
				}
				AddGene(geneDef, xenotypeHolder.inheritable);
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
