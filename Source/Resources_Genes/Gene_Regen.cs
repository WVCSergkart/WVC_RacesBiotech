using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{

	public class Gene_MachineWoundHealing : Gene_OverOverridable
	{

		public GeneExtension_Undead Undead => def.GetModExtension<GeneExtension_Undead>();

		public override void Tick()
		{
			// base.Tick();
			if (!pawn.IsHashIntervalTick(676))
			{
				return;
			}
			HealingUtility.Regeneration(pawn, Undead.regeneration, WVC_Biotech.settings.totalHealingIgnoreScarification, 676);
		}

	}

	[Obsolete]
	public class Gene_Regeneration : Gene_MachineWoundHealing
	{


	}

	public class Gene_SelfRepair : Gene_MachineWoundHealing, IGeneFloatMenuOptions
	{

		public GeneExtension_Giver Props => def?.GetModExtension<GeneExtension_Giver>();

		public IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
		{
			if (XaG_GeneUtility.ActiveDowned(pawn, this))
			{
				yield break;
			}
			if (selPawn.mechanitor == null)
			{
				yield break;
			}
			if (!selPawn.CanReserveAndReach(pawn, PathEndMode.Touch, Danger.Deadly))
			{
				yield break;
			}
			yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("WVC_Repair".Translate() + " " + pawn.LabelShort, delegate
            {
                GiveRepairJob(selPawn);
            }), selPawn, pawn);
		}

        public void GiveRepairJob(Pawn selPawn)
        {
            Job job = JobMaker.MakeJob(Props.repairJobDef, pawn);
            selPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
        }

        //private int repairTick = 10;

        public virtual void Notify_RepairedBy(Pawn worker, int tick)
		{
			HealingUtility.Regeneration(pawn, Undead.regeneration * 10, WVC_Biotech.settings.totalHealingIgnoreScarification, tick);
			pawn.stances.stunner.StunFor(tick, worker, addBattleLog: false);
			GeneResourceUtility.OffsetNeedFood(pawn, -0.01f);
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (pawn.mechanitor == null)
			{
				yield break;
			}
			if (XaG_GeneUtility.ActiveDowned(pawn, this))
			{
				yield break;
			}
			yield return new Command_Action
			{
				defaultLabel = "WVC_OrderToRepair".Translate(),
				defaultDesc = "WVC_OrderToRepairDesc".Translate(),
				icon = ContentFinder<Texture2D>.Get(def.iconPath),
				action = delegate
				{
					List<FloatMenuOption> list = new();
					List<Pawn> list2 = pawn.mechanitor.ControlledPawns;
					for (int i = 0; i < list2.Count; i++)
					{
						Pawn mech = list2[i];
						if (mech.CanReserveAndReach(pawn, PathEndMode.Touch, Danger.Deadly))
						{
							list.Add(new FloatMenuOption(mech.LabelShort, delegate
							{
								GiveRepairJob(mech);
							}, mech, Color.white));
						}
					}
					if (!list.Any())
					{
						list.Add(new FloatMenuOption("WVC_XaG_NoSuitableTargets".Translate(), null));
					}
					Find.WindowStack.Add(new FloatMenu(list));
				}
			};
		}

		public override void Tick()
		{
			if (!pawn.IsHashIntervalTick(1626))
			{
				return;
			}
			if (pawn.Drafted)
			{
				return;
			}
			if ((pawn.Downed || !pawn.Awake() || pawn.Deathresting) && pawn.Map != null)
			{
				if (pawn.mechanitor != null && pawn.health.summaryHealth.SummaryHealthPercent < 1f)
				{
					GiveRepairJob(pawn.mechanitor.ControlledPawns.Where((mech) => mech.CanReserveAndReach(pawn, PathEndMode.Touch, Danger.Deadly)).RandomElement());
				}
				return;
			}
			HealingUtility.Regeneration(pawn, Undead.regeneration, WVC_Biotech.settings.totalHealingIgnoreScarification, 1626);
		}

	}

	// Health
	public class Gene_HealingStomach : Gene
	{

		public override void Tick()
		{
			//base.Tick();
			if (!pawn.IsHashIntervalTick(2317))
			{
				return;
			}
			EatWounds();
		}

		public void EatWounds()
		{
			List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
			float eatedDamage = 0f;
			foreach (Hediff hediff in hediffs.ToList())
			{
				if (hediff is not Hediff_Injury injury)
				{
					continue;
				}
				if (hediff.def == HediffDefOf.Scarification && WVC_Biotech.settings.totalHealingIgnoreScarification)
				{
					continue;
				}
				eatedDamage += 0.005f;
				injury.Heal(0.5f);
			}
			GeneResourceUtility.OffsetNeedFood(pawn, eatedDamage);
		}

	}

}
