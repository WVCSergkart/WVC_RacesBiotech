using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Shapeshifter : Gene, IGeneOverridden, IGenePregnantHuman, IGeneWithEffects, IGeneMetabolism
	{
		//public string RemoteActionName => "WVC_HideShow".Translate();

		//public string RemoteActionDesc => "WVC_XaG_Shapeshifter_HideShowDesc".Translate();

		//public void RemoteControl_Action()
		//{
		//	gizmoCollapse = !gizmoCollapse;
		//	SetupRemoteContollers(gizmoCollapse);
		//}

		//public bool RemoteControl_Hide => !Active;

		//public bool RemoteControl_Enabled
		//{
		//	get
		//	{
		//		return true;
		//	}
		//	set
		//	{

		//	}
		//}

		//public void RemoteControl_Recache()
		//{

		//}

		//private void SetupRemoteContollers(bool setAllTo)
		//{
		//	XaG_UiUtility.SetAllRemoteControllersTo(pawn, setAllTo);
		//}

		// ===================

		//public override bool Active => !base.Overridden;

		public GeneExtension_Undead Props => def?.GetModExtension<GeneExtension_Undead>();

		public GeneExtension_Giver Giver => def?.GetModExtension<GeneExtension_Giver>();

		//public bool xenogermComaAfterShapeshift = true;
		public bool genesRegrowAfterShapeshift = true;

		public override void PostAdd()
		{
			base.PostAdd();
			UpdateMetabolism();
			//	if (pawn.genes.IsXenogene(this))
			//	{
			//		pawn.genes.RemoveGene(this);
			//		pawn.genes.AddGene(this.def, false);
			//		return;
			//	}
		}

		private Gizmo gizmo;
		//public bool remoteControllerCached = false;

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (XaG_GeneUtility.SelectorActiveFactionMap(pawn, this))
			{
				yield break;
			}
			//if (!remoteControllerCached)
   //         {
   //             SetupRemoteContollers(false);
   //         }
            if (gizmo == null)
			{
				gizmo = (Gizmo)Activator.CreateInstance(def.resourceGizmoType, this);
			}
			yield return gizmo;
		}

        public void Notify_OverriddenBy(Gene overriddenBy)
		{
			RemoveHediffs();
			//SetupRemoteContollers(true);
		}

		public void RemoveHediffs()
		{
			HediffUtility.RemoveHediffsFromList(pawn, Giver?.hediffDefs);
		}

		public void Notify_Override()
		{
			UpdateMetabolism();
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
			//SetupRemoteContollers(true);
		}

		//public StatDef ShiftStatDef => Props.shiftStatDef;

		//public float MinGenesMatch
		//{
		//	get
		//	{
		//		return pawn.GetStatValue(ShiftStatDef);
		//	}
		//}

		//private string cachedGenesMatch_UI;
		//public string MinGenesMatch_UI
		//{
		//	get
		//	{
		//		if (cachedGenesMatch_UI.NullOrEmpty())
		//		{
		//			cachedGenesMatch_UI = MinGenesMatch.ToStringPercent();
		//		}
		//		return cachedGenesMatch_UI;
		//	}
		//}

		//public void Notify_GenesChanged(Gene changedGene)
		//{
		//	cachedGenesMatch_UI = null;
		//}

		public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
		{
			//cachedGenesMatch_UI = null;
			//yield return new StatDrawEntry(StatCategoryDefOf.Genetics, ShiftStatDef.LabelCap, MinGenesMatch_UI, ShiftStatDef.description, 160);
			yield return new StatDrawEntry(StatCategoryDefOf.Genetics, "WVC_XaG_GeneShapeshifter_GenesRegrowAfterShapeshift_Label".Translate(), genesRegrowAfterShapeshift.ToStringYesNo(), "WVC_XaG_GeneShapeshifter_GenesRegrowAfterShapeshift_Desc".Translate(), 220);
		}

		public bool gizmoCollapse = WVC_Biotech.settings.geneGizmosDefaultCollapse;

		public override void ExposeData()
		{
			base.ExposeData();
			//Scribe_Values.Look(ref xenogermComaAfterShapeshift, "xenogerminationComaAfterShapeshift", defaultValue: true);
			Scribe_Values.Look(ref genesRegrowAfterShapeshift, "genesRegrowAfterShapeshift", defaultValue: true);
			// Scribe_Values.Look(ref maxEvolveGenes, "maxEvolveGenes", 0);
			Scribe_Values.Look(ref geneticMaterial, "geneticMaterial", 0);
			Scribe_Values.Look(ref gizmoCollapse, "gizmoCollapse", WVC_Biotech.settings.geneGizmosDefaultCollapse);
		}

		public virtual void UpdateForNewGene(Gene_Shapeshifter oldShapeshifter)
		{
			//xenogermComaAfterShapeshift = oldShapeshifter.xenogermComaAfterShapeshift;
			genesRegrowAfterShapeshift = oldShapeshifter.genesRegrowAfterShapeshift;
			geneticMaterial = oldShapeshifter.geneticMaterial;
			gizmoCollapse = oldShapeshifter.gizmoCollapse;
		}

		public void ChangeType_GermlineXenogerm()
        {
            bool xenogene = pawn.genes.IsXenogene(this);
            if (!XaG_GeneUtility.TryRemoveAllConflicts(pawn, def))
            {
                return;
            }
            pawn.genes.AddGene(def, !xenogene);
            Gene_Shapeshifter gene_Shapeshifter = pawn.genes.GetFirstGeneOfType<Gene_Shapeshifter>();
            if (gene_Shapeshifter != null)
            {
                gene_Shapeshifter.UpdateForNewGene(this);
                gene_Shapeshifter.AddXenogermReplicating(new() { def });
                gene_Shapeshifter.DoEffects();
                Messages.Message("WVC_XaG_DialogEditShiftGenes_ChangeTypeMessage".Translate(), pawn, MessageTypeDefOf.NeutralEvent, historical: false);
            }
        }

        private int geneticMaterial = 0;

		public int GeneticMaterial => geneticMaterial;

		public bool TryOffsetResource(int count)
        {
			if (!genesRegrowAfterShapeshift && count > 0)
            {
				return false;
            }
            geneticMaterial += count;
            if (geneticMaterial < 0)
            {
                geneticMaterial = 0;
			}
			return true;
		}

		public bool TryOffsetResource(Gene gene)
		{
			return TryOffsetResource((int)((gene.def.biostatCpx * 0.5f) + (gene.def.biostatArc * 0.2f)));
		}

		public bool TryConsumeResource(int count)
        {
			if (count > 0)
            {
				count *= -1;
			}
            if ((geneticMaterial + count) >= 0)
            {
                return TryOffsetResource(count);
            }
            return false;
		}

        public void TryForceGene(GeneDef geneDef, bool inheritable)
        {
            if (!geneDef.ConflictsWith(this.def) && XaG_GeneUtility.TryRemoveAllConflicts(pawn, geneDef))
			{
				pawn.genes.AddGene(geneDef, !inheritable);
				AddXenogermReplicating(new() { geneDef });
				//UpdateMetabolism();
			}
        }

        // Reimplanter

        //public virtual void AddGene(GeneDef geneDef, bool inheritable)
        //{
        //	if (!geneDef.ConflictsWith(this.def))
        //	{
        //		pawn.genes.AddGene(geneDef, !inheritable);
        //	}
        //}

        //public virtual void RemoveGene(Gene gene)
        //{
        //	if (gene != this)
        //	{
        //		pawn?.genes?.RemoveGene(gene);
        //	}
        //}

        public virtual void Shapeshift(XenotypeHolder xenotypeHolder, bool xenogenes = true)
		{
			//if (doubleXenotypes && !xenotypeHolder.xenotypeDef.doubleXenotypeChances.NullOrEmpty() && Rand.Value < xenotypeHolder.xenotypeDef.doubleXenotypeChances.Sum((XenotypeChance x) => x.chance) && xenotypeHolder.xenotypeDef.doubleXenotypeChances.TryRandomElementByWeight((XenotypeChance x) => x.chance, out var result))
			//{
			//	Reimplant(result.xenotype, false);
			//}
			ReimplanterUtility.SetXenotype(pawn, xenotypeHolder, xenogenes, this);
			//if (xenogermComaAfterShapeshift)
			//{
			//	pawn.health.AddHediff(HediffDefOf.XenogerminationComa);
			//}
			if (genesRegrowAfterShapeshift)
			{
				if (!xenotypeHolder.isTrueShiftForm)
				{
					TryOffsetResource((int)((xenotypeHolder.genes.Sum((gene) => gene.biostatCpx) * 0.05f) + (xenotypeHolder.genes.Sum((gene) => gene.biostatArc) * 0.1f)));
				}
				//GeneUtility.UpdateXenogermReplication(pawn);
				AddXenogermReplicating(xenotypeHolder.genes);
                //AddGenMat(days);
            }
            if (ModLister.IdeologyInstalled)
			{
				Find.HistoryEventsManager.RecordEvent(new HistoryEvent(WVC_HistoryEventDefOf.WVC_Shapeshift, pawn.Named(HistoryEventArgsNames.Doer)));
			}
			DoEffects();
		}

        public void AddXenogermReplicating(List<GeneDef> genes)
        {
            XaG_GeneUtility.GetBiostatsFromList(genes, out int cpx, out int met, out int _);
            int architeCount = genes.Where((geneDef) => geneDef.biostatArc != 0).ToList().Count;
            int nonArchiteCount = genes.Count - architeCount;
            int days = Mathf.Clamp(nonArchiteCount + (architeCount * 2) - met + (int)(cpx * 0.1f), 0, 999);
            int count = days * 60000;
            ReimplanterUtility.XenogermReplicating_WithCustomDuration(pawn, new((int)(count * 0.8f), (int)(count * 1.1f)), pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.XenogermReplicating));
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

		public void UpdateMetabolism()
        {
			HediffUtility.TryAddOrUpdMetabolism(Giver.metHediffDef, pawn, this);
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
				//GeneResourceUtility.Notify_PostShapeshift_Traits(shapeshiftGene);
			}
			UpdateMetabolism();
			//ReimplanterUtility.PostImplantDebug(pawn);
		}

    }

}
