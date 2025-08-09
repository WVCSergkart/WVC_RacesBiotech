using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{

	public class GeneGizmo_Morpher : Gizmo
	{

		public Pawn pawn;

		public Gene_Morpher gene;

		private static readonly CachedTexture JobIcon = new("WVC/UI/XaG_General/MorpherFindArchites_Gizmo_v0");
		private static readonly CachedTexture MenuIcon = new("WVC/UI/XaG_General/Morpher_Gizmo_v0");
        private static readonly CachedTexture AutoCastIcon = new("WVC/UI/XaG_General/Morpher_AutoCast");

        // private static readonly CachedTexture InheritableGenesIcon = new("WVC/UI/XaG_General/UI_ShapeshifterMode_Duplicate");

        public override bool Visible => true;

		public GeneGizmo_Morpher(Gene_Morpher geneShapeshifter)
			: base()
		{
			gene = geneShapeshifter;
			pawn = gene?.pawn;
            //gene.gizmoCollapse = gene.IsOneTime;
			Order = -95f;
            // if (!ModLister.CheckIdeology("Styling station"))
            // {
            // styleIcon = new("WVC/UI/XaG_General/UI_DisabledWhite");
            // }
            // if (gene.ShiftMode == null)
            // {
            // gene.Reset();
            // }
            autoCast = gene.pawn.health.hediffSet.HasHediff(gene.Giver.hediffDef);
        }

		public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
        {
            if (gene.gizmoCollapse)
            {
                Collapsed(topLeft, maxWidth);
            }
            else
            {
                Uncollapsed(topLeft, maxWidth);
            }
            return new GizmoResult(GizmoState.Clear);
        }

        private void Collapsed(Vector2 topLeft, float maxWidth)
        {
            Basic(topLeft, maxWidth, out Rect rect, out Rect rect2, out TaggedString taggedString);
            TooltipHandler.TipRegion(rect, taggedString);
            // Collapse Button
            Rect rect3 = new(rect2.x, rect2.y, rect2.width, 20f);
            XaG_UiUtility.GizmoButton(rect3, ref gene.gizmoCollapse);
        }

        private void Basic(Vector2 topLeft, float maxWidth, out Rect rect, out Rect rect2, out TaggedString taggedString)
        {
            rect = new(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
            rect2 = rect.ContractedBy(6f);
            Widgets.DrawWindowBackground(rect);
            // Tip
            taggedString = gene.LabelCap.Colorize(ColoredText.TipSectionTitleColor) + "\n\n" + gene.GizmoTootip + "\n\n" + "WVC_XaG_MorpherGizmoLimitTip".Translate(gene.FormsCount, gene.CurrentLimit, gene.IsOneTime.ToStringYesNo(), gene.PossibleXenotypes);
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
        }

        private void Uncollapsed(Vector2 topLeft, float maxWidth)
        {
            Basic(topLeft, maxWidth, out Rect _, out Rect rect2, out TaggedString taggedString);
            // Label
            Rect rect3 = new(rect2.x, rect2.y, rect2.width, 20f);
            Widgets.Label(rect3, gene.def.LabelCap);
            TooltipHandler.TipRegion(rect3, taggedString);
            XaG_UiUtility.GizmoButton(rect3, ref gene.gizmoCollapse);
            // Button
            Rect rect4 = new(rect2.x, rect2.y + 23f, 40f, 40f);
            ChangeTrigger(rect4);
            // Button
            Rect rect5 = new(rect4.x + 44f, rect4.y, rect4.width, rect4.height);
            HolderDialog(rect5);
            // Button
            Rect rect6 = new(rect5.x + 44f, rect5.y, rect5.width, rect5.height);
            AutoCast(rect6);
        }

        private bool autoCast;

        private void AutoCast(Rect rect5)
        {
            Widgets.DrawTextureFitted(rect5, AutoCastIcon.Texture, 1f);
            if (!autoCast)
            {
                Widgets.DrawTextureFitted(rect5, XaG_UiUtility.NonAggressiveRedCancelIcon.Texture, 1f);
            }
            if (Mouse.IsOver(rect5))
            {
                Widgets.DrawHighlight(rect5);
                if (Widgets.ButtonInvisible(rect5))
                {
                    if (HediffUtility.TryAddHediff(gene.Giver.hediffDef, gene.pawn, gene.def))
                    {
                        autoCast = true;
                    }
                    else
                    {
                        HediffUtility.TryRemoveHediff(gene.Giver.hediffDef, gene.pawn);
                        autoCast = false;
                    }
                }
            }
            if (Mouse.IsOver(rect5))
            {
                TooltipHandler.TipRegion(rect5, "WVC_XaG_GeneMorpherAutoCast_Desc".Translate() + "\n\n" + "WVC_XaG_GeneMorpherAutoCast_Status".Translate() + ": " + XaG_UiUtility.OnOrOff(gene.pawn.health.hediffSet.HasHediff(gene.Giver.hediffDef)));
            }
        }

        private void HolderDialog(Rect rect5)
        {
            Widgets.DrawTextureFitted(rect5, MenuIcon.Texture, 1f);
            if (Mouse.IsOver(rect5))
            {
                Widgets.DrawHighlight(rect5);
                if (Widgets.ButtonInvisible(rect5))
                {
                    if (!gene.SavedGeneSets.NullOrEmpty())
                    {
                        Find.WindowStack.Add(new Dialog_Morpher(gene));
                    }
                    else
                    {
                        Messages.Message("WVC_XaG_ReqAnyGeneSetHolders".Translate().CapitalizeFirst(), null, MessageTypeDefOf.RejectInput, historical: false);
                    }
                }
            }
            TooltipHandler.TipRegion(rect5, "WVC_XaG_GeneMorpherMenu_Desc".Translate());
        }

        private void ChangeTrigger(Rect rect4)
        {
            Widgets.DrawTextureFitted(rect4, JobIcon.Texture, 1f);
            if (Mouse.IsOver(rect4))
            {
                Widgets.DrawHighlight(rect4);
                if (Widgets.ButtonInvisible(rect4))
                {
                    if (!gene.IsOneTime)
                    {
                        FloatMenu();
                    }
                    else
                    {
                        Messages.Message("WVC_XaG_HasOneTimeMorpherGene".Translate().CapitalizeFirst(), null, MessageTypeDefOf.RejectInput, historical: false);
                    }
                }
            }
            TooltipHandler.TipRegion(rect4, "WVC_XaG_GeneMorpherChangeTriggerGene_Desc".Translate());
        }

        //public Gene_MorpherTrigger cachedTrigger = null; 

        //private void TryMorph(Rect rect4)
        //{
        //    if ()
        //    {

        //    }
        //    Widgets.DrawTextureFitted(rect4, JobIcon.Texture, 1f);
        //    if (Mouse.IsOver(rect4))
        //    {
        //        Widgets.DrawHighlight(rect4);
        //        if (Widgets.ButtonInvisible(rect4))
        //        {
        //            if (!gene.IsOneTime)
        //            {
        //                FloatMenu();
        //            }
        //            else
        //            {
        //                Messages.Message("WVC_XaG_HasOneTimeMorpherGene".Translate().CapitalizeFirst(), null, MessageTypeDefOf.RejectInput, historical: false);
        //            }
        //        }
        //    }
        //    TooltipHandler.TipRegion(rect4, "WVC_XaG_GeneMorpherChangeTriggerGene_Desc".Translate());
        //}

        public override float GetWidth(float maxWidth)
		{
            if (gene.gizmoCollapse)
            {
                return 32f;
            }
			//return 96f;
            return 140f;
        }

		//private List<GeneDef> geneTriggers = null;

		private void FloatMenu()
		{
			List<FloatMenuOption> list = new();
			List<GeneDef> geneTriggers = DefDatabase<GeneDef>.AllDefsListForReading.Where((GeneDef geneDef) => geneDef.IsGeneDefOfType<Gene_MorpherTrigger>() && !XaG_GeneUtility.HasGene(geneDef, pawn) && !geneDef.IsGeneDefOfType<Gene_AbilityMorph>()).ToList();
			if (!geneTriggers.NullOrEmpty())
			{
				for (int i = 0; i < geneTriggers.Count; i++)
				{
					GeneDef geneDef = geneTriggers[i];
					list.Add(new FloatMenuOption(geneDef.LabelCap, delegate
					{
						Thing architeCapsule = GetBestArchiteStack(pawn, false);
						if (architeCapsule != null)
                        {
							MiscUtility.MakeJobWithGeneDef(pawn, gene.Giver.morpherTriggerChangeJob, geneDef, architeCapsule);
                        }
                        else
						{
							Messages.Message("WVC_XaG_GeneMorpherChangeTriggerGene_FailMessage".Translate().CapitalizeFirst(), null, MessageTypeDefOf.RejectInput, historical: false);
						}
					}));
				}
			}
			Find.WindowStack.Add(new FloatMenu(list));
		}

        public static Thing GetBestArchiteStack(Pawn pawn, bool forced)
		{
			Danger danger = (forced ? Danger.Deadly : Danger.Some);
			return (Thing)GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(ThingDefOf.ArchiteCapsule), PathEndMode.Touch, TraverseParms.For(pawn, danger), 9999f, delegate (Thing t)
			{
				Thing chunk = (Thing)t;
				if (!pawn.CanReach(t, PathEndMode.InteractionCell, danger))
				{
					return false;
				}
				return !t.IsForbidden(pawn) && pawn.CanReserve(t, 1, -1, null, forced);
			});
		}

	}

}
