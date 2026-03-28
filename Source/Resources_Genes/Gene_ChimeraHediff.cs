using System;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_ChimeraHediff : Gene_ChimeraDependant, IGeneOverriddenBy, IGeneAddOrRemoveHediff
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
			Local_RemoveHediff();
		}

		public virtual void Notify_Override()
		{
			Local_AddOrRemoveHediff();
		}

		public override void TickInterval(int delta)
		{
			if (!pawn.IsHashIntervalTick(67150, delta))
			{
				return;
			}
			Local_AddOrRemoveHediff();
		}

		public override void PostRemove()
		{
			base.PostRemove();
			Local_RemoveHediff();
		}

		public virtual void Local_RemoveHediff()
		{
			HediffUtility.TryRemoveHediff(Props.hediffDefName, pawn);
		}

	}

	public class Gene_ChimeraBandwidth : Gene_ChimeraHediff, IGeneRemoteControl, IGeneMechanitorUI
	{

		public string RemoteActionName => "WVC_HideShow".Translate();

		public TaggedString RemoteActionDesc => "WVC_XaG_RemoteControlHideBandwitdhDesc".Translate();

		public void RemoteControl_Action(Dialog_GenesSettings genesSettings)
		{
			shouldHideMechanitorUI = !shouldHideMechanitorUI;
			RecacheCollection();
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
			RecacheCollection();
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

		// =======================================

		public override void PostAdd()
		{
			base.PostAdd();
			RecacheCollection();
			InitPatch();
		}

		private bool shouldHideMechanitorUI;

		public override void Notify_OverriddenBy(Gene overriddenBy)
		{
			base.Notify_OverriddenBy(overriddenBy);
			RecacheCollection();
		}

		public override void Notify_Override()
		{
			base.Notify_Override();
			RecacheCollection();
		}

		public static void RecacheCollection()
		{
			MechanoidsUtility.ResetCollection();
		}

		private void InitPatch()
		{
			MechanoidsUtility.HarmonyPatch();
		}

		public bool HideMechanitorGUI => shouldHideMechanitorUI;

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref shouldHideMechanitorUI, "shouldHideMechanitorUI", defaultValue: false);
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				InitPatch();
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
				Local_RemoveHediff();
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
