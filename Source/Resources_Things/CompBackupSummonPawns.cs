using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{
	public class CompBackupSummonPawns : ThingComp
	{

		public CompProperties_CustomFloatMenu Props => (CompProperties_CustomFloatMenu)props;

		public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
		{
			if (!CanSummon(selPawn))
			{
				yield break;
			}
			yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("WVC_XaG_GeneBackup_Summon".Translate(), delegate
			{
				MiscUtility.MakeCustomJob(selPawn, parent, Props.jobDef);
			}), selPawn, parent);
		}

		private bool CanSummon(Pawn selPawn)
		{
			if (selPawn?.mechanitor == null)
			{
				return false;
			}
			if (!CanSummon())
			{
				return false;
			}
			if (StaticCollectionsClass.GameComponent?.BackupOnCooldown == true)
			{
				return false;
			}
			return true;
		}

		private bool CanSummon()
		{
			if (Gene_Backup.BackupPawns.NullOrEmpty())
			{
				return false;
			}
			if (Props.jobDef == null)
			{
				return false;
			}
			return true;
		}

		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			if (!CanSummon() || StaticCollectionsClass.GameComponent == null)
			{
				yield break;
			}
			yield return new Command_Action
			{
				defaultLabel = "WVC_XaG_GeneBackup_Summon".Translate(),
				defaultDesc = "WVC_XaG_GeneBackup_SummonDesc".Translate(),
				icon = ContentFinder<Texture2D>.Get(Props.iconPath),
				disabledReason = "WVC_XaG_GeneBackup_Cooldown".Translate((StaticCollectionsClass.GameComponent.BackupCooldownTicks).ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor)),
				Disabled = StaticCollectionsClass.GameComponent.BackupOnCooldown,
				action = delegate
				{
					List<FloatMenuOption> list = new();
					List<Pawn> list2 = parent.MapHeld.mapPawns.SpawnedPawnsInFaction(Faction.OfPlayer);
					for (int i = 0; i < list2.Count; i++)
					{
						Pawn selPawn = list2[i];
						if (selPawn?.mechanitor != null && selPawn.CanReach(parent, PathEndMode.Touch, Danger.Deadly))
						{
							list.Add(new FloatMenuOption(selPawn.LabelShort, delegate
							{
								MiscUtility.MakeCustomJob(selPawn, parent, Props.jobDef);
							}, selPawn, Color.white));
						}
					}
					if (!list.Any())
					{
						list.Add(new FloatMenuOption("WVC_XaG_GeneBackup_NeedMechanitor".Translate(), null));
					}
					Find.WindowStack.Add(new FloatMenu(list));
				}
			};
		}

		public void InitSummon()
		{
			StaticCollectionsClass.GameComponent?.Backup_InitSummon();
		}

		//private static int cooldown = -1;
		//public override void PostExposeData()
		//{
		//	base.PostExposeData();
		//	Scribe_Values.Look(ref cooldown, "cooldown", -1);
		//}

	}

}
