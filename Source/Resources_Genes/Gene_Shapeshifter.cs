using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Shapeshifter : XaG_Gene, IGeneOverriddenBy, IGenePregnantHuman, IGeneWithEffects, IGeneMetabolism, IGeneRecacheable, IGeneShapeshifter
	{

		private GeneExtension_Undead cachedGeneExtension_Undead;
		public GeneExtension_Undead Props
		{
			get
			{
				if (cachedGeneExtension_Undead == null)
				{
					cachedGeneExtension_Undead = def.GetModExtension<GeneExtension_Undead>();
				}
				return cachedGeneExtension_Undead;
			}
		}

		private GeneExtension_Giver cachedGeneExtension_Giver;
		public GeneExtension_Giver Giver
		{
			get
			{
				if (cachedGeneExtension_Giver == null)
				{
					cachedGeneExtension_Giver = def.GetModExtension<GeneExtension_Giver>();
				}
				return cachedGeneExtension_Giver;
			}
		}

		// Scenario hook
		public static List<XenotypeHolder> xenotypesOverride;
		public List<XenotypeHolder> Xenotypes
		{
			get
			{
				if (xenotypesOverride != null)
				{
					return xenotypesOverride;
				}
				//List<XenotypeHolder> holders = new();
				//int limit = 3;
				//List<XenotypeHolder> xenotypeHolders = ListsUtility.GetAllXenotypesHolders();
				////xenotypeHolders.SortBy(xenos => xenos.genes.Any(geneDef => geneDef.IsGeneDefOfType<Gene_Shapeshift_TrueForm>()), xenos => xenos.genes);
				//xenotypeHolders.Shuffle();
				//foreach (XenotypeHolder xenotypeHolder in xenotypeHolders)
				//{
				//	if (holders.Count >= limit)
				//	{
				//		break;
				//	}
				//	holders.Add(xenotypeHolder);
				//}
				//if (!holders.Any(holder => holder.xenotypeDef == pawn.genes.Xenotype))
				//{
				//	holders.Add(new(pawn.genes.Xenotype));
				//}
				//return holders;
				return ListsUtility.GetAllXenotypesHolders();
			}
		}

		private bool? cachedGenesRegrow;
		[Obsolete]
		public bool EnableGenesRegrowing
		{
			get
			{
				if (!cachedGenesRegrow.HasValue)
				{
					UpdGenesRegrow();
				}
				return cachedGenesRegrow.Value;
			}
		}

		[Obsolete]
		private void UpdGenesRegrow()
		{
			cachedGenesRegrow = null;
			foreach (Gene item in pawn.genes.GenesListForReading)
			{
				if (item is Gene_ShapeshifterDependant dependant && dependant.DisableGenesRegrowing)
				{
					cachedGenesRegrow = false;
					break;
				}
			}
			if (!cachedGenesRegrow.HasValue)
			{
				cachedGenesRegrow = true;
			}
		}

		public override void PostAdd()
		{
			base.PostAdd();
			UpdateMetabolism();
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
			cachedGenesRegrow = null;
		}

		public void RemoveHediffs()
		{
			HediffUtility.RemoveHediffsFromList(pawn, Giver?.hediffDefs);
		}

		public void Notify_Override()
		{
			UpdateMetabolism();
			cachedGenesRegrow = null;
		}

		public void Notify_PregnancyStarted(Hediff_Pregnant pregnancy)
		{
			GeneSet geneSet = pregnancy.geneSet;
			geneSet.AddGene(def);
			geneSet.SortGenes();
		}

		public bool Notify_CustomPregnancy(Hediff_Pregnant pregnancy)
		{
			return false;
		}

		public override void TickInterval(int delta)
		{

		}

		public override void PostRemove()
		{
			base.PostRemove();
			RemoveHediffs();
		}

		//public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
		//{
		//	yield return new StatDrawEntry(StatCategoryDefOf.Genetics, "WVC_XaG_GeneShapeshifter_GenesRegrowAfterShapeshift_Label".Translate(), EnableGenesRegrowing.ToStringYesNo(), "WVC_XaG_GeneShapeshifter_GenesRegrowAfterShapeshift_Desc".Translate(), 220);
		//}

		public virtual void CopyFrom(Gene_Shapeshifter oldShapeshifter)
		{
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
				gene_Shapeshifter.CopyFrom(this);
				gene_Shapeshifter.AddXenogermReplicating(new() { def });
				gene_Shapeshifter.DoEffects();
				Messages.Message("WVC_XaG_DialogEditShiftGenes_ChangeTypeMessage".Translate(), pawn, MessageTypeDefOf.NeutralEvent, historical: false);
			}
		}

		private float geneticMaterial = 0;

		public int GeneticMaterial => (int)geneticMaterial;

		public bool TryOffsetResource(float count)
		{
			if (!EnableGenesRegrowing && count > 0f)
			{
				return false;
			}
			geneticMaterial += count;
			if (geneticMaterial < 0f)
			{
				geneticMaterial = 0f;
			}
			return true;
		}

		public bool TryOffsetResource(Gene gene)
		{
			return TryOffsetResource((gene.def.biostatCpx * 0.5f) + (gene.def.biostatArc * 0.2f));
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

		public static void OffsetResource(Pawn pawn, float resourceOffset)
		{
			pawn.genes?.GetFirstGeneOfType<Gene_Shapeshifter>()?.TryOffsetResource(resourceOffset);
		}

		public void TryForceGene(GeneDef geneDef, bool inheritable)
		{
			if (!geneDef.ConflictsWith(this.def) && XaG_GeneUtility.TryRemoveAllConflicts(pawn, geneDef))
			{
				pawn.genes.AddGene(geneDef, !inheritable);
				AddXenogermReplicating(new() { geneDef });
			}
		}

		// Reimplanter

		private List<GeneDef> cachedPreservedGenes;
		public List<GeneDef> PreservedGeneDefs
		{
			get
			{
				if (cachedPreservedGenes == null)
				{
					List<GeneDef> newList = new();
					foreach (Gene item in pawn.genes.GenesListForReading)
					{
						if (item is Gene_ShapeshifterDependant dependant && dependant.PreservedGeneDefs != null && dependant.Active)
						{
							foreach (GeneDef geneDef in dependant.PreservedGeneDefs)
							{
								if (!newList.Contains(geneDef))
								{
									newList.Add(geneDef);
								}
							}
						}
					}
					cachedPreservedGenes = newList;
				}
				return cachedPreservedGenes;
			}
		}

		public virtual void AddGene(GeneDef geneDef, bool inheritable)
		{
			if (!geneDef.ConflictsWith(this.def))
			{
				pawn.genes.AddGene(geneDef, !inheritable);
			}
		}

		public virtual void RemoveGene(Gene gene)
		{
			if (gene != this && !PreservedGeneDefs.Contains(gene.def))
			{
				pawn.genes.RemoveGene(gene);
			}
		}

		public virtual void Shapeshift(XenotypeHolder xenotypeHolder, bool removeXenogenes = true, bool hybridizeXenotypes = false)
		{
			if (hybridizeXenotypes)
			{
				foreach (GeneDef geneDef in xenotypeHolder.genes)
				{
					if (XaG_GeneUtility.TryRemoveAllConflicts(pawn, geneDef, new() { def }))
					{
						pawn.TryAddOrRemoveGene(this, null, geneDef, xenotypeHolder.inheritable);
					}
				}
				ReimplanterUtility.UnknownXenotype(pawn, pawn.genes.XenotypeLabel.TrimmedToLength(3) + xenotypeHolder.Label.ToString().Reverse().TrimmedToLength(4).Reverse());
			}
			else
			{
				ReimplanterUtility.SetXenotype(pawn, xenotypeHolder, this, removeXenogenes);
			}
			if (EnableGenesRegrowing)
			{
				if (!xenotypeHolder.isTrueShiftForm)
				{
					TryOffsetResource((int)((xenotypeHolder.genes.Sum((gene) => gene.biostatCpx) * 0.05f) + (xenotypeHolder.genes.Sum((gene) => gene.biostatArc) * 0.1f)));
				}
				AddXenogermReplicating(xenotypeHolder.genes, (hybridizeXenotypes ? 2f : 1f) * WVC_Biotech.settings.shapeshifer_CooldownDurationFactor);
			}
			if (ModLister.IdeologyInstalled)
			{
				Find.HistoryEventsManager.RecordEvent(new HistoryEvent(HistoryEventDefOf.WVC_Shapeshift, pawn.Named(HistoryEventArgsNames.Doer)));
			}
			DoEffects();
		}

		public void AddXenogermReplicating(List<GeneDef> genes, float durationFactor = 1f)
		{
			XaG_GeneUtility.GetBiostatsFromList(genes, out int cpx, out int met, out int _);
			int architeCount = genes.Where((geneDef) => geneDef.biostatArc != 0).ToList().Count;
			int nonArchiteCount = genes.Count - architeCount;
			int days = Mathf.Clamp(nonArchiteCount + (architeCount * 2) - met + (int)(cpx * 0.1f), 0, 999);
			int count = (days + (StaticCollectionsClass.cachedNonDeathrestingColonistsCount * 3)) * 60000;
			// get modded cd percent
			float vanillaGenesCD = 140;
			float moddedGenesCD = HediffDefOf.XenogermReplicating.CompProps<HediffCompProperties_Disappears>().disappearsAfterTicks.TrueMax / 60000;
			//Log.Error("Modded CD Percent: " + moddedGenesCD);
			float finalPercent = moddedGenesCD / vanillaGenesCD;
			//Log.Error("CD Percent: " + finalPercent);
			// get modded cd percent
			ReimplanterUtility.XenogermReplicating_WithCustomDuration(pawn, new((int)(count * 0.8f * finalPercent * durationFactor), (int)(count * 1.1f * finalPercent * durationFactor)));
		}

		public virtual void DoEffects()
		{
			if (pawn.Map == null)
			{
				return;
			}
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
			cachedPreservedGenes = null;
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
			}
			UpdateMetabolism();
			cachedPreservedGenes = null;
		}

		public void Notify_GenesRecache(Gene changedGene)
		{
			cachedGenesRegrow = null;
			cachedPreservedGenes = null;
		}

		public bool gizmoCollapse = WVC_Biotech.settings.geneGizmosDefaultCollapse;
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref geneticMaterial, "geneticMaterial", 0);
			Scribe_Values.Look(ref gizmoCollapse, "gizmoCollapse", WVC_Biotech.settings.geneGizmosDefaultCollapse);
		}

	}

}
