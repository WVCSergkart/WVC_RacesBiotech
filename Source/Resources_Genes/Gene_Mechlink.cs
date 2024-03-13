using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Mechlink : Gene
	{

		public override void PostAdd()
		{
			base.PostAdd();
			if (!pawn.health.hediffSet.HasHediff(HediffDefOf.MechlinkImplant))
			{
				pawn.health.AddHediff(HediffDefOf.MechlinkImplant, pawn.health.hediffSet.GetBrain());
			}
		}

		// public override void Notify_PawnDied()
		// {
			// base.Notify_PawnDied();
			// if (WVC_Biotech.settings.genesRemoveMechlinkUponDeath && pawn.health.hediffSet.HasHediff(HediffDefOf.MechlinkImplant))
			// {
				// Gene_AddOrRemoveHediff.RemoveHediff(HediffDefOf.MechlinkImplant, pawn);
			// }
		// }

		public override void Reset()
		{
			base.Reset();
			if (!pawn.health.hediffSet.HasHediff(HediffDefOf.MechlinkImplant))
			{
				pawn.health.AddHediff(HediffDefOf.MechlinkImplant, pawn.health.hediffSet.GetBrain());
			}
		}

	}

	public class Gene_MechlinkWithGizmo : Gene_Mechlink
	{

		private Gizmo gizmo;

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (pawn.Faction != Faction.OfPlayer || Find.Selector.SelectedPawns.Count != 1)
			{
				yield break;
			}
			// if (MechanitorUtility.IsMechanitor(pawn))
			// {
			// }
			if (gizmo == null)
			{
				gizmo = (Gizmo)Activator.CreateInstance(def.resourceGizmoType, pawn);
			}
			yield return gizmo;
		}

	}

	[Obsolete]
	public class Gene_ResurgentMechlink : Gene_MechlinkWithGizmo
	{


	}

	public class Gene_DustMechlink : Gene_Mechlink
	{

		public GeneExtension_Spawner Props => def?.GetModExtension<GeneExtension_Spawner>();

		public int timeForNextSummon = 60000;
		public bool summonMechanoids = false;

		public override void PostAdd()
		{
			base.PostAdd();
			ResetInterval();
		}

		public override void Tick()
		{
			base.Tick();
			timeForNextSummon--;
			if (timeForNextSummon > 0)
			{
				return;
			}
			ResetInterval();
			if (!summonMechanoids)
			{
				return;
			}
			if (pawn.Map == null)
			{
				return;
			}
			if (pawn.Faction != Faction.OfPlayer)
			{
				summonMechanoids = false;
				return;
			}
			if (!MechanitorUtility.IsMechanitor(pawn))
			{
				summonMechanoids = false;
				return;
			}
			SummonRandomMech();
		}

		private void ResetInterval()
		{
			timeForNextSummon = Props.spawnIntervalRange.RandomInRange;
		}

		private void SummonRandomMech()
		{
			int countSpawn = Props.summonRange.RandomInRange;
			for (int i = 0; i < countSpawn; i++)
			{
				MechanoidsUtility.MechSummonQuest(pawn, Props.summonQuest);
				if (i == 0)
				{
					Messages.Message("WVC_RB_Gene_Summoner".Translate(pawn.LabelIndefinite().CapitalizeFirst()), pawn, MessageTypeDefOf.PositiveEvent);
				}
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref timeForNextSummon, "timeForNextSummon", 0);
			Scribe_Values.Look(ref summonMechanoids, "summonMechanoids", false);
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (!Active || Find.Selector.SelectedPawns.Count != 1 || pawn.Faction != Faction.OfPlayer)
			{
				yield break;
			}
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: Summon mechs",
					action = delegate
					{
						SummonRandomMech();
						ResetInterval();
					}
				};
				yield return new Command_Action
				{
					defaultLabel = "DEV: Get mechs list",
					action = delegate
					{
						List<PawnKindDef> pawnKindDefs = DefDatabase<PawnKindDef>.AllDefsListForReading.Where((PawnKindDef randomXenotypeDef) => MechanoidsUtility.MechanoidIsPlayerMechanoid(randomXenotypeDef)).ToList();
						if (!pawnKindDefs.NullOrEmpty())
						{
							Log.Error("Mechanoids that can be summoned:" + "\n" + pawnKindDefs.Select((PawnKindDef x) => x.defName).ToLineList(" - "));
						}
						else
						{
							Log.Error("Mechanoids list is null");
						}
					}
				};
			}
			Command_Action command_Action = new()
			{
				defaultLabel = "WVC_XaG_Gene_DustMechlink".Translate() + ": " + GeneUiUtility.OnOrOff(summonMechanoids),
				defaultDesc = "WVC_XaG_Gene_DustMechlinkDesc".Translate(),
				icon = ContentFinder<Texture2D>.Get(def.iconPath),
				action = delegate
				{
					summonMechanoids = !summonMechanoids;
					if (summonMechanoids)
					{
						SoundDefOf.Tick_High.PlayOneShotOnCamera();
					}
					else
					{
						SoundDefOf.Tick_Low.PlayOneShotOnCamera();
					}
				}
			};
			yield return command_Action;
		}

	}

}
