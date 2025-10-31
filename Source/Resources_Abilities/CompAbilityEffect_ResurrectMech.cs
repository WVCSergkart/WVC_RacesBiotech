using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_ResurrectMech : CompProperties_AbilityEffect
	{

		public EffecterDef appliedEffecterDef;

		public ThrallDef thrallDef;

		public CompProperties_ResurrectMech()
		{
			compClass = typeof(CompAbilityEffect_ResurrectMech);
		}
	}

	public class CompAbilityEffect_ResurrectMech : CompAbilityEffect
	{

		public new CompProperties_ResurrectMech Props => (CompProperties_ResurrectMech)props;

		public override void Initialize(AbilityCompProperties props)
		{
			base.Initialize(props);
		}

		public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
		{
			if (!base.CanApplyOn(target, dest))
			{
				return false;
			}
			if (!target.HasThing || target.Thing is not Corpse corpse)
			{
				return false;
			}
			if (!CanResurrect(corpse))
			{
				return false;
			}
			return true;
		}

		private bool CanResurrect(Corpse corpse)
		{
			if (!corpse.InnerPawn.RaceProps.IsMechanoid)
			{
				return false;
			}
			if (corpse.InnerPawn.Faction != parent.pawn.Faction)
			{
				return false;
			}
			return true;
		}

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			base.Apply(target, dest);
			Corpse corpse = (Corpse)target.Thing;
			if (CanResurrect(corpse))
			{
				Pawn innerPawn = corpse.InnerPawn;
				ResurrectionUtility.TryResurrect(innerPawn);
				if (Props.appliedEffecterDef != null)
				{
					Effecter effecter = Props.appliedEffecterDef.SpawnAttached(innerPawn, innerPawn.MapHeld);
					effecter.Trigger(innerPawn, innerPawn);
					effecter.Cleanup();
				}
				List<Pawn> mechanitors = MechanoidsUtility.GetAllMechanitors(innerPawn.Map);
				if (!mechanitors.NullOrEmpty())
				{
					MechanoidsUtility.SetOverseer((MechanitorUtility.IsMechanitor(parent.pawn) ? parent.pawn : mechanitors.RandomElement()), innerPawn);
					//innerPawn.GetOverseer()?.relations.RemoveDirectRelation(PawnRelationDefOf.Overseer, innerPawn);
					//innerPawn.SetFaction(Faction.OfPlayer);
					//(MechanitorUtility.IsMechanitor(parent.pawn) ? parent.pawn : mechanitors.RandomElement()).relations.AddDirectRelation(PawnRelationDefOf.Overseer, innerPawn);
				}
				innerPawn.stances.stagger.StaggerFor(60);
			}
		}

		public override bool GizmoDisabled(out string reason)
		{
			reason = null;
			return false;
		}

		public override IEnumerable<Mote> CustomWarmupMotes(LocalTargetInfo target)
		{
			foreach (LocalTargetInfo affectedTarget in parent.GetAffectedTargets(target))
			{
				Thing thing = affectedTarget.Thing;
				yield return MoteMaker.MakeAttachedOverlay(thing, ThingDefOf.Mote_MechResurrectWarmupOnTarget, Vector3.zero);
			}
		}

		public override void PostApplied(List<LocalTargetInfo> targets, Map map)
		{
			Vector3 zero = Vector3.zero;
			foreach (LocalTargetInfo target in targets)
			{
				zero += target.Cell.ToVector3Shifted();
			}
			zero /= targets.Count();
			IntVec3 intVec = zero.ToIntVec3();
			EffecterDefOf.ApocrionAoeResolve.Spawn(intVec, map).EffectTick(new TargetInfo(intVec, map), new TargetInfo(intVec, map));
		}

	}

}
