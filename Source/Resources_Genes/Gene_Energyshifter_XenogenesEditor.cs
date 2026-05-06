using System;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Energyshifter_XenogenesEditor : Gene_RemoteController, IGeneDisconnectable, IGeneXenogenesEditor
	{

		public Type MasterClass => typeof(Gene_Energyshifter);

		private Gene_Energyshifter cachedShapeshifterGene;
		public Gene_Energyshifter Shapeshifter
		{
			get
			{
				if (cachedShapeshifterGene == null || !cachedShapeshifterGene.Active)
				{
					cachedShapeshifterGene = pawn?.genes?.GetFirstGeneOfType<Gene_Energyshifter>();
				}
				return cachedShapeshifterGene;
			}
		}

		public float ResourceConsumption => 0.05f;

		public List<GeneDef> AllGenes => GenelineGenes;

		public List<GeneDef> CollectedGenes => new();
		public List<GeneDef> DisabledGenes => new();
		public List<GeneDef> DestroyedGenes => new();

		private List<GeneDef> cachedGeneline;
		public List<GeneDef> GenelineGenes
		{
			get
			{
				if (cachedGeneline == null)
				{
					cachedGeneline = new();
					if (Shapeshifter != null)
					{
						cachedGeneline.AddRangeSafe(Shapeshifter.XenotypesGenes);
					}
				}
				return cachedGeneline;
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

		public int ArchiteLimit => 3;
		public int ComplexityLimit => (int)geneticMaterial;

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

		private float geneticMaterial;
		public void TickMasterGene(int delay, int outTicks)
		{
			geneticMaterial += delay;
			if (geneticMaterial < 0f)
			{
				geneticMaterial = 0f;
			}
		}

		public IntRange ReqMetRange => new(-5, 999);

		public bool ReqCooldown => true;
		public bool DisableSubActions => true;
		public bool UseGeneline => true;

		public void AddGene_Editor(GeneDef geneDef)
		{
			Shapeshifter?.AddGene_Safe(geneDef, false);
			if (geneDef.biostatArc != 0)
			{
				geneticMaterial = 0f;
			}
			else
			{
				geneticMaterial -= geneDef.biostatCpx;
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
		}

		public bool TryDisableGene(GeneDef geneDef) => false;
		public bool TryGetToolGene() => false;
		public bool TryGetUniqueGene() => false;

		public void UpdateCache()
		{
			cachedShapeshifterGene = null;
			cachedGeneline = null;
			Shapeshifter?.UpdateCache();
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

	}

}
