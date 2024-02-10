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

		public override void Notify_PawnDied()
		{
			base.Notify_PawnDied();
			if (WVC_Biotech.settings.genesRemoveMechlinkUponDeath && pawn.health.hediffSet.HasHediff(HediffDefOf.MechlinkImplant))
			{
				Gene_AddOrRemoveHediff.RemoveHediff(HediffDefOf.MechlinkImplant, pawn);
				// DamageInfo dinfo = new(DamageDefOf.ExecutionCut, 9999f, 999f, -1f, null, pawn.health.hediffSet.GetBrain());
				// pawn.TakeDamage(dinfo);
			}
		}

		public override void Reset()
		{
			base.Reset();
			if (!pawn.health.hediffSet.HasHediff(HediffDefOf.MechlinkImplant))
			{
				pawn.health.AddHediff(HediffDefOf.MechlinkImplant, pawn.health.hediffSet.GetBrain());
			}
		}

	}

	public class Gene_ResurgentMechlink : Gene_Mechlink
	{

		// public override void PostAdd()
		// {
			// base.PostAdd();
			// if (!pawn.health.hediffSet.HasHediff(HediffDefOf.MechlinkImplant))
			// {
				// pawn.health.AddHediff(HediffDefOf.MechlinkImplant, pawn.health.hediffSet.GetBrain());
			// }
		// }

		// public override void Notify_PawnDied()
		// {
			// base.Notify_PawnDied();
			// if (WVC_Biotech.settings.genesRemoveMechlinkUponDeath && pawn.health.hediffSet.HasHediff(HediffDefOf.MechlinkImplant))
			// {
				// Gene_AddOrRemoveHediff.RemoveHediff(HediffDefOf.MechlinkImplant, pawn);
			// }
		// }

		// public override void Reset()
		// {
			// base.Reset();
			// if (!pawn.health.hediffSet.HasHediff(HediffDefOf.MechlinkImplant))
			// {
				// pawn.health.AddHediff(HediffDefOf.MechlinkImplant, pawn.health.hediffSet.GetBrain());
			// }
		// }

		private Gizmo gizmo;

		public override IEnumerable<Gizmo> GetGizmos()
		{
			base.GetGizmos();
			// Pawn pawn = parent as Pawn;
			if (pawn.IsColonist && MechanitorUtility.IsMechanitor(pawn))
			{
				if (gizmo == null)
				{
					gizmo = (Gizmo)Activator.CreateInstance(def.resourceGizmoType, pawn);
				}
				if (Find.Selector.SelectedPawns.Count == 1)
				{
					yield return gizmo;
				}
				// yield return new StatDrawEntry(StatCategoryDefOf.Genetics, "DeathrestCapacity".Translate().CapitalizeFirst(), deathrestCapacity.ToString(), "DeathrestCapacityDesc".Translate(), 1010);
			}
		}

	}

	public class Gene_DustMechlink : Gene_Mechlink
	{

		public IntRange SpawnIntervalRange => def.GetModExtension<GeneExtension_Spawner>().spawnIntervalRange;
		public QuestScriptDef SummonQuest => def.GetModExtension<GeneExtension_Spawner>().summonQuest;

		public int timeForNextSummon = 60000;
		public bool summonMechanoids = false;

		public override void PostAdd()
		{
			base.PostAdd();
			ResetInterval();
			// if (pawn.Faction != null && pawn.Faction == Faction.OfPlayer)
			// {
				// summonMechanoids = true;
			// }
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
			Gene_Dust gene_Dust = pawn.genes?.GetFirstGeneOfType<Gene_Dust>();
			if (pawn.Faction != Faction.OfPlayer || !Active || pawn.Map == null || !pawn.ageTracker.Adult || gene_Dust == null)
			{
				return;
			}
			// if (!MechanoidsUtility.CanSummonMechanoidsIdeo(pawn))
			// {
				// summonMechanoids = false;
				// return;
			// }
			// if ((gene_Dust.Value - def.resourceLossPerDay) >= 0f)
			// {
			// }
			SummonRandomMech();
		}

		private void ResetInterval()
		{
			timeForNextSummon = SpawnIntervalRange.RandomInRange;
		}

		private void SummonRandomMech()
		{
			// Gene_Dust gene_Dust = pawn.genes?.GetFirstGeneOfType<Gene_Dust>();
			IntRange countSpawn = new(1, 5);
			for (int i = 0; i < countSpawn.RandomInRange; i++)
			{
				// if ((gene_Dust.Value - def.resourceLossPerDay) >= 0f)
				// {
					// gene_Dust.Value -= def.resourceLossPerDay;
				// }
				DustUtility.OffsetNeedFood(pawn, -1f * def.resourceLossPerDay);
				MechanoidsUtility.MechSummonQuest(pawn, SummonQuest);
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
			Scribe_Values.Look(ref summonMechanoids, "summonMechanoids");
		}

		// private string OnOrOff()
		// {
			// if (summonMechanoids)
			// {
				// return "WVC_XaG_Gene_DustMechlink_On".Translate();
			// }
			// return "WVC_XaG_Gene_DustMechlink_Off".Translate();
		// }

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
					defaultLabel = "DEV: Summon mech",
					action = delegate
					{
						if (Active)
						{
							SummonRandomMech();
						}
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
