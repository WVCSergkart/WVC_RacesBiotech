using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class Gene_Gestator_TestTool : Gene
	{

		public override IEnumerable<Gizmo> GetGizmos()
		{
			// DEV
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: Spawn pawn",
					action = delegate
					{
						GestationUtility.GenerateNewBornPawn(pawn);
					}
				};
			}
		}

	}

	public class Gene_DustGestator_TestTool : Gene_DustDrain
	{

		public override IEnumerable<Gizmo> GetGizmos()
		{
			// DEV
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: Spawn pawn",
					action = delegate
					{
						GestationUtility.GenerateNewBornPawn(pawn);
					}
				};
			}
		}

	}

	// Finally Gene-Gestator
	public class Gene_XenotypeGestator : Gene
	{

		// public HediffDef HediffDefName => def.GetModExtension<GeneExtension_XenotypeGestator>().hediffDefName;

		// public float MatchPercent => def.GetModExtension<GeneExtension_XenotypeGestator>().matchPercent;

		// public int MinimumDays => def.GetModExtension<GeneExtension_XenotypeGestator>().minimumDays;

		// public int gestatorCooldown = 0;

		// public override void ExposeData()
		// {
			// base.ExposeData();
			// Scribe_Defs.Look(ref cooldownHediffDef, "cooldownHediffDef");
		// }

		public Hediff PreventGestation => HediffUtility.GetFirstHediffPreventsPregnancy(pawn.health.hediffSet.hediffs);

		public override IEnumerable<Gizmo> GetGizmos()
		{
			// DEV
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: Spawn pawn",
					action = delegate
					{
						GestationUtility.GenerateNewBornPawn_WithChosenXenotype(pawn, XenotypeFilterUtility.AllXenotypesExceptAndroids().RandomElement(), "WVC_XaG_XenoTreeBirthLabel", "WVC_XaG_XenoTreeBirthDesc", true);
					}
				};
				yield return new Command_Action
				{
					defaultLabel = "DEV: Get matching list 60%",
					action = delegate
					{
						DevGetMatchingList(0.6f);
					}
				};
				yield return new Command_Action
				{
					defaultLabel = "DEV: Get matching list 40%",
					action = delegate
					{
						DevGetMatchingList(0.4f);
					}
				};
				yield return new Command_Action
				{
					defaultLabel = "DEV: Get matching list 20%",
					action = delegate
					{
						DevGetMatchingList(0.2f);
					}
				};
			}
			if (Find.Selector.SelectedPawns.Count > 1 || pawn.Drafted || !Active || !MiscUtility.PawnIsColonistOrSlave(pawn, true))
			{
				yield break;
			}
			// Log.Error();
			yield return new Command_Action
			{
				defaultLabel = "WVC_XaG_GeneXenoGestator_Label".Translate(),
				defaultDesc = "WVC_XaG_GeneXenoGestator_Desc".Translate(),
				disabled = PreventGestation != null,
				disabledReason = "WVC_XaG_GeneXenoGestator_Disabled".Translate(PreventGestation != null ? PreventGestation.def.label : "ERR"),
				icon = ContentFinder<Texture2D>.Get(def.iconPath),
				action = delegate
				{
					Find.WindowStack.Add(new Dialog_ChooseXenotype(this));
				}
			};
		}

		private void DevGetMatchingList(float percent = 0.6f)
		{
			List<XenotypeDef> xenotypesDef = XaG_GeneUtility.GetAllMatchedXenotypes(pawn, XenotypeFilterUtility.AllXenotypesExceptAndroids(), percent);
			if (!xenotypesDef.NullOrEmpty())
			{
				Log.Error("All matched xenotypes:" + "\n" + xenotypesDef.Select((XenotypeDef x) => x.defName).ToLineList(" - "));
			}
			else
			{
				Log.Error("Match list is null");
			}
		}

	}

}
