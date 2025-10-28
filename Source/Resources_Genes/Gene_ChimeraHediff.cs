using System;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class Gene_ChimeraHediff : Gene_ChimeraDependant, IGeneOverridden
	{

        public GeneExtension_Giver Props => def.GetModExtension<GeneExtension_Giver>();

		public virtual Hediff ChimeraHediff
		{
			get
			{
				if (Props?.hediffDefName == null)
				{
					return null;
				}
				return pawn.health?.hediffSet?.GetFirstHediffOfDef(Props.hediffDefName);
			}
		}

		public override void PostAdd()
		{
			base.PostAdd();
			Local_AddOrRemoveHediff();
		}

		public virtual void Local_AddOrRemoveHediff()
		{
			try
			{
				HediffUtility.TryAddOrRemoveHediff(Props?.hediffDefName, pawn, this, Props?.bodyparts);
			}
			catch (Exception arg)
			{
				Log.Error("Error in Gene_ChimeraHediff in def: " + def.defName + ". Pawn: " + pawn.Name + ". Reason: " + arg);
			}
		}

		public virtual void Notify_OverriddenBy(Gene overriddenBy)
		{
			RemoveHediff();
		}

		public virtual void Notify_Override()
		{
			Local_AddOrRemoveHediff();
		}

		public override void TickInterval(int delta)
		{
			if (!pawn.IsHashIntervalTick(67200, delta))
			{
				return;
			}
			Local_AddOrRemoveHediff();
		}

		public override void PostRemove()
		{
			base.PostRemove();
			RemoveHediff();
		}

		public virtual void RemoveHediff()
		{
			HediffUtility.TryRemoveHediff(Props.hediffDefName, pawn);
		}

	}

	public class Gene_ChimeraBandwidth : Gene_ChimeraHediff, IGeneRemoteControl
	{

		public string RemoteActionName => "WVC_HideShow".Translate();

		public TaggedString RemoteActionDesc => "WVC_XaG_RemoteControlHideBandwitdhDesc".Translate();

		public void RemoteControl_Action(Dialog_GenesSettings genesSettings)
        {
            HideOrUnhideUI();
            //genesSettings.Close();
        }

        public bool RemoteControl_Hide => !WVC_Biotech.settings.enable_HideMechanitorButtonsPatch || !Active;

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
            UnhideUI();
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

		public bool shouldHideMechanitorUI = true;

        public override void Notify_OverriddenBy(Gene overriddenBy)
        {
            base.Notify_OverriddenBy(overriddenBy);
			UnhideUI();
		}

        public override void Notify_Override()
        {
            base.Notify_Override();
            Load();
        }

        private void Load()
        {
            if (!shouldHideMechanitorUI)
            {
                StaticCollectionsClass.AddHideMechanitors(pawn);
            }
        }

        private void HideOrUnhideUI()
		{
			shouldHideMechanitorUI = !shouldHideMechanitorUI;
            if (shouldHideMechanitorUI)
            {
                StaticCollectionsClass.RemoveHideMechanitors(pawn);
            }
            else
            {
                StaticCollectionsClass.AddHideMechanitors(pawn);
            }
            //StaticCollectionsClass.AddOrRemoveHideMechanitors(pawn);
        }

        private void UnhideUI()
		{
			shouldHideMechanitorUI = true;
			StaticCollectionsClass.RemoveHideMechanitors(pawn);
		}

		public override void ExposeData()
        {
            base.ExposeData();
			Scribe_Values.Look(ref shouldHideMechanitorUI, "shouldHideMechanitorUI", defaultValue: true);
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				Load();
			}
		}

	}

	/// <summary>
	/// Dormant hivemind gene. Dormant hivemind gene - do not cause recache and synchronization, but are still considered hivemind genes.
	/// </summary>
	public class Gene_HiveMind_ChimeraLimit : Gene_ChimeraHediff, IGeneHivemind, IGeneNonSync
	{

		//public override void PostAdd()
		//{
		//	ResetCollection();
		//	base.PostAdd();
		//}

		public override void Local_AddOrRemoveHediff()
		{
			if (!HivemindUtility.InHivemind(pawn))
			{
				RemoveHediff();
				return;
			}
			base.Local_AddOrRemoveHediff();
		}

		//public void ResetCollection()
		//{
		//	if (!HivemindUtility.SuitableForHivemind(pawn))
		//	{
		//		return;
		//	}
		//	HivemindUtility.ResetCollection();
		//}

		//public override void Notify_OverriddenBy(Gene overriddenBy)
		//{
		//	base.Notify_OverriddenBy(overriddenBy);
		//	ResetCollection();
		//}

		//public override void Notify_Override()
		//{
		//	ResetCollection();
		//	base.Notify_Override();
		//}

		//public override void PostRemove()
		//{
		//	base.PostRemove();
		//	ResetCollection();
		//}

	}

}
