using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class Gene_Skinshaper : Gene_Chameleon, IGeneCustomGraphic
	{

		//public GeneExtension_Giver Giver => def?.GetModExtension<GeneExtension_Giver>();

		//private static string cachedDescription;
		//public override TaggedString RemoteActionDesc
		//{
		//	get
		//	{
		//		if (cachedDescription == null)
		//		{
		//			StringBuilder stringBuilder = new();
		//			stringBuilder.AppendLine("WVC_XaG_RemoteControlFloatMenu_Desc".Translate());
		//			stringBuilder.AppendLine();
		//			stringBuilder.AppendLine("WVC_Style".Translate().Colorize(ColoredText.TipSectionTitleColor) + ":\n" + "WVC_XaG_GeneShapeshifterStyles_Desc".Translate());
		//			stringBuilder.AppendLine();
		//			stringBuilder.Append(LabelCap.Colorize(ColoredText.TipSectionTitleColor) + ":\n" + "WVC_XaG_GeneSkinshaper_Desc".Translate());
		//			cachedDescription = stringBuilder.ToString();
		//		}
		//		return cachedDescription;
		//	}
		//}

		//public override void RemoteControl_Action(Dialog_GenesSettings genesSettings)
		//{
		//	List<FloatMenuOption> list = new();
		//	List<Window> dialogs = new() { new Dialog_StylingGene(pawn, this, true), new Dialog_Skinshaper(this, Giver.geneSetPresets) };
		//	for (int i = 0; i < dialogs.Count; i++)
		//	{
		//		Window geneSet = dialogs[i];
		//		list.Add(new FloatMenuOption(ToStringHuman(geneSet), delegate
		//		{
		//			Find.WindowStack.Add(geneSet);
		//			genesSettings.Close();
		//		}, orderInPriority: 0 - i));
		//	}
		//	Find.WindowStack.Add(new FloatMenu(list));
		//}

		//private string ToStringHuman(Window window)
		//{
		//	return window switch
		//	{
		//		Dialog_StylingGene => "WVC_Style".Translate(),
		//		Dialog_Skinshaper => LabelCap,
		//		_ => "error",
		//	};
		//}

		//private bool? cachedIsXenogene;
		//private bool IsXenogene
		//{
		//	get
		//	{
		//		if (!cachedIsXenogene.HasValue)
		//		{
		//			cachedIsXenogene = pawn.genes.IsXenogene(this);
		//		}
		//		return cachedIsXenogene.Value;
		//	}
		//}

		//public override void PostAdd()
		//{
		//	base.PostAdd();
		//	//         if (MiscUtility.GameNotStarted())
		//	//         {
		//	//}
		//	savedPreset = pawn.genes.Xenotype?.GetModExtension<GeneExtension_Giver>()?.geneSetByDefault;
		//}

		//public void RemovePreset()
		//{
		//	if (savedPreset == null)
		//	{
		//		return;
		//	}
		//	List<Gene> pawnGenes = (IsXenogene ? pawn.genes.Xenogenes : pawn.genes.Endogenes);
		//	foreach (Gene gene in pawnGenes.ToList())
		//	{
		//		if (savedPreset.geneDefs.Contains(gene.def))
		//		{
		//			pawn.genes.RemoveGene(gene);
		//		}
		//	}
		//	//savedPreset = null;
		//}

		//public void ImplantPreset(GeneSetPresets newPreset)
		//{
		//	RemovePreset();
		//	bool canImplant = true;
		//	foreach (GeneDef geneDef in newPreset.geneDefs)
		//	{
		//		if (XaG_GeneUtility.ConflictWith(geneDef, pawn.genes.GenesListForReading))
		//		{
		//			canImplant = false;
		//			break;
		//		}
		//	}
		//	if (canImplant)
		//	{
		//		foreach (GeneDef geneDef in newPreset.geneDefs)
		//		{
		//			pawn.genes.AddGene(geneDef, IsXenogene);
		//		}
		//		savedPreset = newPreset;
		//	}
		//	else
		//	{
		//		Messages.Message("WVC_XaG_GeneSkinshaper_FailImplant".Translate(), null, MessageTypeDefOf.RejectInput, historical: false);
		//	}
		//	MiscUtility.DoShapeshiftEffects_OnPawn(pawn);
		//	ReimplanterUtility.PostImplantDebug(pawn);
		//	ReimplanterUtility.TrySetSkinAndHairGenes(pawn);
		//}

		//private GeneSetPresets savedPreset;

		//public override void ExposeData()
		//{
		//	base.ExposeData();
		//	Scribe_Values.Look(ref savedPreset, "savedPreset");
		//}
		//===========================================================

		public override string RemoteActionName => LabelCap;

		public int StyleId => 2350;

		private StyleGeneDef styleGeneDef;
		private bool updated = false;
		public StyleGeneDef StyleGeneDef
		{
			get
			{
				if (!updated && styleGeneDef == null)
				{
					styleGeneDef = DefDatabase<StyleGeneDef>.AllDefsListForReading?.Where(style => style.uniqueStyleId == StyleId && style.geneDefs != null && XaG_GeneUtility.HasAllGenes(style.GeneDefs, pawn))?.FirstOrDefault();
					updated = true;
				}
				return styleGeneDef;
			}
			set
			{
				SetNewSkin(value);
			}
		}

		private void SetNewSkin(StyleGeneDef styleGeneDef)
		{
			try
			{
				Remove_StyleGeneDef();
				Add_StyleGeneDef(styleGeneDef);
				updated = false;
			}
			catch (Exception arg)
			{
				Log.Error("Failed skin-shape. Reason: " + arg.Message);
			}
		}

		private void Remove_StyleGeneDef()
		{
			if (this.styleGeneDef?.geneDefs == null)
			{
				return;
			}
			bool xenogene = IsXenogene;
			if (xenogene)
			{
				foreach (Gene item in pawn.genes.Xenogenes.ToList())
				{
					RemoveGene(this.styleGeneDef, item);
				}
			}
			else
			{
				foreach (Gene item in pawn.genes.Endogenes.ToList())
				{
					RemoveGene(this.styleGeneDef, item);
				}
			}
			//foreach (GeneDef geneDef in styleGeneDef.geneDefs)
			//{
			//	XaG_GeneUtility.TryRemoveAllConflicts(pawn, geneDef, xenogene, new() { def });
			//}
			this.styleGeneDef = null;

			void RemoveGene(StyleGeneDef styleGeneDef, Gene item)
			{
				if (styleGeneDef.GeneDefs.Contains(item.def))
				{
					pawn.RemoveGene(item, this);
				}
			}
		}

		private bool IsXenogene => pawn.genes.IsXenogene(this);
		private void Add_StyleGeneDef(StyleGeneDef styleGeneDef)
		{
			bool xenogene = IsXenogene;
			foreach (GeneDef geneDef in styleGeneDef.GeneDefs)
			{
				//if (XaG_GeneUtility.TryRemoveAllConflicts(pawn, geneDef, xenogene, new() { def }))
				//{
				//}
				pawn.AddGene(geneDef, xenogene, this);
			}
			this.styleGeneDef = styleGeneDef;
		}


		public Color CurrentColor
		{
			get
			{
				return Color.white;
			}
			set
			{

			}
		}

		//public Color? DefaultColor => Color.white;

		public List<Color> AllColors => new();

		public override void RemoteControl_Action(Dialog_GenesSettings genesSettings)
		{
			Find.WindowStack.Add(new Dialog_StylingExtra(pawn, this, true, false, null));
			genesSettings.Close();
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Defs.Look(ref styleGeneDef, "styleGeneDef");
		}

	}
}
