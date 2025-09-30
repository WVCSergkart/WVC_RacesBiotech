using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Gene_VoidlinkDependant : Gene, IGeneOverridden
	{

		//public GeneExtension_Giver Giver => def?.GetModExtension<GeneExtension_Giver>();

		[Unsaved(false)]
		private Gene_Voidlink cachedGene;

		public Gene_Voidlink MasterGene
		{
			get
			{
				if (cachedGene == null || !cachedGene.Active)
				{
					cachedGene = pawn?.genes?.GetFirstGeneOfType<Gene_Voidlink>();
				}
				return cachedGene;
			}
		}

        public virtual void Notify_OverriddenBy(Gene overriddenBy)
		{
			MasterGene?.CacheReset();
		}

        public virtual void Notify_Override()
		{
			MasterGene?.CacheReset();
		}

        public override void PostAdd()
		{
			base.PostAdd();
			MasterGene?.CacheReset();
		}

		public override void PostRemove()
		{
			base.PostRemove();
			MasterGene?.CacheReset();
		}

	}

	public class Gene_VoidlinkOffset : Gene_VoidlinkDependant
	{

		public virtual float ResourceGain => def.resourceLossPerDay / 60000;

		public override void TickInterval(int delta)
		{
			if (pawn.IsHashIntervalTick(2500, delta))
			{
				MasterGene?.OffsetResource(ResourceGain * 2500);
			}
		}

	}

	//public class Gene_VoidlinkMaxResource : Gene_VoidlinkDependant
	//{

	//}

	public class Gene_Voidlink_MechCharging : Gene_VoidlinkDependant, IGeneRemoteControl
	{

		public string RemoteActionName => XaG_UiUtility.OnOrOff(charge);

		public TaggedString RemoteActionDesc => "WVC_XaG_VoidMechChargingDesc".Translate();

		public void RemoteControl_Action(Dialog_GenesSettings genesSettings)
		{
			charge = !charge;
		}

		public bool RemoteControl_Hide => !Active;

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

		// =================================

		public override void TickInterval(int delta)
		{
			if (pawn.IsHashIntervalTick(7764, delta))
			{
				Charge(7764);
			}
		}

		public void Charge(int tick)
		{
			if (pawn.Faction != Faction.OfPlayer)
			{
				return;
			}
			if (pawn.mechanitor == null)
			{
				return;
			}
			if (MasterGene == null)
            {
				return;
            }
			foreach (Pawn mech in pawn.mechanitor.ControlledPawns)
			{
				Need_MechEnergy need_MechEnergy = mech.needs?.energy;
				if (need_MechEnergy != null)
				{
                    float value = Mathf.Clamp(0.20f / 60000 * tick, 0f, need_MechEnergy.MaxLevel - need_MechEnergy.CurLevel);
                    need_MechEnergy.CurLevel += value;
					MasterGene?.OffsetResource(-value);
				}
			}
		}

		private bool charge = false;
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref charge, "charge", defaultValue: false);
		}

	}

}
