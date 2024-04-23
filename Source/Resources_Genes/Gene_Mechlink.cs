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

		public GeneExtension_Spawner Spawner => def?.GetModExtension<GeneExtension_Spawner>();

		public int timeForNextSummon = -1;
		public bool summonMechanoids = false;

		public override void PostAdd()
		{
			base.PostAdd();
			if (!pawn.health.hediffSet.HasHediff(HediffDefOf.MechlinkImplant))
			{
				pawn.health.AddHediff(HediffDefOf.MechlinkImplant, pawn.health.hediffSet.GetBrain());
			}
			ResetSummonInterval();
		}

		public void ResetSummonInterval()
		{
			if (Spawner == null)
			{
				return;
			}
			timeForNextSummon = Spawner.spawnIntervalRange.RandomInRange;
		}

		public bool CanDoOrbitalSummon()
		{
			if (!summonMechanoids)
			{
				return false;
			}
			if (pawn.Faction != Faction.OfPlayer)
			{
				summonMechanoids = false;
				return false;
			}
			if (pawn.Map == null)
			{
				return false;
			}
			// if (!CommsConsoleUtility.PlayerHasPoweredCommsConsole(pawn.Map))
			// {
				// return false;
			// }
			if (!MechanitorUtility.IsMechanitor(pawn))
			{
				summonMechanoids = false;
				Reset();
				return false;
			}
			return true;
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

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref timeForNextSummon, "timeForNextSummon", -1);
			Scribe_Values.Look(ref summonMechanoids, "summonMechanoids", false);
		}

	}

	public class Gene_MechlinkWithGizmo : Gene_Mechlink
	{

		public GeneExtension_Giver Giver => def?.GetModExtension<GeneExtension_Giver>();

		private Gizmo gizmo;

		public override IEnumerable<Gizmo> GetGizmos()
		{
			// if (!Active || Find.Selector.SelectedPawns.Count != 1 || pawn.Faction != Faction.OfPlayer)
			// {
				// yield break;
			// }
			if (pawn?.Map == null)
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

	[Obsolete]
	public class Gene_Sporelink : Gene_MechlinkWithGizmo
	{

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (!Active || Find.Selector.SelectedPawns.Count != 1 || pawn.Faction != Faction.OfPlayer)
			{
				yield break;
			}
			foreach (Gizmo item in base.GetGizmos())
			{
				yield return item;
			}
			Command_Action command_Action = new()
			{
				defaultLabel = "WVC_XaG_Gene_Sporelink_ConnectWithResurgentTrees".Translate() + ": " + GeneUiUtility.OnOrOff(summonMechanoids),
				defaultDesc = "WVC_XaG_Gene_Sporelink_ConnectWithResurgentTreesDesc".Translate(),
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

	public class Gene_Stonelink : Gene_MechlinkWithGizmo, IGeneInspectInfo
	{

		public override void Tick()
		{
			base.Tick();
			timeForNextSummon--;
			if (timeForNextSummon > 0)
			{
				return;
			}
			if (CanDoOrbitalSummon())
			{
				SummonRandomMech();
			}
			ResetSummonInterval();
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
					defaultLabel = "DEV: Summon golems",
					action = delegate
					{
						SummonRandomMech();
						ResetSummonInterval();
					}
				};
			}
			foreach (Gizmo item in base.GetGizmos())
			{
				yield return item;
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

		private void SummonRandomMech()
		{
			int countSpawn = Spawner.summonRange.RandomInRange;
			for (int i = 0; i < countSpawn; i++)
			{
				if (!MechanoidsUtility.HasEnoughGolembond(pawn))
				{
					break;
				}
				MechanoidsUtility.MechSummonQuest(pawn, Spawner.summonQuest);
				if (i == 0)
				{
					Messages.Message("WVC_RB_Gene_Summoner".Translate(), pawn, MessageTypeDefOf.PositiveEvent);
				}
			}
		}

		public string GetInspectInfo
		{
			get
			{
				if (summonMechanoids)
				{
					return "WVC_XaG_Gene_Blesslin_On_Info".Translate().Resolve() + ": " + timeForNextSummon.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor);
				}
				return null;
			}
		}

	}

	public class Gene_Falselink : Gene_Mechlink, IGeneInspectInfo
	{

		public override void Tick()
		{
			base.Tick();
			timeForNextSummon--;
			if (timeForNextSummon > 0)
			{
				return;
			}
			if (CanDoOrbitalSummon())
			{
				SummonRandomMech();
			}
			ResetSummonInterval();
		}

		private void SummonRandomMech()
		{
			int countSpawn = Spawner.summonRange.RandomInRange;
			for (int i = 0; i < countSpawn; i++)
			{
				MechanoidsUtility.MechSummonQuest(pawn, Spawner.summonQuest);
				if (i == 0)
				{
					Messages.Message("WVC_RB_Gene_Summoner".Translate(), pawn, MessageTypeDefOf.PositiveEvent);
				}
			}
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
						ResetSummonInterval();
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

		public string GetInspectInfo
		{
			get
			{
				if (summonMechanoids)
				{
					return "WVC_XaG_Gene_Blesslin_On_Info".Translate().Resolve() + ": " + timeForNextSummon.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor);
				}
				return null;
			}
		}

	}

}
