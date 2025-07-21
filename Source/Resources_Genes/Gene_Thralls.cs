using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Gene_ThrallMaker : Gene, IGeneRemoteControl
	{
		public string RemoteActionName => "WVC_HideShow".Translate();

		public TaggedString RemoteActionDesc => "WVC_XaG_HideShowDesc".Translate();

		public void RemoteControl_Action(Dialog_GenesSettings genesSettings)
		{
			shouldDrawGizmo = !shouldDrawGizmo;
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


		//===========

		public GeneExtension_Giver Giver => def?.GetModExtension<GeneExtension_Giver>();

		public ThrallDef thrallDef = null;

		private Gizmo gizmo;

  //      public override void PostAdd()
  //      {
  //          base.PostAdd();
		//	shouldDrawGizmo = pawn?.genes?.GetFirstGeneOfType<Gene_Resurgent>() != null;
		//}

        public bool shouldDrawGizmo = true;

		//public void Notify_GenesChanged(Gene changedGene)
		//{
		//	cachedShouldDraw = pawn?.genes?.GetFirstGeneOfType<Gene_ResurgentCells>() != null;
		//}

		public void ThrallMakerDialog()
		{
			Find.WindowStack.Add(new Dialog_ThrallMaker(this));
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (enabled)
			{
				foreach (Gizmo gizmo in XaG_UiUtility.GetRemoteControllerGizmo(pawn, remoteControllerCached, this))
				{
					yield return gizmo;
				}
			}
			//yield return new Command_Action
			//{
			//	defaultLabel = thrallDef != null ? thrallDef.LabelCap.ToString() : "WVC_XaG_XenoTreeXenotypeChooseLabel".Translate(),
			//	defaultDesc = "WVC_XaG_GeneThrallMaker_ButtonDesc".Translate(),
			//	icon = ContentFinder<Texture2D>.Get(def.iconPath),
			//	action = delegate
			//	{
			//		ThrallMakerDialog();
			//	}
			//};
			if (shouldDrawGizmo)
			{
				if (XaG_GeneUtility.SelectorDraftedActiveFactionMap(pawn, this))
				{
					yield break;
				}
				if (gizmo == null)
				{
					gizmo = (Gizmo)Activator.CreateInstance(def.resourceGizmoType, this);
				}
				yield return gizmo;
			}
		}

		//public bool gizmoCollapse = WVC_Biotech.settings.geneGizmosDefaultCollapse;

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Defs.Look(ref thrallDef, "thrallDef");
			Scribe_Values.Look(ref shouldDrawGizmo, "shouldDrawGizmo", defaultValue: true);
			//Scribe_Values.Look(ref gizmoCollapse, "gizmoCollapse", WVC_Biotech.settings.geneGizmosDefaultCollapse);
		}

    }

}
