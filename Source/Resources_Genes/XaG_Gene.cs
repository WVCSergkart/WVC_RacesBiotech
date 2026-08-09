using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace WVC_XenotypesAndGenes
{

	/// <summary>
	/// General WVC gene. Used for generic crap like extensions.
	/// </summary>
	public class XaG_Gene : Verse.Gene
	{

		[Unsaved(false)]
		private GeneExtension_Spawner cachedSpawnerExtension;
		public GeneExtension_Spawner Extension_Spawner
		{
			get
			{
				if (cachedSpawnerExtension == null)
				{
					//cachedSpawnerExtension = def.GetModExtension<GeneExtension_Spawner>();
					GetModExtensions();
				}
				return cachedSpawnerExtension;
			}
		}

		[Unsaved(false)]
		private GeneExtension_Giver cachedGiverExtension;
		public GeneExtension_Giver Extension_Giver
		{
			get
			{
				if (cachedGiverExtension == null)
				{
					//cachedGiverExtension = def.GetModExtension<GeneExtension_Giver>();
					GetModExtensions();
				}
				return cachedGiverExtension;
			}
		}

		[Unsaved(false)]
		private GeneExtension_Undead cachedUndeadExtension;
		public GeneExtension_Undead Extension_Undead
		{
			get
			{
				if (cachedUndeadExtension == null)
				{
					//cachedUndeadExtension = def.GetModExtension<GeneExtension_Undead>();
					GetModExtensions();
				}
				return cachedUndeadExtension;
			}
		}

		[Unsaved(false)]
		private GeneExtension_Opinion cachedOpinionExtension;
		public GeneExtension_Opinion Extension_Opinion
		{
			get
			{
				if (cachedOpinionExtension == null)
				{
					//cachedOpinionExtension = def.GetModExtension<GeneExtension_Opinion>();
					GetModExtensions();
				}
				return cachedOpinionExtension;
			}
		}

		[Unsaved(false)]
		private GeneExtension_Graphic cachedGraphicExtension;
		public GeneExtension_Graphic Extension_Graphic
		{
			get
			{
				if (cachedGraphicExtension == null)
				{
					//cachedGraphicExtension = def.GetModExtension<GeneExtension_Graphic>();
					GetModExtensions();
				}
				return cachedGraphicExtension;
			}
		}

		private void GetModExtensions()
		{
			if (def.modExtensions == null)
			{
				return;
			}
			cachedGiverExtension = null;
			cachedGraphicExtension = null;
			cachedOpinionExtension = null;
			cachedSpawnerExtension = null;
			cachedUndeadExtension = null;
			foreach (DefModExtension defModExtension in def.modExtensions)
			{
				if (defModExtension is GeneExtension_Giver geneExtension_Giver)
				{
					SetExtension(geneExtension_Giver, ref cachedGiverExtension);
				}
				else if (defModExtension is GeneExtension_Graphic geneExtension_Graphic)
				{
					SetExtension(geneExtension_Graphic, ref cachedGraphicExtension);
				}
				else if (defModExtension is GeneExtension_Opinion geneExtension_Opinion)
				{
					SetExtension(geneExtension_Opinion, ref cachedOpinionExtension);
				}
				else if (defModExtension is GeneExtension_Spawner geneExtension_Spawner)
				{
					SetExtension(geneExtension_Spawner, ref cachedSpawnerExtension);
				}
				else if (defModExtension is GeneExtension_Undead geneExtension_Undead)
				{
					SetExtension(geneExtension_Undead, ref cachedUndeadExtension);
				}
			}

			void SetExtension<DefModExtension>(DefModExtension extension, ref DefModExtension extensionRef)
			{
				if (extensionRef != null)
				{
					Log.Warning($"Duplicate {extension.ToString()} extension for geneDef: " + def.defName);
				}
				extensionRef = extension;
			}
		}

		// Cache?
		//public bool? cachedActivity;

		//private static int errorsCatch;
		//public override bool Active
		//{
		//	get
		//	{
		//		if (cachedActivity == null)
		//		{
		//			try
		//			{
		//				if (MiscUtility.GameNotStarted())
		//				{
		//					return base.Active;
		//				}
		//				cachedActivity = base.Active && (pawn?.mutant == null || InitialUtility.DisabledForMutant(pawn.mutant.Def, def));
		//			}
		//			catch (Exception arg)
		//			{
		//				cachedActivity = null;
		//				errorsCatch++;
		//				if (errorsCatch > 100)
		//				{
		//					Log.Error("Error in activity. Reason: " + arg.Message);
		//				}
		//				return base.Active;
		//			}
		//		}
		//		return cachedActivity.Value;
		//	}
		//}

	}

	/// <summary>
	/// Generic remote controller aka Genes Settings gene
	/// </summary>
	public class Gene_RemoteController : XaG_Gene, IGeneRemoteControl
	{

		public virtual string RemoteActionName => "ERR";

		public virtual TaggedString RemoteActionDesc => "ERR";

		public virtual void RemoteControl_Action(Dialog_GenesSettings genesSettings)
		{

		}

		public virtual bool RemoteControl_Hide => !Active;

		public bool RemoteControl_Enabled
		{
			get
			{
				return enabled;
			}
			set
			{
				enabled = value;
				remoteControllerCached = false;
			}
		}

		public override void PostRemove()
		{
			base.PostRemove();
			XaG_UiUtility.SetAllRemoteControllersTo(pawn);
		}

		private bool enabled = true;
		private bool remoteControllerCached = false;

		public void RemoteControl_Recache()
		{
			XaG_UiUtility.RecacheRemoteController(pawn, ref remoteControllerCached, ref enabled);
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (enabled)
			{
				yield return XaG_UiUtility.GetRemoteControllerGizmo(pawn, remoteControllerCached, this);
			}
		}

	}

	/// <summary>
	/// Basic disconnectable gene. Used by Energyshifter and Resurgent. See Dialog_ActivityManager
	/// </summary>
	public class Gene_Disconnectable : XaG_Gene, IGeneDisconnectable, IGeneOverriddenBy
	{

		//private GeneExtension_Giver cachedGeneExtension;
		//public GeneExtension_Giver Giver
		//{
		//	get
		//	{
		//		if (cachedGeneExtension == null)
		//		{
		//			cachedGeneExtension = def.GetModExtension<GeneExtension_Giver>();
		//		}
		//		return cachedGeneExtension;
		//	}
		//}


		//private GeneExtension_Undead cachedGeneExtension_Undead;
		//public GeneExtension_Undead Undead
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

		protected bool disabled = false;
		public virtual bool Disabled
		{
			get
			{
				return disabled;
			}
			set
			{
				disabled = !disabled;
			}
		}

		public virtual void Notify_OverriddenBy(Gene overriddenBy)
		{
			UpdateCache();
		}

		public virtual void Notify_Override()
		{
			UpdateCache();
		}

		public virtual Type MasterClass => typeof(Gene);
		public virtual float ResourceConsumption_Offset => def.resourceLossPerDay;
		public virtual float ResourceConsumption_Factor => 1f;

		public virtual void TickMasterGene(int factorDelayTicks, int outTicks)
		{

		}

		public virtual void UpdateCache()
		{

		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref disabled, "disabled", false);
		}

	}

	public class Gene_XenogenesEditor : XaG_Gene, IGeneXenogenesEditor
	{

		public virtual List<GeneDef> AllGenes => [];
		public virtual List<GeneDef> CollectedGenes => [];
		public virtual List<GeneDef> DisabledGenes => [];
		public virtual List<GeneDef> DestroyedGenes => [];
		public virtual List<GeneDef> GenelineGenes => [];

		public Pawn Pawn => pawn;
		public GeneDef Def => def;



		//private GeneExtension_Undead cachedGeneExtension_Undead;
		//public GeneExtension_Undead Extension_Undead
		//{
		//	get
		//	{
		//		//if (cachedGeneExtension_Undead == null)
		//		//{
		//		//	cachedGeneExtension_Undead = def.GetModExtension<GeneExtension_Undead>();
		//		//}
		//		return base.Extension_Undead;
		//	}
		//}

		public virtual int ArchiteLimit => 999;
		public virtual int ComplexityLimit => 999;

		public virtual List<GeneSetPreset> GeneSetPresets
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

		protected List<GeneSetPreset> geneSetPresets;
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Collections.Look(ref geneSetPresets, "geneSetPresets", LookMode.Deep);
		}

		public virtual IntRange ReqMetRange => new(-999, 999);
		public virtual bool ReqCooldown => false;
		public virtual bool DisableSubActions => true;
		public virtual bool UseGeneline => false;

		public virtual void AddGene_Editor(GeneDef geneDef)
		{

		}

		public virtual void Debug_RemoveDupes()
		{

		}

		public virtual bool TryDisableGene(GeneDef geneDef)
		{
			return false;
		}

		public virtual bool TryGetToolGene()
		{
			return false;
		}

		public virtual bool TryGetUniqueGene()
		{
			return false;
		}

		public virtual void UpdateCache()
		{

		}

		public virtual void UpdSubHediffs()
		{

		}

	}

}
