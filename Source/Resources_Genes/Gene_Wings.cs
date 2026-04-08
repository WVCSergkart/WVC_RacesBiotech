using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using WVC_XenotypesAndGenes.HarmonyPatches;
using static HarmonyLib.Code;

namespace WVC_XenotypesAndGenes
{

	//public class Gene_Wings : Gene_Flickable, IGeneRemoteControl
	//{
	//	public string RemoteActionName => Wings();

	//	public TaggedString RemoteActionDesc => "WVC_XaG_Gene_WingsDesc".Translate();

	//	public void RemoteControl_Action(Dialog_GenesSettings genesSettings)
	//	{
	//		AddOrRemoveHediff(pawn, Props.hediffDefName, this);
	//	}

	//	public bool RemoteControl_Hide => !Active;

	//	public bool RemoteControl_Enabled
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

	//	public void RemoteControl_Recache()
	//	{
	//		XaG_UiUtility.RecacheRemoteController(pawn, ref remoteControllerCached, ref enabled);
	//	}

	//	//=================

	//	private string Wings()
	//	{
	//		if (pawn.health.hediffSet.HasHediff(Props.hediffDefName))
	//		{
	//			return "WVC_XaG_Gene_Wings_On".Translate();
	//		}
	//		return "WVC_XaG_Gene_Wings_Off".Translate();
	//	}

	//	public override IEnumerable<Gizmo> GetGizmos()
	//	{
	//		if (enabled)
	//		{
	//			yield return XaG_UiUtility.GetRemoteControllerGizmo(pawn, remoteControllerCached, this);
	//		}
	//	}

	//}
	public class Gene_Wings : XaG_Gene, IGeneRemoteControl, IGeneOverriddenBy, IGeneInspectInfo, IGeneNotifyGenesChanged
	{

		private static HashSet<Pawn> cachedWingedPawns;
		public static HashSet<Pawn> WingedPawns
		{
			get
			{
				if (cachedWingedPawns == null)
				{
					List<Pawn> list = new();
					foreach (Pawn pawn in PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive)
					{
						if (pawn?.genes?.GetFirstGeneOfType<Gene_Wings>()?.IgnoreMovementCost == true)
						{
							list.Add(pawn);
						}
					}
					cachedWingedPawns = [.. list];
				}
				return cachedWingedPawns;
			}
		}

		public static void ResetCollection()
		{
			cachedWingedPawns = null;
		}

		//=================

		public string RemoteActionName => Wings();

		public TaggedString RemoteActionDesc => "WVC_XaG_Gene_WingsDesc".Translate();

		public void RemoteControl_Action(Dialog_GenesSettings genesSettings)
		{
			ignoreMovementCost = !ignoreMovementCost;
			ResetCollection();
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
			ResetCollection();
		}

		public bool enabled = true;
		public bool remoteControllerCached = false;

		public void RemoteControl_Recache()
		{
			XaG_UiUtility.RecacheRemoteController(pawn, ref remoteControllerCached, ref enabled);
		}

		//=================

		public override void PostAdd()
		{
			base.PostAdd();
			ResetCollection();
			HarmonyPatch();
		}

		public void Notify_OverriddenBy(Gene overriddenBy)
		{
			ResetCollection();
		}

		public void Notify_Override()
		{
			ResetCollection();
			HarmonyPatch();
		}

		public bool ignoreMovementCost = true;
		public virtual bool IgnoreMovementCost
		{
			get
			{
				return ignoreMovementCost;
			}
		}

		public string GetInspectInfo
		{
			get
			{
				if (IgnoreMovementCost)
				{
					return "WVC_XaG_Gene_Wings_On_Info".Translate();
				}
				return null;
			}
		}

		private string Wings()
		{
			if (ignoreMovementCost)
			{
				return "WVC_XaG_Gene_Wings_On".Translate();
			}
			return "WVC_XaG_Gene_Wings_Off".Translate();
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (enabled)
			{
				yield return XaG_UiUtility.GetRemoteControllerGizmo(pawn, remoteControllerCached, this);
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref ignoreMovementCost, "ignoreMovementCost", defaultValue: true);
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				HarmonyPatch();
			}
		}

		//=================


		private static bool movementCostPatched = false;
		private static void HarmonyPatch()
		{
			if (movementCostPatched)
			{
				return;
			}
			try
			{
				//var harmony = new Harmony("wvc.sergkart.races.biotech.pergene");
				HarmonyUtility.Harmony.Patch(AccessTools.Method(typeof(Pawn_PathFollower), "CostToMoveIntoCell", [typeof(Pawn), typeof(IntVec3)]), prefix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod(nameof(HarmonyUtility.NoMovementCost))));
			}
			catch (Exception arg)
			{
				Log.Error("Failed apply wings patch. Reason: " + arg.Message);
			}
			movementCostPatched = true;
		}

	}

	public class Gene_Levitation : Gene_Wings
	{

		public override bool IgnoreMovementCost
		{
			get
			{
				if (base.IgnoreMovementCost)
				{
					return pawn.IsPsychicSensitive();
				}
				return false;
			}
		}

	}

}
