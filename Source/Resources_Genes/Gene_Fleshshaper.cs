using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class Gene_Fleshshaper : Gene_Shapeshifter, IGeneXenogenesEditor, IGeneDevourer
	{

		//private GeneExtension_Undead cachedGeneExtension_Undead;
		//public GeneExtension_Undead Extension_Undead
		//{
		//	get
		//	{
		//		if (cachedGeneExtension_Undead == null)
		//		{
		//			cachedGeneExtension_Undead = def.GetModExtension<GeneExtension_Undead>();
		//		}
		//		return cachedGeneExtension_Undead;
		//	}
		//}

		public GeneExtension_Undead Extension_Undead => Props;

		public override void PostAdd()
		{
			base.PostAdd();
			UnlockXenotype(pawn.genes.XenotypeLabel);
		}

		private List<string> unlockedXenotypes;
		public override List<XenotypeHolder> Xenotypes
		{
			get
			{
				return base.Xenotypes.Where(holder =>
				{
					bool isSpecifiedXenotype = Giver.xenotypeDefs != null && Giver.xenotypeDefs.Contains(holder.XenotypeDef_Safe.defName);
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
					cachedGeneDefs = XenotypesGenes;
				}
				return cachedGeneDefs;
			}
		}

		//public bool IsContainer => false;

		public void UpdateCache()
		{
			cachedGeneDefs = null;
		}

		public List<GeneDef> DisabledGenes => [];
		public List<GeneDef> DestroyedGenes => [];

		public Pawn Pawn => pawn;
		public GeneDef Def => def;

		//public StatDef ChimeraLimitStatDef => Giver.statDef;

		public int ArchiteLimit => pawn.genes.Endogenes.Sum(gene => gene.def.biostatArc);
		public int ComplexityLimit => pawn.genes.Endogenes.Sum(gene => gene.def.biostatCpx);

		// Disabled for this gene
		//public List<GeneSetPresets> geneSetPresets = new();
		public List<GeneSetPreset> GeneSetPresets
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
			unlockedXenotypes.AddSafe(xenotypeName.UncapitalizeFirst());
		}

		public void AddGene_Editor(GeneDef geneDef)
		{
			//if (!this.def.ConflictsWith(geneDef))
			//{
			//	pawn.genes.AddGene(geneDef, true);
			//}
			AddGene_Safe(geneDef, false);
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
				//pawn.genes.Xenogenes.TryRandomElement(out Gene gene)
				if (TryRandomGene(pawn, out Gene gene) && TryOffsetResource(gene, 2f))
				{
					//RemoveGene_Safe(gene);
					pawn.genes.RemoveGene(gene);
					Message(pawn, gene);
				}
				else
				{
					TryOffsetResource(4);
				}
			}
			catch (Exception arg)
			{
				Log.Error("Failed remove random gene. Reason: " + arg.Message);
			}

			static void Message(Pawn pawn, Gene gene)
			{
				if (PawnUtility.ShouldSendNotificationAbout(pawn))
				{
					Messages.Message("WVC_XaG_FaultyShapeDisease".Translate(pawn, gene.Label), pawn, MessageTypeDefOf.NegativeEvent);
				}
			}

			bool TryRandomGene(Pawn pawn, out Gene gene)
			{
				gene = null;
				List<Gene> xenogenes = pawn.genes.Xenogenes;
				if (xenogenes.Empty())
				{
					return false;
				}
				List<Gene> genes = new();
				foreach (Gene pawnGene in xenogenes)
				{
					if (pawnGene == this || PreservedGeneDefs.Contains(pawnGene.def))
					{
						continue;
					}
					if (xenogenes.Any(g => g.def.prerequisite == pawnGene.def))
					{
						continue;
					}
					genes.Add(pawnGene);
				}
				return genes.TryRandomElement(out gene);
			}
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

		public void Notify_DevouredHuman(Pawn victim)
		{
			UnlockXenotype(victim.genes.XenotypeLabel.UncapitalizeFirst());
		}

		public void Notify_DevouredFlesh(Pawn victim)
		{

		}

	}

}
