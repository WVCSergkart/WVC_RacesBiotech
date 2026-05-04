using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class Gene_Fleshshaper : Gene_Shapeshifter, IGeneXenogenesEditor, IGeneDevourer
	{

		private static List<Gene_Fleshshaper> cachedFleshshaperGenes;
		public static List<Gene_Fleshshaper> FleshshaperGenes
		{
			get
			{
				if (cachedFleshshaperGenes == null)
				{
					List<Gene_Fleshshaper> list = new();
					foreach (Pawn pawn in PawnsFinder.AllMapsAndWorld_Alive)
					{
						//if (pawn?.genes == null)
						//{
						//	continue;
						//}
						//foreach (Gene gene in pawn.genes.GenesListForReading)
						//{
						//	if (gene is Gene_Fleshshaper item && gene.Active)
						//	{
						//		list.Add(item);
						//		break;
						//	}
						//}
						list.AddSafe(pawn?.genes?.GetFirstGeneOfType<Gene_Fleshshaper>());
					}
					cachedFleshshaperGenes = list;
				}
				return cachedFleshshaperGenes;
			}
		}

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

		//public override List<GeneralHolder> ShaperGenes
		//{
		//	get
		//	{
		//		List<GeneralHolder> holders = new();
		//		foreach (ShaperGeneSetDef fleshshaperGeneSetDef in DefDatabase<ShaperGeneSetDef>.AllDefsListForReading)
		//		{
		//			if (fleshshaperGeneSetDef.Allowed(pawn, this))
		//			{
		//				holders.AddRange(fleshshaperGeneSetDef.geneSets);
		//			}
		//		}
		//		if (!holders.Empty())
		//		{
		//			return holders;
		//		}
		//		return base.ShaperGenes;
		//	}
		//}

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

		public List<GeneDef> AllGenes
		{
			get
			{
				List<GeneDef> geneDefs = new();
				geneDefs.AddRangeSafe(CollectedGenes);
				geneDefs.AddRangeSafe(GenelineGenes);
				return geneDefs;
			}
		}

		private List<GeneDef> collectedGeneDefs;
		public List<GeneDef> CollectedGenes
		{
			get
			{
				if (collectedGeneDefs == null)
				{
					collectedGeneDefs = new();
				}
				return collectedGeneDefs;
			}
		}

		public List<GeneDef> GenelineGenes => XenotypesGenes;

		public override void UpdateCache()
		{
			base.UpdateCache();
			//collectedGeneDefs = null;
			cachedFleshshaperGenes = null;
		}

		public List<GeneDef> DisabledGenes => [];
		public List<GeneDef> DestroyedGenes => [];

		//public Pawn Pawn => pawn;
		public GeneDef Def => def;

		public int ArchiteLimit => pawn.genes.Endogenes.Sum(gene => gene.def.biostatArc);
		public int ComplexityLimit => pawn.genes.Endogenes.Sum(gene => gene.def.biostatCpx);

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

		public IntRange ReqMetRange => new(-5, 999);
		public bool ReqCooldown => true;
		public bool DisableSubActions => true;
		public bool UseGeneline => true;

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
			AddGene_Safe(geneDef, false);
		}

		public override void AddGene_Genetic(GeneDef geneDef, bool inheritable)
		{
			CollectedGenes.AddSafe(geneDef);
			AddGene_Safe(geneDef, false);
		}

		public override void CopyFrom(Gene_Shapeshifter oldShapeshifter)
		{
			if (oldShapeshifter is Gene_Fleshshaper gene_Fleshshaper)
			{
				this.unlockedXenotypes = gene_Fleshshaper.unlockedXenotypes;
				this.collectedGeneDefs = gene_Fleshshaper.CollectedGenes;
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
				if (TryRandomGene(pawn, out Gene gene) && TryOffsetResource(gene, 2f))
				{
					pawn.genes.RemoveGene(gene);
					Message(pawn, gene);
					// Small and medium colony only
					if (StaticCollectionsClass.cachedPlayerPawnsCount < 10)
					{
						ReimplanterUtility.PostImplantDebug(pawn);
					}
				}
				else
				{
					TryOffsetResource(4);
					ReimplanterUtility.ReduceXenogermReplicationTick(pawn, 5);
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
					if (pawnGene == this || CollectedGenes.Contains(pawnGene.def))
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
			Scribe_Collections.Look(ref collectedGeneDefs, "collectedGeneDefs", LookMode.Def);
			if (Scribe.mode == LoadSaveMode.LoadingVars && collectedGeneDefs != null && collectedGeneDefs.RemoveAll((GeneDef x) => x == null) > 0)
			{
				Log.Warning("Removed null geneDef(s)");
			}
		}

		public void Notify_DevouredHuman(Pawn victim)
		{
			UnlockXenotype(victim.genes.XenotypeLabel.UncapitalizeFirst());
		}

		public void Notify_DevouredFlesh(Pawn victim)
		{

		}

		public void Notify_DevouredMech(Pawn victim)
		{

		}

	}
}
