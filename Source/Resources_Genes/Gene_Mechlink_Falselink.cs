﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{
    //[Obsolete]
    //public class Gene_Stonelink : Gene_Golemlink
    //{
    //}

    public class Gene_Falselink : Gene_Mechlink, IGeneInspectInfo, IGeneRemoteControl
	{
		public string RemoteActionName => XaG_UiUtility.OnOrOff(summonMechanoids);

		public TaggedString RemoteActionDesc => "WVC_XaG_Gene_DustMechlinkDesc".Translate();

		public void RemoteControl_Action(Dialog_GenesSettings genesSettings)
		{
			summonMechanoids = !summonMechanoids;
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

		public override void PostAdd()
		{
			base.PostAdd();
			ResetSummonInterval();
		}

		public void ResetSummonInterval()
		{
			timeForNextSummon = WVC_Biotech.settings.falselink_spawnIntervalRange.RandomInRange;
		}

		public override void TickInterval(int delta)
		{
			base.TickInterval(delta);
			if (!summonMechanoids)
			{
				return;
			}
			timeForNextSummon -= delta;
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
			int countSpawn = WVC_Biotech.settings.falselink_mechsToSpawnRange.RandomInRange;
            //for (int i = 0; i < countSpawn; i++)
            //{
            //	MechanoidsUtility.MechSummonQuest(pawn, Spawner.summonQuest);
            //	if (i == 0)
            //	{
            //		Messages.Message("WVC_RB_Gene_Summoner".Translate(), pawn, MessageTypeDefOf.PositiveEvent);
            //	}
            //}
            try
            {
				if (MechanoidsUtility.TrySummonMechanoids(pawn, countSpawn, Spawner.allowedMechWeightClasses, out List<Thing> summonList, Spawner.mechHediff))
				{
					pawn.mechanitor?.Notify_BandwidthChanged();
					Messages.Message("WVC_RB_Gene_Summoner".Translate(), new LookTargets(summonList), MessageTypeDefOf.PositiveEvent);
				}
			}
            catch (Exception arg)
            {
				Log.Error("Failed summon hacked mechanoid. Reason: " + arg);
            }
        }

        public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos && !XaG_GeneUtility.SelectorActiveFactionMapMechanitor(pawn, this))
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
						List<PawnKindDef> pawnKindDefs = DefDatabase<PawnKindDef>.AllDefsListForReading.Where((PawnKindDef randomXenotypeDef) => MechanoidsUtility.MechanoidIsPlayerMechanoid(randomXenotypeDef, null)).ToList();
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
			//Command_Action command_Action = new()
			//{
			//	defaultLabel = "WVC_XaG_Gene_DustMechlink".Translate() + ": " + XaG_UiUtility.OnOrOff(summonMechanoids),
			//	defaultDesc = "WVC_XaG_Gene_DustMechlinkDesc".Translate(),
			//	icon = ContentFinder<Texture2D>.Get(def.iconPath),
			//	action = delegate
			//	{
			//		summonMechanoids = !summonMechanoids;
			//		if (summonMechanoids)
			//		{
			//			SoundDefOf.Tick_High.PlayOneShotOnCamera();
			//		}
			//		else
			//		{
			//			SoundDefOf.Tick_Low.PlayOneShotOnCamera();
			//		}
			//	}
			//};
			//yield return command_Action;
			if (enabled)
			{
				foreach (Gizmo gizmo in XaG_UiUtility.GetRemoteControllerGizmo(pawn, remoteControllerCached, this))
				{
					yield return gizmo;
				}
			}
		}

		public string GetInspectInfo
		{
			get
			{
				if (!summonMechanoids)
				{
					return null;
				}
				if (pawn.mechanitor == null)
                {
                    return null;
                }
                return "WVC_XaG_Gene_Blesslin_On_Info".Translate().Resolve() + ": " + timeForNextSummon.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor);
            }
        }

	}

}
