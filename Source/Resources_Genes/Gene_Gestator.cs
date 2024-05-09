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

	public class Gene_DustGestator_TestTool : Gene_FoodEfficiency
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

	// Gene-Gestator
	public class Gene_XenotypeGestator : Gene, IGeneOverridden
	{

		public GeneExtension_Giver Giver => def?.GetModExtension<GeneExtension_Giver>();

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (Find.Selector.SelectedPawns.Count > 1 || pawn.Drafted || !Active || pawn.Faction != Faction.OfPlayer)
			{
				yield break;
			}
			// Log.Error();
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
					defaultLabel = "DEV: Get matching list",
					action = delegate
					{
						DevGetMatchingList(WVC_Biotech.settings.xenotypeGestator_GestationMatchPercent);
					}
				};
			}
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

		public void Notify_OverriddenBy(Gene overriddenBy)
		{
			RemoveHediffs();
		}

		public void RemoveHediffs()
		{
			HediffUtility.RemoveHediffsFromList(pawn, Giver?.hediffDefs);
		}

		public void Notify_Override()
		{
		}

		public override void PostRemove()
		{
			base.PostRemove();
			RemoveHediffs();
		}

	}

}
