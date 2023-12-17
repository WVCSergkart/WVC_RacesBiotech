using System.Collections.Generic;
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

		public HediffDef HediffDefName => def.GetModExtension<GeneExtension_Giver>().hediffDefName;

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
			}
			if (pawn.health.hediffSet.HasHediff(HediffDefName) || Find.Selector.SelectedPawns.Count > 1 || pawn.Drafted || !Active)
			{
				yield break;
			}
			yield return new Command_Action
			{
				defaultLabel = "WVC_XaG_GeneXenoGestator_Label".Translate(),
				defaultDesc = "WVC_XaG_GeneXenoGestator_Desc".Translate(),
				icon = ContentFinder<Texture2D>.Get(def.iconPath),
				action = delegate
				{
					Find.WindowStack.Add(new Dialog_ChooseXenotype(this));
				}
			};
		}

	}

}
