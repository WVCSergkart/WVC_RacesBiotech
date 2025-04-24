using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Gene_SelfOverrider : Gene, IGeneRemoteControl, IGeneOverridden
	{

		public virtual string RemoteActionName => XaG_UiUtility.OnOrOff(!Overridden);

		public virtual string RemoteActionDesc => "WVC_XaG_SelfOverrideDesc".Translate();

		public virtual void RemoteControl_Action(Dialog_GenesSettings genesSettings)
		{
			if (base.overriddenByGene != null && base.overriddenByGene != this)
			{
				SoundDefOf.ClickReject.PlayOneShotOnCamera();
				return;
			}
			cycleTry = 0;
			if (Overridden)
			{
				overrided = false;
				OverrideBy(null);
			}
			else
			{
				overrided = true;
				OverrideBy(this);
			}
			MiscUtility.Notify_DebugPawn(pawn);
		}

		public virtual bool RemoteControl_Hide => false;

		private bool overrided = false;

		public virtual void Notify_OverriddenBy(Gene overriddenBy)
		{
			cycleTry++;
			if (cycleTry > 1000)
			{
				return;
			}
			if (overriddenBy != this)
			{
				overrided = false;
			}
			else
			{
				XaG_GeneUtility.Notify_GenesConflicts(pawn, def);
			}
		}

		private int cycleTry = 0;

		public virtual void Notify_Override()
		{
			cycleTry++;
			if (cycleTry > 1000)
			{
				return;
			}
			if (overrided)
			{
				OverrideBy(this);
			}
			else
			{
				XaG_GeneUtility.Notify_GenesConflicts(pawn, def, this);
			}
		}

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

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref overrided, "overrided", defaultValue: false);
		}

	}

    //public class Gene_Deathrest_SelfOverrider : Gene_Deathrest, IGeneRemoteControl
    //{

    //	public GeneExtension_Giver Giver => def?.GetModExtension<GeneExtension_Giver>();

    //	public virtual string RemoteActionName => XaG_UiUtility.OnOrOff(!Overridden);

    //	public virtual string RemoteActionDesc => "WVC_XaG_SelfOverrideDesc".Translate();

    //	public virtual void RemoteControl_Action(Dialog_GenesSettings genesSettings)
    //	{
    //		if (Overridden)
    //		{
    //			OverrideBy(null);
    //			base.PostAdd();
    //		}
    //		else
    //		{
    //			OverrideBy(this);
    //			SelfOverrideReset();
    //		}
    //		MiscUtility.Notify_DebugPawn(pawn);
    //	}

    //	public void SelfOverrideReset()
    //	{
    //		List<HediffDef> removeList = new();
    //		removeList.AddRange(pawn.health.hediffSet.hediffs.Where((hediff) => hediff.def.removeOnDeathrestStart).ToList().ConvertToDef());
    //		if (Giver?.hediffDefs != null)
    //		{
    //			removeList.AddRange(Giver.hediffDefs);
    //		}
    //		HediffUtility.RemoveHediffsFromList(pawn, removeList);
    //		RemoveOldDeathrestBonuses();
    //	}

    //	public virtual bool RemoteControl_Hide => false;

    //	public virtual bool RemoteControl_Enabled
    //	{
    //		get
    //		{
    //			return enabled;
    //		}
    //		set
    //		{
    //			enabled = value;
    //			remoteControllerCached = false;
    //		}
    //	}

    //	public override void PostRemove()
    //	{
    //		base.PostRemove();
    //		XaG_UiUtility.SetAllRemoteControllersTo(pawn);
    //	}

    //	public bool enabled = true;
    //	public bool remoteControllerCached = false;

    //	public virtual void RemoteControl_Recache()
    //	{
    //		XaG_UiUtility.RecacheRemoteController(pawn, ref remoteControllerCached, ref enabled);
    //	}

    //	public override IEnumerable<Gizmo> GetGizmos()
    //	{
    //		if (enabled)
    //		{
    //			return XaG_UiUtility.GetRemoteControllerGizmo(pawn, remoteControllerCached, this);
    //		}
    //		return null;
    //	}

    //}

    public class Gene_SelfOverrider_Healing : Gene_SelfOverrider
	{

		private bool? regenerateEyes;
		public bool RegenerateEyes
		{
			get
			{
				if (!regenerateEyes.HasValue)
				{
					regenerateEyes = HealingUtility.ShouldRegenerateEyes(pawn);
				}
				return regenerateEyes.Value;
			}
		}

		public override void Tick()
        {
            base.Tick();
			if (!pawn.IsHashIntervalTick(713))
			{
				return;
			}
			HealingUtility.Regeneration(pawn, 100, WVC_Biotech.settings.totalHealingIgnoreScarification, 676, RegenerateEyes);
		}

	}

	public class Gene_SelfOverrider_Stomach : Gene_SelfOverrider, IGeneNotifyGenesChanged
	{

		private float? cachedOffset;
		public float Offset
		{
			get
			{
				if (!cachedOffset.HasValue)
				{
					cachedOffset = Gene_HungerlessStomach.GetFoodOffset(pawn);
				}
				return cachedOffset.Value;
			}
		}

		public void Notify_GenesChanged(Gene changedGene)
		{
			cachedOffset = null;
		}

		public override void Tick()
		{
			base.Tick();
			if (!pawn.IsHashIntervalTick(3101))
			{
				return;
			}
			GeneResourceUtility.OffsetNeedFood(pawn, Offset);
		}

		public override void Notify_IngestedThing(Thing thing, int numTaken)
		{
			MiscUtility.TryAddFoodPoisoningHediff(pawn, thing);
		}

	}

}
