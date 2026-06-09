using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Energyshifter_XenogenesEditor : Gene_RemoteController, IGeneDisconnectable, IGeneXenogenesEditor, IGeneWithEffects
	{

		public override string RemoteActionName => "Edit".Translate().CapitalizeFirst();
		public override TaggedString RemoteActionDesc => "WVC_XaG_Energyshaper_XenogenesEditorTip".Translate();

		public Type MasterClass => typeof(Gene_Energyshifter);

		public override string LabelCap => base.LabelCap + " (" + (int)geneticMaterial + ")";

		private Gene_Energyshifter cachedShapeshifterGene;
		public Gene_Energyshifter Energyshifter
		{
			get
			{
				if (cachedShapeshifterGene == null || !cachedShapeshifterGene.Active)
				{
					cachedShapeshifterGene = pawn?.genes?.GetFirstGeneOfType<Gene_Energyshifter>();
					if (cachedShapeshifterGene == null && MiscUtility.GameStarted())
					{
						Disabled = true;
					}
				}
				return cachedShapeshifterGene;
			}
		}

		public override void PostAdd()
		{
			base.PostAdd();
			if (MiscUtility.GameNotStarted())
			{
				geneticMaterial = new IntRange(1, 20).RandomInRange;
			}
		}

		public float ResourceConsumption_Offset => def.resourceLossPerDay;
		public float ResourceConsumption_Factor => 1f;

		public List<GeneDef> AllGenes
		{
			get
			{
				List<GeneDef> geneDefs = new();
				geneDefs.AddRangeSafe(GenelineGenes);
				geneDefs.AddRangeSafe(CollectedGenes);
				return geneDefs;
			}
		}

		public List<GeneDef> CollectedGenes
		{
			get
			{
				return Energyshifter?.CollectedGenes ?? new();
			}
		}

		public List<GeneDef> DisabledGenes => pawn.genes.GetFirstGeneOfType<Gene_Energyshifter_XenotypesUnlocker>()?.GeneDefs ?? new();
		public List<GeneDef> DestroyedGenes => new();

		private List<GeneDef> cachedGeneline;
		public List<GeneDef> GenelineGenes
		{
			get
			{
				if (cachedGeneline == null)
				{
					cachedGeneline = new();
					if (Energyshifter != null)
					{
						cachedGeneline.AddRangeSafe(Energyshifter.XenotypesGenes);
						cachedGeneline.AddRangeSafe(Energyshifter.CollectedGenes);
					}
					cachedGeneline.AddRangeSafe(OverridedGenes);
				}
				return cachedGeneline;
			}
		}

		private static List<GeneDef> cachedOverridedGenes;
		public List<GeneDef> OverridedGenes
		{
			get
			{
				if (cachedOverridedGenes == null)
				{
					List<GeneDef> other = new();
					if (Extension_Giver != null)
					{
						other = Gene_Chimera_GeneDatabase.Database.Where(geneDef => Extension_Giver.geneCategoryDefs.Contains(geneDef.displayCategory)).ToList();
					}
					cachedOverridedGenes = other;
				}
				return cachedOverridedGenes;
			}
		}

		public Pawn Pawn => pawn;
		public GeneDef Def => def;


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


		private GeneExtension_Giver cachedGeneExtension_Giver;
		public GeneExtension_Giver Extension_Giver
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

		public int ArchiteLimit => Energyshifter?.UnlcokedXenotypes?.Count ?? 3;
		public int ComplexityLimit => (int)geneticMaterial;

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

		private float geneticMaterial;
		public void TickMasterGene(int delay, int outTicks)
		{
			if (pawn.HasGenesRegrowing())
			{
				geneticMaterial += 1;
				if (geneticMaterial < 0f)
				{
					geneticMaterial = 0f;
				}
				else if (geneticMaterial > 100f)
				{
					geneticMaterial = 100f;
				}
			}
			else
			{
				ReimplanterUtility.ReduceXenogermReplicationTick(pawn, outTicks);
			}
		}

		public IntRange ReqMetRange => new(-5, 5);

		public bool ReqCooldown => false;
		public bool DisableSubActions => true;
		public bool UseGeneline => true;

		public void AddGene_Editor(GeneDef geneDef)
		{
			Energyshifter?.AddGene_Safe(geneDef, false);
			if (geneDef.biostatArc != 0)
			{
				geneticMaterial = 0f;
			}
			else
			{
				geneticMaterial -= geneDef.biostatCpx;
			}
			if (geneticMaterial < 0)
			{
				geneticMaterial = 0;
			}
		}

		public void Debug_RemoveDupes()
		{

		}

		public override bool Active
		{
			get
			{
				if (Disabled)
				{
					return false;
				}
				if (Energyshifter == null)
				{
					return false;
				}
				return base.Active;
			}
		}

		private bool disabled = false;
		public bool Disabled
		{
			get
			{
				return disabled;
			}
			set
			{
				 disabled = value;
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref disabled, "disabled", false);
			Scribe_Values.Look(ref geneticMaterial, "geneticMaterial");
			Scribe_Collections.Look(ref geneSetPresets, "geneSetPresets", LookMode.Deep);
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				if (geneSetPresets == null)
				{
					geneSetPresets = new();
				}
			}
		}

		public bool TryDisableGene(GeneDef geneDef) => false;
		public bool TryGetToolGene() => false;
		public bool TryGetUniqueGene() => false;

		public void UpdateCache()
		{
			cachedShapeshifterGene = null;
			cachedGeneline = null;
			Energyshifter?.UpdateCache();
		}

		public override void RemoteControl_Action(Dialog_GenesSettings genesSettings)
		{
			UpdateCache();
			UpdSubHediffs();
			Find.WindowStack.Add(new Dialog_XenogenesEditor(this));
			genesSettings.Close();
		}

		public void UpdSubHediffs()
		{

		}

		public void DoEffects()
		{
			Energyshifter?.DoEffects();
		}

		public void DoEffects(Pawn pawn)
		{
			DoEffects();
		}

	}

}
