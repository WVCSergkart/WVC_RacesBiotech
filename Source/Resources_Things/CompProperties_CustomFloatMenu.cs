using RimWorld;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_CustomFloatMenu : CompProperties
	{

		public GeneDef geneDef;
		public XenotypeDef xenotypeDef;

		public JobDef jobDef;

		[NoTranslate]
		public string iconPath = "WVC/UI/Genes3/Gene_Backup";

		[NoTranslate]
		public string warningText = "WVC_XaG_GeneChimeraDevourFleshmassNucleusWarning";

		[MustTranslate]
		public string description = null;

		public float factor = 1;

		public CompProperties_CustomFloatMenu()
		{
			compClass = typeof(CompEntitiesGenes);
		}

		public override void ResolveReferences(ThingDef parentDef)
		{
			if (description.NullOrEmpty())
			{
				return;
			}
			parentDef.description += "\n\n" + description;
		}

	}

	//[Obsolete]
	//public class CompProperties_EntitiesGenes : CompProperties_CustomFloatMenu
	//{



	//}

	public class CompEntitiesGenes : ThingComp
	{

		public CompProperties_CustomFloatMenu Props => (CompProperties_CustomFloatMenu)props;

		public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
		{
			if (Props.xenotypeDef == null || Props.jobDef == null)
			{
				yield break;
			}
			if (!selPawn.IsChimerkin())
			{
				yield break;
			}
			yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("WVC_XaG_GeneChimeraEntitiesDevour".Translate(), delegate
			{
				CompStudyUnlocks study = parent.TryGetComp<CompStudyUnlocks>();
				if (study == null || study.Completed)
				{
					Dialog_MessageBox window = Dialog_MessageBox.CreateConfirmation(Props.warningText.Translate(), delegate
					{
						MiscUtility.MakeCustomJob(selPawn, parent, Props.jobDef);
					});
					Find.WindowStack.Add(window);
				}
				else
				{
					Messages.Message("WVC_XaG_GeneChimeraReqGene".Translate().CapitalizeFirst(), null, MessageTypeDefOf.RejectInput, historical: false);
				}
			}), selPawn, parent);
			//return Enumerable.Empty<FloatMenuOption>();
		}

	}

	public class CompChimeraArchiteLimit : ThingComp
	{

		public CompProperties_CustomFloatMenu Props => (CompProperties_CustomFloatMenu)props;

		public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
		{
			if (Props.jobDef == null)
			{
				yield break;
			}
			if (!selPawn.IsChimerkin())
			{
				yield break;
			}
			//Log.Error("01");
			yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("WVC_XaG_GeneChimera_ArchiteDevour".Translate(), delegate
			{
				MiscUtility.MakeCustomJob(selPawn, parent, Props.jobDef, factor: Props.factor);
			}), selPawn, parent);
			if (parent.stackCount <= 1)
			{
				yield break;
			}
			yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("WVC_XaG_GeneChimera_ArchiteDevour".Translate() + " x" + parent.stackCount, delegate
			{
				MiscUtility.MakeCustomJob(selPawn, parent, Props.jobDef, null, true, factor: Props.factor);
			}), selPawn, parent);
		}

	}

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
