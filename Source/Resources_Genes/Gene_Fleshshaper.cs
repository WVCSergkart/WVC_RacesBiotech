using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class Gene_Fleshshaper : Gene_Shapeshifter, IGeneXenogenesEditor
	{

		private GeneExtension_Undead cachedGeneExtension_Undead;
		public GeneExtension_Undead Extension_Undead
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

		private List<string> unlockedXenotypes;
		public override List<XenotypeHolder> Xenotypes
		{
			get
			{
				return base.Xenotypes.Where(holder =>
				{
					bool isSpecifiedXenotype = Giver.xenotypeDefs != null && Giver.xenotypeDefs.Contains(holder.xenotypeDef);
					bool inAnyCategory = Giver.geneCategoryDefs != null && holder.genes.Any(def => Giver.geneCategoryDefs.Contains(def.displayCategory));
					bool isUnlocked = unlockedXenotypes != null && unlockedXenotypes.Contains(holder.Label);
					return holder.Baseliner || inAnyCategory || isUnlocked || isSpecifiedXenotype;
				}).ToList();
			}
		}

		public List<GeneDef> AllGenes => CollectedGenes;

		private List<GeneDef> cachedGeneDefs;
		public List<GeneDef> CollectedGenes
		{
			get
			{
				if (cachedGeneDefs == null)
				{
					List<GeneDef> geneDefs = new();
					foreach (XenotypeHolder xenotypeHolder in Xenotypes)
					{
						geneDefs.AddRangeSafe(xenotypeHolder.genes);
					}
					cachedGeneDefs = geneDefs;
				}
				return cachedGeneDefs;
			}
		}

		public void UpdateCache()
		{
			cachedGeneDefs = null;
		}

		public List<GeneDef> DisabledGenes => [];
		public List<GeneDef> DestroyedGenes => [];

		public Pawn Pawn => pawn;
		public GeneDef Def => def;

		//public StatDef ChimeraLimitStatDef => Giver.statDef;

		public int ArchiteLimit
		{
			get
			{
				return pawn.genes.GenesListForReading.Sum(gene => gene.def.biostatArc);
			}
		}

		public int ComplexityLimit
		{
			get
			{
				//return (int)pawn.GetStatValue(ChimeraLimitStatDef);
				return pawn.genes.GenesListForReading.Sum(gene => gene.def.biostatCpx);
			}
		}

		// Disabled for this gene
		//public List<GeneSetPresets> geneSetPresets = new();
		public List<GeneSetPresets> GeneSetPresets
		{
			get
			{
				return null;
			}
			set
			{

			}
		}

		public IntRange ReqMetRange => new(-15, 15);
		public bool ReqCooldown => false;
		public bool DisableSubActions => true;
		public bool UseGeneline => false;

		public void Debug_RemoveDupes()
		{
			// Dev
		}

		public void UnlockXenotype(string xenotypeName)
		{
			if (unlockedXenotypes == null)
			{
				unlockedXenotypes = new();
			}
			unlockedXenotypes.AddSafe(xenotypeName);
		}

		public void ImplantGene(GeneDef geneDef)
		{
			//if (!this.def.ConflictsWith(geneDef))
			//{
			//	pawn.genes.AddGene(geneDef, true);
			//}
			AddGene(geneDef, false);
		}

		public override void CopyFrom(Gene_Shapeshifter oldShapeshifter)
		{
			if (oldShapeshifter is Gene_Fleshshaper gene_Fleshshaper)
			{
				this.unlockedXenotypes = gene_Fleshshaper.unlockedXenotypes;
			}
			base.CopyFrom(oldShapeshifter);
		}

		public override void TickInterval(int delta)
		{
			base.TickInterval(delta);
			if (pawn.IsHashIntervalTick(59991, delta))
			{
				XenogenesRemover();
			}
		}

		private void XenogenesRemover()
		{
			try
			{
				//TryOffsetResource(5);
				//if (pawn.genes.Endogenes.Count > 35)
				//{
				//	Gene gene = pawn.genes.Endogenes.RandomElement();
				//	pawn.genes.RemoveGene(gene);
				//	Message(pawn, gene);
				//}
				if (pawn.genes.Xenogenes.TryRandomElement(out Gene gene) && TryOffsetResource(5))
				{
					//Gene gene = pawn.genes.Xenogenes.TryRandomElement();
					pawn.genes.RemoveGene(gene);
					Message(pawn, gene);
				}
			}
			catch (Exception arg)
			{
				Log.Error("Failed remove shapeshifter random gene. Reason: " + arg.Message);
			}

			static void Message(Pawn pawn, Gene gene)
			{
				if (PawnUtility.ShouldSendNotificationAbout(pawn))
				{
					Messages.Message("WVC_XaG_FaultyShapeDisease".Translate(pawn, gene.Label), pawn, MessageTypeDefOf.NegativeEvent);
				}
			}
		}

		public void RemoveCollectedGene_Storage(GeneDef geneDef)
		{

		}

		public bool TryDisableGene(GeneDef geneDef)
		{
			return false;
		}

		public bool TryGetToolGene()
		{
			return false;
		}

		public bool TryGetUniqueGene()
		{
			return false;
		}

		public void UpdSubHediffs()
		{

		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Collections.Look(ref unlockedXenotypes, "unlockedXenotypeDefs", LookMode.Value);
			//if (Scribe.mode == LoadSaveMode.PostLoadInit)
			//{
			//	if (geneSetPresets == null)
			//	{
			//		geneSetPresets = new();
			//	}
			//}
		}

	}

}
