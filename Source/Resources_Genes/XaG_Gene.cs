using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class XaG_Gene : Verse.Gene
	{

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

	// Basic remote controller
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

		public bool enabled = true;
		public bool remoteControllerCached = false;

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

	// Basic disconnectable
	public class Gene_Disconnectable : XaG_Gene, IGeneDisconnectable, IGeneOverriddenBy
	{

		private GeneExtension_Giver cachedGeneExtension;
		public GeneExtension_Giver Giver
		{
			get
			{
				if (cachedGeneExtension == null)
				{
					cachedGeneExtension = def.GetModExtension<GeneExtension_Giver>();
				}
				return cachedGeneExtension;
			}
		}


		private GeneExtension_Undead cachedGeneExtension_Undead;
		public GeneExtension_Undead Undead
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

}
