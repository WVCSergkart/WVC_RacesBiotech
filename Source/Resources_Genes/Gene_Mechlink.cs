using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Mechlink : Gene
	{

		public GeneExtension_Spawner Spawner => def?.GetModExtension<GeneExtension_Spawner>();

		public int timeForNextSummon = -1;
		public bool summonMechanoids = false;

		//public bool pawnHadMechlinkBefore = false;

		public override void PostAdd()
        {
            base.PostAdd();
            if (!WVC_Biotech.settings.link_addedMechlinkWithGene)
            {
                return;
            }
			GeneResourceUtility.TryAddMechlink(pawn);
            //else
            //{
            //	pawnHadMechlinkBefore = true;
            //}
        }

        public override void Tick()
		{
			//base.Tick();
			GeneResourceUtility.TryAddMechlinkRandomly(pawn, WVC_Biotech.settings.mechlink_HediffFromGeneChance);
		}

		public bool CanDoOrbitalSummon()
		{
			if (pawn.Faction != Faction.OfPlayer)
			{
				summonMechanoids = false;
				return false;
			}
			if (pawn.Map == null)
			{
				return false;
			}
			if (!MechanitorUtility.IsMechanitor(pawn))
			{
				summonMechanoids = false;
				return false;
			}
			return true;
		}

		//public void Notify_OverriddenBy(Gene overriddenBy)
		//{
		//	if (WVC_Biotech.settings.link_removeMechlinkWithGene && !pawnHadMechlinkBefore)
		//	{
		//		HediffUtility.TryRemoveHediff(HediffDefOf.MechlinkImplant, pawn);
		//	}
		//}

		//public void Notify_Override()
		//{
		//	if (WVC_Biotech.settings.link_removeMechlinkWithGene && WVC_Biotech.settings.link_addedMechlinkWithGene)
		//	{
		//		if (!pawn.health.hediffSet.HasHediff(HediffDefOf.MechlinkImplant))
		//		{
		//			pawn.health.AddHediff(HediffDefOf.MechlinkImplant, pawn.health.hediffSet.GetBrain());
		//		}
		//	}
		//}

		//public override void PostRemove()
		//{
		//	base.PostRemove();
		//	if (WVC_Biotech.settings.link_removeMechlinkWithGene && !pawnHadMechlinkBefore)
		//	{
		//		HediffUtility.TryRemoveHediff(HediffDefOf.MechlinkImplant, pawn);
		//	}
		//}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref timeForNextSummon, "timeForNextSummon", -1);
			Scribe_Values.Look(ref summonMechanoids, "summonMechanoids", false);
			//Scribe_Values.Look(ref pawnHadMechlinkBefore, "pawnHadMechlinkBefore", false);
		}

	}

	//[Obsolete]
	//public class Gene_Stonelink : Gene_Golemlink
	//{
	//}

	public class Gene_Falselink : Gene_Mechlink, IGeneInspectInfo, IGeneRemoteControl
	{
		public string RemoteActionName => XaG_UiUtility.OnOrOff(summonMechanoids);

		public string RemoteActionDesc => "WVC_XaG_Gene_DustMechlinkDesc".Translate();

		public void RemoteControl()
		{
			summonMechanoids = !summonMechanoids;
		}

		public bool Enabled
		{
			get
			{
				return enabled;
			}
			set
			{
				enabled = value;
			}
		}

		public override void PostRemove()
		{
			base.PostRemove();
			XaG_UiUtility.ResetAllRemoteControllers(ref cachedRemoteControlGenes);
		}

		public void RecacheGenes()
		{
			XaG_UiUtility.RecacheRemoteController(pawn, ref cachedRemoteControlGenes, ref enabled);
		}

		public bool enabled = true;

		public void RemoteControl_Recache()
		{
			RecacheGenes();
		}

		private List<IGeneRemoteControl> cachedRemoteControlGenes;


		//===========

		public override void PostAdd()
		{
			base.PostAdd();
			ResetSummonInterval();
		}

		public void ResetSummonInterval()
		{
			if (Spawner == null)
			{
				return;
			}
			timeForNextSummon = WVC_Biotech.settings.falselink_spawnIntervalRange.RandomInRange;
		}

		public override void Tick()
		{
			base.Tick();
			if (!summonMechanoids)
			{
				return;
			}
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
			int countSpawn = WVC_Biotech.settings.falselink_mechsToSpawnRange.RandomInRange;
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
				foreach (Gizmo gizmo in XaG_UiUtility.GetRemoteControllerGizmo(pawn, this, cachedRemoteControlGenes))
				{
					yield return gizmo;
				}
			}
		}

		public string GetInspectInfo
		{
			get
			{
				if (pawn.mechanitor == null)
				{
					return null;
				}
				if (summonMechanoids)
				{
					return "WVC_XaG_Gene_Blesslin_On_Info".Translate().Resolve() + ": " + timeForNextSummon.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor);
				}
				return null;
			}
		}

	}

}
