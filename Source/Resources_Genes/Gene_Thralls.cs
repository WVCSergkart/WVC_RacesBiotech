using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Gene_ThrallMaker : Gene
	{
		//public string RemoteActionName => "WVC_HideShow".Translate();

		//public TaggedString RemoteActionDesc => "WVC_XaG_HideShowDesc".Translate();

		//public void RemoteControl_Action(Dialog_GenesSettings genesSettings)
		//{
		//	shouldDrawGizmo = !shouldDrawGizmo;
		//}

		//public bool RemoteControl_Hide => !Active;

		//public bool RemoteControl_Enabled
		//{
		//	get
		//	{
		//		return enabled;
		//	}
		//	set
		//	{
		//		enabled = value;
		//		remoteControllerCached = false;
		//	}
		//}

		//public override void PostRemove()
		//{
		//	base.PostRemove();
		//	XaG_UiUtility.SetAllRemoteControllersTo(pawn);
		//}

		//public bool enabled = true;
		//public bool remoteControllerCached = false;

		//public void RemoteControl_Recache()
		//{
		//	XaG_UiUtility.RecacheRemoteController(pawn, ref remoteControllerCached, ref enabled);
		//}


		//===========

		public virtual List<XenotypeHolder> AllowedThralls => ListsUtility.GetAllThrallHolders();

		public GeneExtension_Giver Giver => def?.GetModExtension<GeneExtension_Giver>();

		private ThrallDef thrallDef = null;

        public ThrallDef ThrallDef
        {
            get
            {
                return thrallDef;
            }
			set
            {
				thrallDef = value;
			}
        }

        //private Gizmo gizmo;

        //public bool shouldDrawGizmo = true;

        public void ThrallMakerDialog()
		{
			Find.WindowStack.Add(new Dialog_ThrallMaker(this));
		}

		public override void TickInterval(int delta)
		{

		}

		//public override IEnumerable<Gizmo> GetGizmos()
		//{
		//	if (enabled)
		//	{
		//		yield return XaG_UiUtility.GetRemoteControllerGizmo(pawn, remoteControllerCached, this);
		//	}
		//	if (shouldDrawGizmo)
		//	{
		//		if (XaG_GeneUtility.SelectorDraftedActiveFactionMap(pawn, this))
		//		{
		//			yield break;
		//		}
		//		if (gizmo == null)
		//		{
		//			gizmo = (Gizmo)Activator.CreateInstance(def.resourceGizmoType, this);
		//		}
		//		yield return gizmo;
		//	}
		//}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Defs.Look(ref thrallDef, "thrallDef");
			//Scribe_Values.Look(ref shouldDrawGizmo, "shouldDrawGizmo", defaultValue: true);
		}

	}

	public class Gene_HivemindMaker : Gene_ThrallMaker, IGeneHivemind, IGeneOverridden
	{

        public override List<XenotypeHolder> AllowedThralls => base.AllowedThralls.Where((thrall) => thrall.genes.Any((gene) => gene.IsGeneDefOfType<IGeneHivemind>())).ToList();

        public override void PostAdd()
		{
			base.PostAdd();
			ResetCollection();
		}

		public void ResetCollection()
		{
			if (!HivemindUtility.SuitableForHivemind(pawn))
			{
				return;
			}
			HivemindUtility.ResetCollection();
		}

		public void Notify_OverriddenBy(Gene overriddenBy)
		{
			ResetCollection();
		}

		public void Notify_Override()
		{
			ResetCollection();
		}

		public override void PostRemove()
		{
			base.PostRemove();
			ResetCollection();
		}

	}

}
