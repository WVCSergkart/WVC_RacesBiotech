using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_StorageImplanter : XaG_Gene, IGeneXenogenesEditor, IGeneXenogenesContainer
	{

		//public static bool CanStoreGenes(Pawn pawn, out Gene_StorageImplanter implanter)
		//{
		//	implanter = pawn.genes?.GetFirstGeneOfType<Gene_StorageImplanter>();
		//	if (implanter != null)
		//	{
		//		Messages.Message("WVC_XaG_StorageImplanter_Message".Translate(), null, MessageTypeDefOf.PositiveEvent, historical: false);
		//		return true;
		//	}
		//	Messages.Message("WVC_XaG_StorageImplanter_ErrorMessage".Translate(), null, MessageTypeDefOf.RejectInput, historical: false);
		//	return false;
		//}

		public void SetupHolder(XenotypeDef xenotypeDef = null, List<GeneDef> genes = null, bool inheritable = false, XenotypeIconDef icon = null, string name = null)
		{
			if (xenotypeDef == XenotypeDefOf.Baseliner)
			{
				if (name.NullOrEmpty())
				{
					name = GeneUtility.GenerateXenotypeNameFromGenes(genes);
				}
				if (icon == null)
				{
					icon = DefDatabase<XenotypeIconDef>.AllDefsListForReading.RandomElement();
				}
			}
			this.xenotypeHolder = new XenotypeHolder_Exposable(xenotypeDef, genes, inheritable, icon, name);
			this.xenotypeHolder.PostSetup();
		}

		public void SetupHolder(XenotypeHolder xenotypeHolder)
		{
			this.xenotypeHolder = new XenotypeHolder_Exposable(xenotypeHolder);
			this.xenotypeHolder.PostSetup();
		}

		//public bool TryAddGene(GeneDef geneDef)
		//{
		//	if (xenotypeHolder == null)
		//	{
		//		return false;
		//	}
		//	if (!xenotypeHolder.genes.Contains(geneDef))
		//	{
		//		xenotypeHolder.genes.Add(geneDef);
		//		return true;
		//	}
		//	return false;
		//}

		//public bool TryRemoveGene(GeneDef geneDef)
		//{
		//	if (xenotypeHolder == null)
		//	{
		//		return false;
		//	}
		//	if (xenotypeHolder.genes.Contains(geneDef))
		//	{
		//		xenotypeHolder.genes.Remove(geneDef);
		//		return true;
		//	}
		//	return false;
		//}

		public override void PostAdd()
		{
			base.PostAdd();
			WVC_XenotypesAndGenes.GeneSetPreset.SetupGeneSetPresets(this);
		}

		public void DoAction()
		{
			UpdateCache();
			UpdSubHediffs();
			Find.WindowStack.Add(new Dialog_XenogenesEditor(this));
		}

		private XenotypeHolder_Exposable xenotypeHolder = null;
		public XenotypeHolder XenotypeHolder => xenotypeHolder;

		public List<GeneDef> AllGenes => CollectedGenes;

		private List<GeneDef> cachedGeneDefs;
		public List<GeneDef> CollectedGenes
		{
			get
			{
				if (cachedGeneDefs == null)
				{
					List<GeneDef> geneDefs = new();
					geneDefs.AddRangeSafe(pawn.genes?.GetFirstGeneOfType<Gene_Chimera>()?.CollectedGenes);
					geneDefs.AddRangeSafe(pawn.genes?.GetFirstGeneOfType<Gene_Shapeshifter>()?.XenotypesGenes);
					geneDefs.AddRangeSafe(pawn.genes?.GetFirstGeneOfType<Gene_Morpher>()?.AllHoldedGenes);
					cachedGeneDefs = geneDefs;
				}
				return cachedGeneDefs;
			}
		}

		public List<GeneDef> DisabledGenes => [];
		public List<GeneDef> DestroyedGenes => [];

		public Pawn Pawn => pawn;

		public GeneDef Def => def;

		public GeneExtension_Undead Extension_Undead => null;

		public int ArchiteLimit => 999;
		public int ComplexityLimit => 999;


		public List<GeneSetPreset> geneSetPresets = new();
		public List<GeneSetPreset> GeneSetPresets
		{
			get
			{
				return geneSetPresets;
			}
			set
			{
				geneSetPresets = value;
			}
		}

		public IntRange ReqMetRange => new(-5, 5);
		public bool ReqCooldown => false;
		public bool DisableSubActions => true;
		public bool UseGeneline => false;
		public List<GeneDef> GenelineGenes => [];

		//public bool IsContainer => true;

		public void ResetContainer()
		{
			cachedGeneDefs = null;
			xenotypeHolder = null;
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Deep.Look(ref xenotypeHolder, "xenotypeHolder");
			Scribe_Collections.Look(ref geneSetPresets, "geneSetPresets", LookMode.Deep);
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				if (geneSetPresets == null)
				{
					geneSetPresets = new();
				}
			}
		}

		public void Debug_RemoveDupes()
		{
			xenotypeHolder?.PostSetup();
		}

		public bool TryDisableGene(GeneDef geneDef)
		{
			return false;
		}

		public void AddGene_Editor(GeneDef geneDef)
		{
			if (xenotypeHolder == null)
			{
				xenotypeHolder = new();
			}
			xenotypeHolder.genes.AddSafe(geneDef);
		}

		public bool TryGetUniqueGene()
		{
			return false;
		}

		public bool TryGetToolGene()
		{
			return false;
		}

		public void UpdSubHediffs()
		{

		}

		public void UpdateCache()
		{
			cachedGeneDefs = null;
			Command_Ability_FloatMenu.RecacheFloatAbilities(pawn);
		}

	}

}
