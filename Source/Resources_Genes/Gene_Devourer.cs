using RimWorld;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Jobs;
using UnityEngine;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Devourer : Gene_RemoteController
	{

		public override string RemoteActionName => XaG_UiUtility.OnOrOff(autoCast);
		public override TaggedString RemoteActionDesc => "WVC_XaG_GeneDevourer_AutoCast".Translate();

		private bool autoCast = false;


		public override void RemoteControl_Action(Dialog_GenesSettings genesSettings)
		{
			autoCast = !autoCast;
		}

		private Ability cachedAbility;
		public Ability Ability
		{
			get
			{
				if (cachedAbility == null)
				{
					cachedAbility = pawn.abilities.GetAbility(def.abilities.First());
				}
				return cachedAbility;
			}
		}

		public bool TryHuntFatTastyAnimal(AbilityDef abilityDef)
		{
			if (Gene_Rechargeable.PawnHaveThisJob(pawn, abilityDef.jobDef))
			{
				return false;
			}
			List<Pawn> animals = new();
			foreach (Pawn target in pawn.MapHeld.mapPawns.AllPawnsSpawned)
			{
				if (target.Faction != null)
				{
					continue;
				}
				if (!target.RaceProps.Animal)
				{
					continue;
				}
				if (!target.RaceProps.canBePredatorPrey)
				{
					continue;
				}
				if (target.IsForbidden(pawn) || !pawn.CanReserveAndReach(target, PathEndMode.OnCell, pawn.NormalMaxDanger()))
				{
					continue;
				}
				animals.Add(target);
			}
			if (animals.Empty())
			{
				return false;
			}
			animals.SortBy(x => -x.BodySize);
			foreach (Pawn target in animals)
			{
				if (target.BodySize > 0.8f)
				{
					if (!MiscUtility.TryGetAbilityJob(pawn, target, abilityDef, out Job job))
					{
						continue;
					}
					pawn.TryTakeOrderedJob(job, JobTag.SatisfyingNeeds, true);
					return true;
				}
			}
			return false;
		}

		public override void TickInterval(int delta)
		{
			if (!autoCast)
			{
				return;
			}
			if (!pawn.IsHashIntervalTick(68585, delta))
			{
				return;
			}
			TryHunt();
		}

		private bool TryHunt()
		{
			if (pawn.Faction != Faction.OfPlayer)
			{
				return false;
			}
			if (pawn.Map == null)
			{
				return false;
			}
			if (pawn.Downed || pawn.Drafted)
			{
				return false;
			}
			if (Ability == null)
			{
				return false;
			}
			if (Ability.OnCooldown)
			{
				return false;
			}
			if (TryHuntFatTastyAnimal(Ability.def))
			{
				return true;
			}
			return false;
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			//foreach (Gizmo item in base.GetGizmos())
			//{
			//	yield return item;
			//}
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: TryAutoDevourer",
					action = delegate
					{
						TryHunt();
					}
				};
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref autoCast, "autoCast", false);
		}

	}

}
