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

		public GeneExtension_Undead Extension_Undead => Undead;

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
			cachedOverloadLevel = null;
		}

		public List<GeneDef> DisabledGenes => [];
		public List<GeneDef> DestroyedGenes => [];

		public int ArchiteLimit => pawn.genes.Endogenes.Sum(gene => gene.def.biostatArc);
		public int ComplexityLimit => pawn.genes.Endogenes.Sum(gene => gene.def.biostatCpx);

		private static List<GeneSetPreset> geneSetPresets;
		public List<GeneSetPreset> GeneSetPresets
		{
			get
			{
				if (geneSetPresets == null)
				{
					geneSetPresets = new();
				}
				return geneSetPresets;
			}
			set
			{
				geneSetPresets = value;
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

		public void AddGene_Editor(GeneDef geneDef)
		{
			AddGene_Safe(geneDef, false);
		}

		public override void AddGene_Genetic(GeneDef geneDef, bool inheritable)
		{
			CollectedGenes.AddSafe(geneDef);
			AddGene_Safe(geneDef, false);
		}

		public override void TickInterval(int delta)
		{
			base.TickInterval(delta);
			if (pawn.IsHashIntervalTick(59991, delta))
			{
				XenogenesRemover();
			}
		}

		private int? cachedOverloadLevel;
		public int OverloadLevel
		{
			get
			{
				if (cachedOverloadLevel == null)
				{
					cachedOverloadLevel = pawn.genes.Endogenes.Count / 21;
				}
				return cachedOverloadLevel.Value;
			}
		}

		private static int nextMessageTick = -1;
		private void XenogenesRemover()
		{
			try
			{
				List<GeneDef> removedGeneDefs = new();
				for (int i = 0; i < OverloadLevel; i++)
				{
					if (TryRandomGene(pawn, out Gene gene) && TryOffsetResource(gene, 2f))
					{
						pawn.genes.RemoveGene(gene);
						removedGeneDefs.Add(gene.def);
					}
				}
				if (removedGeneDefs.NullOrEmpty())
				{
					TryOffsetResource(4);
					ReimplanterUtility.ReduceXenogermReplicationDays(pawn, 5);
				}
				else
				{
					Message(removedGeneDefs);
					if (StaticCollectionsClass.cachedPlayerPawnsCount < 10)
					{
						ReimplanterUtility.PostImplantDebug(pawn);
					}
				}
			}
			catch (Exception arg)
			{
				Log.Error("Failed remove random gene. Reason: " + arg.Message);
			}

			void Message(List<GeneDef> removedGeneDefs)
			{
				if (PawnUtility.ShouldSendNotificationAbout(pawn) && nextMessageTick < Find.TickManager.TicksGame)
				{
					if (removedGeneDefs.Count > 1)
					{
						Messages.Message("WVC_XaG_FaultyShapeDisease_FewGenes".Translate(pawn, removedGeneDefs.Select(def => def.label).ToLineList()), pawn, MessageTypeDefOf.NegativeEvent);
					}
					else
					{
						Messages.Message("WVC_XaG_FaultyShapeDisease".Translate(pawn, removedGeneDefs.FirstOrDefault().label), pawn, MessageTypeDefOf.NegativeEvent);
					}
					nextMessageTick = Find.TickManager.TicksGame + 120;
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

		public override bool TryOffsetResource(float count)
		{
			if (geneticMaterial >= 999)
			{
				return false;
			}
			return base.TryOffsetResource(count);
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

		public void Notify_DevouredEntity(Pawn victim)
		{

		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Collections.Look(ref geneSetPresets, "geneSetPresets", LookMode.Deep);
		}
	}
}
