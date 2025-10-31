using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Gene_SelfOverrider : Gene, IGeneRemoteControl, IGeneOverridden
	{

		public int lastTick = -1;

		public virtual string RemoteActionName
		{
			get
			{
				int tick = lastTick - Find.TickManager.TicksGame;
				if (tick > 0)
				{
					return tick.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor);
				}
				return XaG_UiUtility.OnOrOff(!Overridden);
			}
		}

		public virtual TaggedString RemoteActionDesc => "WVC_XaG_SelfOverrideDesc".Translate();

		public virtual void RemoteControl_Action(Dialog_GenesSettings genesSettings)
		{
			if (OverridedByNonOverrider || lastTick > Find.TickManager.TicksGame)
			{
				SoundDefOf.ClickReject.PlayOneShotOnCamera();
				return;
			}
			cycleTry = 0;
			ResetCooldown();
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
			GeneResourceUtility.UpdMetabolism(pawn);
			MiscUtility.Notify_DebugPawn(pawn);
		}

		public virtual void ResetCooldown()
		{
			lastTick = Find.TickManager.TicksGame + 120;
		}

		public bool OverridedByNonOverrider => base.overriddenByGene != null && base.overriddenByGene != this && base.overriddenByGene?.overriddenByGene != this;

		public virtual bool RemoteControl_Hide => OverridedByNonOverrider;

		public bool overrided = false;
		//public bool OverridedBeforeSave => overrided;

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
				yield return XaG_UiUtility.GetRemoteControllerGizmo(pawn, remoteControllerCached, this);
			}
		}

		public override void TickInterval(int delta)
		{

		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref overrided, "overrided", defaultValue: false);
		}

	}

}
