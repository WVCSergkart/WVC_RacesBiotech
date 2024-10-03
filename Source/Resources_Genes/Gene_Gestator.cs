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
						GestationUtility.GestateChild_WithGenes(pawn);
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
						GestationUtility.GestateChild_WithGenes(pawn);
					}
				};
			}
		}

	}

	public class Gene_Parthenogenesis : Gene, IGenePregnantHuman
	{

		public void Notify_PregnancyStarted(Hediff_Pregnant pregnancy)
		{
			GeneSet geneSet = pregnancy.geneSet;
			if (geneSet != null)
			{
				HediffComp_TrueParentGenes.AddParentGenes(pawn, geneSet);
			}
			else
			{
				GeneSet newGeneSet = new();
				HediffComp_TrueParentGenes.AddParentGenes(pawn, newGeneSet);
				geneSet = newGeneSet;
			}
			geneSet.SortGenes();
		}

	}

	// Gene-Gestator
	public class Gene_XenotypeGestator : Gene, IGeneOverridden
	{

		public GeneExtension_Giver Giver => def?.GetModExtension<GeneExtension_Giver>();

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (XaG_GeneUtility.SelectorDraftedActiveFaction(pawn, this))
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
						GestationUtility.GestateChild_WithXenotype(pawn, ListsUtility.GetAllXenotypesExceptAndroids().RandomElement(), "WVC_XaG_XenoTreeBirthLabel", "WVC_XaG_XenoTreeBirthDesc");
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
			List<XenotypeDef> xenotypesDef = XaG_GeneUtility.GetAllMatchedXenotypes(pawn, ListsUtility.GetAllXenotypesExceptAndroids(), percent);
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
			// HediffUtility.Notify_GeneRemoved(this, pawn);
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
