using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

    public class Gene_SelfOverrider : Gene, IGeneRemoteControl
	{

		public virtual string RemoteActionName => XaG_UiUtility.OnOrOff(!Overridden);

		public virtual string RemoteActionDesc => "WVC_XaG_SelfOverrideDesc".Translate();

		public virtual void RemoteControl_Action(Dialog_GenesSettings genesSettings)
		{
			if (Overridden)
            {
				OverrideBy(null);
            }
			else
			{
				OverrideBy(this);
			}
			MiscUtility.Notify_DebugPawn(pawn);
		}

		public virtual bool RemoteControl_Hide => false;

		public virtual bool RemoteControl_Enabled
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

		public virtual void RemoteControl_Recache()
		{
			XaG_UiUtility.RecacheRemoteController(pawn, ref remoteControllerCached, ref enabled);
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (enabled)
			{
				return XaG_UiUtility.GetRemoteControllerGizmo(pawn, remoteControllerCached, this);
			}
			return null;
		}

		public override void Tick()
        {

        }

	}

	public class Gene_Deathrest_SelfOverrider : Gene_Deathrest, IGeneRemoteControl
	{

		public virtual string RemoteActionName => XaG_UiUtility.OnOrOff(!Overridden);

		public virtual string RemoteActionDesc => "WVC_XaG_SelfOverrideDesc".Translate();

		public virtual void RemoteControl_Action(Dialog_GenesSettings genesSettings)
		{
			if (Overridden)
			{
				OverrideBy(null);
				base.PostAdd();
			}
			else
            {
                OverrideBy(this);
                SelfOverrideReset();
            }
            MiscUtility.Notify_DebugPawn(pawn);
		}

        public void SelfOverrideReset()
		{
			Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Deathrest);
			if (firstHediffOfDef != null)
			{
				pawn.health.RemoveHediff(firstHediffOfDef);
			}
			Hediff firstHediffOfDef2 = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.DeathrestExhaustion);
			if (firstHediffOfDef2 != null)
			{
				pawn.health.RemoveHediff(firstHediffOfDef2);
			}
			foreach (Hediff hediff in pawn.health.hediffSet.hediffs.ToList())
            {
                if (hediff.def.removeOnDeathrestStart)
                {
                    pawn.health.RemoveHediff(hediff);
                }
            }
            RemoveOldDeathrestBonuses();
        }

        public virtual bool RemoteControl_Hide => false;

		public virtual bool RemoteControl_Enabled
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

		public virtual void RemoteControl_Recache()
		{
			XaG_UiUtility.RecacheRemoteController(pawn, ref remoteControllerCached, ref enabled);
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (enabled)
			{
				return XaG_UiUtility.GetRemoteControllerGizmo(pawn, remoteControllerCached, this);
			}
			return null;
		}

	}

}
