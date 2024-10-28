using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class Gene_SimpleGestator : Gene, IGeneOverridden
	{
		public GeneExtension_Spawner Spawner => def?.GetModExtension<GeneExtension_Spawner>();

		public GeneExtension_Giver Giver => def?.GetModExtension<GeneExtension_Giver>();

		public virtual string Desc => "WVC_XaG_Gene_SimpleGestatorDesc".Translate();
		public virtual string Warning => "WVC_XaG_Gene_SimpleGestatorWarning".Translate();
		public virtual bool UseDialogWarning => true;


		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (XaG_GeneUtility.SelectorDraftedActiveFaction(pawn, this))
			{
				yield break;
			}
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: Spawn NewBornGenes",
					action = delegate
					{
						GestationUtility.GestateChild_WithGenes(pawn);
					}
				};
				yield return new Command_Action
				{
					defaultLabel = "DEV: Spawn NewBornXenotype",
					action = delegate
					{
						GestationUtility.GestateChild_WithXenotype(pawn, ListsUtility.GetAllXenotypesExceptAndroids().RandomElement(), "WVC_XaG_XenoTreeBirthLabel", "WVC_XaG_XenoTreeBirthDesc");
					}
				};
			}
			yield return new Command_Action
            {
				defaultLabel = LabelCap,
				defaultDesc = Desc,
				icon = ContentFinder<Texture2D>.Get(def.iconPath),
				action = delegate
				{
					if (MiscUtility.CanStartPregnancy(pawn, Giver))
					{
						if (UseDialogWarning)
						{
							Dialog_MessageBox window = Dialog_MessageBox.CreateConfirmation(Warning, delegate
							{
								StartPregnancy();
							});
							Find.WindowStack.Add(window);
						}
						else
						{
							StartPregnancy();
						}
					}
                    else
					{
						Messages.Message("WVC_XaG_Gene_SimpleGestatorFailMessage".Translate().CapitalizeFirst(), null, MessageTypeDefOf.RejectInput, historical: false);
					}
				}
			};
		}

		public virtual void StartPregnancy()
        {
            Hediff hediff = HediffMaker.MakeHediff(Spawner.gestationHediffDef, pawn);
            HediffComp_GeneHediff hediff_GeneCheck = hediff.TryGetComp<HediffComp_GeneHediff>();
            if (hediff_GeneCheck != null)
            {
                hediff_GeneCheck.geneDef = def;
            }
            pawn.health.AddHediff(hediff);
            if (Spawner.cooldownHediffDef != null)
            {
                Hediff cooldownHediff = HediffMaker.MakeHediff(Spawner.cooldownHediffDef, pawn);
                HediffComp_Disappears hediffComp_Disappears = cooldownHediff.TryGetComp<HediffComp_Disappears>();
                if (hediffComp_Disappears != null)
                {
                    hediffComp_Disappears.ticksToDisappear = 60000 * 15;
                }
                HediffComp_GeneHediff cooldownHediff_GeneCheck = cooldownHediff.TryGetComp<HediffComp_GeneHediff>();
                if (cooldownHediff_GeneCheck != null)
                {
                    cooldownHediff_GeneCheck.geneDef = def;
                }
                pawn.health.AddHediff(cooldownHediff);
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

	public class Gene_Gestator_TestTool : Gene_SimpleGestator
	{


	}

	//[Obsolete]
	//public class Gene_DustGestator_TestTool : Gene_FoodEfficiency
	//{

	//	public override IEnumerable<Gizmo> GetGizmos()
	//	{
	//		// DEV
	//		if (DebugSettings.ShowDevGizmos)
	//		{
	//			yield return new Command_Action
	//			{
	//				defaultLabel = "DEV: Spawn pawn",
	//				action = delegate
	//				{
	//					GestationUtility.GestateChild_WithGenes(pawn);
	//				}
	//			};
	//		}
	//	}

	//}

	public class Gene_Parthenogenesis : Gene_SimpleGestator, IGenePregnantHuman
	{
		public override string Warning => "WVC_XaG_Gene_ParthenogenesisWarning".Translate();

		public override string Desc => "WVC_XaG_Gene_ParthenogenesisDesc".Translate();

		public override void StartPregnancy()
		{
			MiscUtility.Impregnate(pawn);
		}

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
	public class Gene_XenotypeGestator : Gene_SimpleGestator, IGeneOverridden
	{
		public override bool UseDialogWarning => false;

		public override string Desc => "WVC_XaG_GeneXenoGestator_Desc".Translate();

		public override void StartPregnancy()
		{
			Find.WindowStack.Add(new Dialog_ChooseXenotype(this));
		}

	}

}
