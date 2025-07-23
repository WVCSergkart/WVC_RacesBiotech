using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class Gene_RegenerationSleep : Gene_OverOverridable, IGeneRemoteControl
	{

		public string RemoteActionName => "WVC_Coma".Translate();

		public TaggedString RemoteActionDesc => "WVC_XaG_RegenerationSleepGizmoTip".Translate();

		public void RemoteControl_Action(Dialog_GenesSettings genesSettings)
		{
			//ChangeColor();
			//genesSettings.Close();
			List<FloatMenuOption> list = new();
			list.Add(new FloatMenuOption("WVC_AddComaOrExtendIt".Translate(), delegate
            {
                AddHediff();
            }, orderInPriority: 1));
			list.Add(new FloatMenuOption("WVC_EndComa".Translate(), delegate
            {
                RemoveHediff();
            }, orderInPriority: 0));
			Find.WindowStack.Add(new FloatMenu(list));
		}

        private void RemoveHediff()
        {
            Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.RegenerationComa);
            if (firstHediffOfDef != null)
            {
                HediffComp_Disappears hediffComp_Disappears = firstHediffOfDef.TryGetComp<HediffComp_Disappears>();
                if (hediffComp_Disappears != null)
                {
                    hediffComp_Disappears.ticksToDisappear = 2500;
                }
            }
        }

        private void AddHediff()
        {
            Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.RegenerationComa);
            if (firstHediffOfDef != null)
            {
                HediffComp_Disappears hediffComp_Disappears = firstHediffOfDef.TryGetComp<HediffComp_Disappears>();
                if (hediffComp_Disappears != null)
                {
                    hediffComp_Disappears.ticksToDisappear += 360000;
                }
            }
            else
            {
                pawn.health.AddHediff(HediffMaker.MakeHediff(HediffDefOf.RegenerationComa, pawn));
            }
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

        public override void Notify_OverriddenBy(Gene overriddenBy)
        {
            base.Notify_OverriddenBy(overriddenBy);
			RemoveHediff();
		}

        public override void PostRemove()
		{
			base.PostRemove();
			XaG_UiUtility.SetAllRemoteControllersTo(pawn);
			RemoveHediff();
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
				return XaG_UiUtility.GetRemoteControllerGizmo(pawn, remoteControllerCached, this);
			}
			return null;
		}

		//==========================

		//private Gizmo gizmo;

		//public override IEnumerable<Gizmo> GetGizmos()
		//{
		//	if (XaG_GeneUtility.SelectorActiveFaction(pawn, this))
		//	{
		//		yield break;
		//	}
		//	if (!def.showGizmoWhenDrafted && pawn.Drafted)
		//	{
		//		yield break;
		//	}
		//	if (gizmo == null)
		//	{
		//		gizmo = (Gizmo)Activator.CreateInstance(def.resourceGizmoType, this);
		//	}
		//	yield return gizmo;
		//}

	}

}
