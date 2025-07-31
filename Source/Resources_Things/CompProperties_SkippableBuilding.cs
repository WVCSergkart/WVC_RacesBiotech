// RimWorld.CompProperties_Toxifier
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{
    public class CompProperties_SkippableBuilding : CompProperties
	{

		[NoTranslate]
		public string activateTexPath = "WVC/UI/XaG_General/SkipThing";

		public TargetingParameters targetingParameters;

		public CompProperties_SkippableBuilding()
		{
			compClass = typeof(CompSkippableBuilding);
		}

	}

	public class CompSkippableBuilding : ThingComp, ITargetingSource
	{


		public bool CasterIsPawn => true;

		public bool IsMeleeAttack => false;

		public bool Targetable => true;

		public bool MultiSelect => false;

		public bool HidePawnTooltips => false;

		public Thing Caster => parent;

		public Pawn CasterPawn => null;

		public Verb GetVerb => null;

		public TargetingParameters targetParams => Props.targetingParameters;

		public virtual ITargetingSource DestinationSelector => null;

		public Texture2D UIIcon
		{
			get
			{
				return ContentFinder<Texture2D>.Get(Props.activateTexPath);
			}
		}

		public bool CanHitTarget(LocalTargetInfo target)
		{
			return ValidateTarget(target, showMessages: false);
		}

		private CompProperties_SkippableBuilding Props => (CompProperties_SkippableBuilding)props;

		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			Command_Action command_Action = new()
			{
				defaultLabel = "WVC_XaG_SkipThing".Translate(),
				defaultDesc = "WVC_XaG_SkipThingDesc".Translate(),
				Disabled = lastSkipTick > Find.TickManager.TicksGame,
				disabledReason = "WVC_XaG_SkipThing_Disabled".Translate(),
				icon = UIIcon,
				action = delegate
				{
					SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
					Find.Targeter.BeginTargeting(this);
				}
			};
			yield return command_Action;
		}

		public void DrawHighlight(LocalTargetInfo target)
		{
			if (target.IsValid)
			{
				GenDraw.DrawTargetHighlight(target);
			}
		}

		public virtual bool ValidateTarget(LocalTargetInfo target, bool showMessages = true)
		{
			if (target.HasThing || !target.Cell.Standable(parent.Map) || target.Cell.Fogged(parent.Map) || target.Cell.IsBuildingInteractionCell(parent.Map))
			{
				return false;
			}
			return true;
		}

		public virtual void OrderForceTarget(LocalTargetInfo target)
		{
			if (ValidateTarget(target, showMessages: false))
			{
				//Log.Error("Try Move");
				parent.Map.effecterMaintainer.AddEffecterToMaintain(EffecterDefOf.Skip_EntryNoDelay.Spawn(parent.Position, parent.Map), parent.Position, 60);
				SoundDefOf.Psycast_Skip_Entry.PlayOneShot(new TargetInfo(parent.Position, parent.Map));
				parent.Position = target.Cell;
				parent.Map.effecterMaintainer.AddEffecterToMaintain(EffecterDefOf.Skip_ExitNoDelay.Spawn(target.Cell, parent.Map), target.Cell, 60);
				SoundDefOf.Psycast_Skip_Entry.PlayOneShot(new TargetInfo(target.Cell, parent.Map));
				lastSkipTick = Find.TickManager.TicksGame + 30000;
			}
		}

		public void OnGUI(LocalTargetInfo target)
		{
			string label = "WVC_XaG_SkipThing_ChooseSkipLocation".Translate();
			Widgets.MouseAttachedLabel(label);
			if (ValidateTarget(target, showMessages: false))
			{
				GenUI.DrawMouseAttachment(UIIcon);
			}
			else
			{
				GenUI.DrawMouseAttachment(TexCommand.CannotShoot);
			}
		}

		private int lastSkipTick = -1;

        public override void PostExposeData()
        {
            base.PostExposeData();
			Scribe_Values.Look(ref lastSkipTick, "lastSkipTick", -1);
		}

    }

	public class CompSkippableDryadPod : CompSkippableBuilding
	{

		public CompDryadHolder_WithGene DryadHolder => parent.TryGetComp<CompDryadHolder_WithGene>();

		//public override bool ValidateTarget(LocalTargetInfo target, bool showMessages = true)
		//{
		//	if (PawnIsNotPsycaster)
		//	{
		//		return false;
		//	}
		//	return base.ValidateTarget(target, showMessages);
		//}

        public bool PawnIsNotPsycaster => DryadHolder?.DryadComp?.Master?.HasPsylink != true;

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			if (PawnIsNotPsycaster)
            {
				yield break;
            }
			foreach (Gizmo item in base.CompGetGizmosExtra())
			{
				yield return item;
			}
		}

	}

}
