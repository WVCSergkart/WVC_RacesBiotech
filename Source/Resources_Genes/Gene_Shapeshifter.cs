using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Verse;
using Verse.Sound;
using static WVC_XenotypesAndGenes.Dialog_ShaperEditor;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Shapeshifter : XaG_Gene, IGeneOverriddenBy, IGenePregnantHuman, IGeneWithEffects, IGeneMetabolism, IGeneRecacheable, IGeneShapeshifter, IGeneShaperEditor
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

		public virtual List<GeneralHolder> ShaperGenes
		{
			get
			{
				List<GeneralHolder> holders = new();
				foreach (ShaperGeneSetDef geneSetDef in DefDatabase<ShaperGeneSetDef>.AllDefsListForReading)
				{
					if (geneSetDef.Allowed(pawn, this))
					{
						foreach (GeneralHolder geneSet in geneSetDef.geneSets)
						{
							if (holders.Any(holder => holder.ConflictWith(geneSet)))
							{
								continue;
							}
							holders.Add(geneSet);
						}
						//holders.AddRange(geneSetDef.geneSets);
					}
				}
				return holders;
				//return Giver.geneDefWithChances;
			}
		}

		public virtual List<XenotypeHolder> Xenotypes => ListsUtility.GetAllXenotypesHolders();
		public virtual List<GeneDef> XenotypesGenes
		{
			get
			{
				List<GeneDef> geneDefs = new();
				foreach (XenotypeHolder xenotypeHolder in Xenotypes)
				{
					geneDefs.AddRangeSafe(xenotypeHolder.genes);
				}
				return geneDefs;
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

		public virtual void Notify_OverriddenBy(Gene overriddenBy)
		{
			RemoveHediffs();
			cachedGenesRegrow = null;
		}

		public void RemoveHediffs()
		{
			//HediffUtility.RemoveHediffsFromList(pawn, Giver?.hediffDefs);
			HediffUtility.TryRemoveHediff(HediffUtility.MetHediffDef, pawn);
		}

		public virtual void Notify_Override()
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

		protected float geneticMaterial = 0;

		public int ShaperResource => (int)geneticMaterial;
		public float ShaperResource_Raw => geneticMaterial;

		public virtual float GeneticMatchOffset
		{
			get
			{
				SimpleCurve geneticCurve = new()
				{
					new CurvePoint(0, 0),
					new CurvePoint(20, 10),
					new CurvePoint(50, 25),
					new CurvePoint(100, 30)
				};
				float geneticMaterial = geneticCurve.Evaluate(ShaperResource) * 0.01f;
				return geneticMaterial;
			}
		}

		public virtual bool TryOffsetResource(float count)
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

		public bool TryOffsetResource(Gene gene, float factor = 1f)
		{
			return TryOffsetResource(((gene.def.biostatCpx * 0.5f) + (gene.def.biostatArc * 1.0f)) * factor);
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

		public virtual void AddGene_Genetic(GeneDef geneDef, bool inheritable)
		{
			if (!geneDef.ConflictsWith(this.def) && XaG_GeneUtility.TryRemoveAllConflicts(pawn, geneDef))
			{
				pawn.genes.AddGene(geneDef, !inheritable);
				AddXenogermReplicating(new() { geneDef });
			}
		}

		// Reimplanter
		private List<GeneDef> cachedPreservedGenes;
		public virtual List<GeneDef> PreservedGeneDefs
		{
			get
			{
				if (cachedPreservedGenes == null)
				{
					List<GeneDef> newList = new();
					foreach (Gene item in pawn.genes.GenesListForReading)
					{
						// Legacy
						if (item is Gene_ShapeshifterDependant dependant && dependant.PreservedGeneDefs != null && dependant.Active)
						{
							newList.AddRangeSafe(dependant.PreservedGeneDefs);
						}
						// Legacy
					}
					cachedPreservedGenes = newList;
				}
				return cachedPreservedGenes;
			}
		}

		public Pawn Pawn => pawn;
		public Gene Gene => this;

		public Dialog_ShaperEditor.GeneMatStatData[] GeneMatStats
		{
			get
			{
				GeneMatStatData[] geneMatStats = new GeneMatStatData[2]
				{
					new("WVC_XaG_GeneticMaterial_Shifter", "WVC_XaG_GeneticMaterial_ShifterDesc", new CachedTexture("WVC/UI/XaG_General/GenMatTex_Req")),
					new("WVC_XaG_GeneticMaterial_Genes", "WVC_XaG_GeneticMaterial_GenesDesc", new CachedTexture("WVC/UI/XaG_General/GenMatTex_Has")),
					//new(Props.shaper_stat1_labelKey, Props.shaper_stat1_descKey, new(Props.shaper_stat1_texturePath)),
					//new(Props.shaper_stat2_labelKey, Props.shaper_stat2_descKey, new(Props.shaper_stat2_texturePath)),
				};
				return geneMatStats;
			}
		}

		//public virtual CachedTexture ReqTex_Shaper => XaG_UiUtility.ShapeshifterGenMatReqTex;
		//public virtual CachedTexture HasTex_Shaper => XaG_UiUtility.ShapeshifterGenMatHasTex;

		public void Action_Shaper(List<GeneDefWithChance> list, bool inheritable)
		{
			foreach (GeneDefWithChance geneDefWithChance in list)
			{
				if (geneDefWithChance.disabled)
				{
					continue;
				}
				if (TryConsumeResource(geneDefWithChance.Cost))
				{
					AddGene_Genetic(geneDefWithChance.geneDef, inheritable);
				}
			}
			ReimplanterUtility.PostImplantDebug(pawn);
			UpdateMetabolism();
			DoEffects();
		}

		public virtual void AddGene_Safe(GeneDef geneDef, bool inheritable)
		{
			if (!geneDef.ConflictsWith(this.def))
			{
				pawn.genes.AddGene(geneDef, !inheritable);
			}
		}

		public virtual void RemoveGene_Safe(Gene gene)
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
			HediffUtility.TryAddOrUpdMetabolism(pawn, this);
		}

		// Shapeshift

		public virtual void PreShapeshift(bool genesRegrowing)
		{
			cachedPreservedGenes = null;
			if (!genesRegrowing)
			{
				GeneResourceUtility.Notify_PreShapeshift(this);
			}
		}

		public virtual void PostShapeshift(bool genesRegrowing)
		{
			if (!genesRegrowing)
			{
				GeneResourceUtility.Notify_PostShapeshift(this);
			}
			//UpdateMetabolism();
			cachedPreservedGenes = null;
		}

		public virtual void Notify_GenesRecache(Gene changedGene)
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
