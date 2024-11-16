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

		// private static readonly CachedTexture InheritableGenesIcon = new("WVC/UI/XaG_General/UI_ShapeshifterMode_Duplicate");

		public override bool Visible => true;

		public GeneGizmo_Morpher(Gene_Morpher geneShapeshifter)
			: base()
		{
			gene = geneShapeshifter;
			pawn = gene?.pawn;
			Order = -95f;
			// if (!ModLister.CheckIdeology("Styling station"))
			// {
				// styleIcon = new("WVC/UI/XaG_General/UI_DisabledWhite");
			// }
			// if (gene.ShiftMode == null)
			// {
				// gene.Reset();
			// }
		}

		public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
		{
			Rect rect = new(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
			Rect rect2 = rect.ContractedBy(6f);
			Widgets.DrawWindowBackground(rect);
			// Tip
			TaggedString taggedString = gene.LabelCap.Colorize(ColoredText.TipSectionTitleColor) + ": " + "\n\n" + "WVC_XaG_MorpherGizmoTip".Translate() + "\n\n" + "WVC_XaG_MorpherGizmoLimitTip".Translate(gene.FormsCount, gene.CurrentLimit);
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.UpperLeft;
			// Label
			Rect rect3 = new(rect2.x, rect2.y, rect2.width, 20f);
			Widgets.Label(rect3, gene.def.LabelCap);
			TooltipHandler.TipRegion(rect3, taggedString);
			// Button
			Rect rect4 = new(rect2.x, rect2.y + 23f, 40f, 40f);
			Widgets.DrawTextureFitted(rect4, JobIcon.Texture, 1f);
			if (Mouse.IsOver(rect4))
			{
				Widgets.DrawHighlight(rect4);
				if (Widgets.ButtonInvisible(rect4))
				{
					if (pawn.genes?.GetFirstGeneOfType<Gene_MorpherOneTimeUse>() == null)
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
			// Button
			Rect rect5 = new(rect4.x + 44f, rect4.y, rect4.width, rect4.height);
			Widgets.DrawTextureFitted(rect5, MenuIcon.Texture, 1f);
			if (Mouse.IsOver(rect5))
			{
				Widgets.DrawHighlight(rect5);
				if (Widgets.ButtonInvisible(rect5))
				{
					if (!gene.GetGeneSets().NullOrEmpty())
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
			return new GizmoResult(GizmoState.Clear);
		}

		public override float GetWidth(float maxWidth)
		{
			return 96f;
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
							XaG_Job xaG_Job = new(JobMaker.MakeJob(gene.Giver.morpherTriggerChangeJob, architeCapsule));
							xaG_Job.geneDef = geneDef;
							pawn.jobs.TryTakeOrderedJob(xaG_Job, JobTag.Misc);
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
