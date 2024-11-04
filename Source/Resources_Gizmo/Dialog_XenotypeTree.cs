using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Dialog_XenotypeTree : Dialog_XenotypeHolderBasic
	{
		public CompXenoTree xenoTree;

		public List<Pawn> allColonists;

		public float matchPercent;

		public XenotypeHolder currentHolder;

		protected override string Header => xenoTree.parent.LabelCap;

		public override List<XenotypeHolder> XenotypesInOrder
		{
			get
			{
				if (cachedXenotypeDefsInOrder == null)
				{
					cachedXenotypeDefsInOrder = new();
					foreach (XenotypeHolder allDef in allXenotypes)
					{
						cachedXenotypeDefsInOrder.Add(allDef);
					}
					cachedXenotypeDefsInOrder.SortBy((XenotypeHolder x) => 0f - x.displayPriority);
				}
				return cachedXenotypeDefsInOrder;
			}
		}

		public Dialog_XenotypeTree(Thing tree)
		{
			xenoTree = tree.TryGetComp<CompXenoTree>();
			GetCurrentXenotypeHolder();
			selectedXenoHolder = currentHolder ?? allXenotypes.First();
			allColonists = tree.Map.mapPawns.FreeColonistsAndPrisoners.ToList();
			matchPercent = xenoTree.Props.minMatchingGenes;
			UpdAllMatchedXenotypes_ForPawns(allColonists, allXenotypes, matchPercent);
		}

		public void GetCurrentXenotypeHolder()
		{
			XenotypeHolder treeHolder = xenoTree.xenotypeHolder;
			if (treeHolder == null)
			{
				currentHolder = null;
				return;
			}
			foreach (XenotypeHolder xenotypeHolder in allXenotypes)
			{
				if (xenotypeHolder.xenotypeDef == treeHolder.xenotypeDef && xenotypeHolder.name == treeHolder.name && xenotypeHolder.iconDef == treeHolder.iconDef && xenotypeHolder.genes.Count == treeHolder.genes.Count)
				{
					currentHolder = xenotypeHolder;
					break;
				}
			}
		}

		protected override bool CanAccept()
		{
			if (currentHolder == selectedXenoHolder)
			{
				Messages.Message("WVC_XaG_XenoTreeMode_AlreadySelected".Translate(), null, MessageTypeDefOf.RejectInput, historical: false);
				return false;
			}
			if (Find.TickManager.TicksGame < xenoTree.changeCooldown)
			{
				Messages.Message("WVC_XaG_XenoTreeXenotypeChangeCooldown".Translate(xenoTree.changeCooldown.ToStringTicksToPeriod()), null, MessageTypeDefOf.RejectInput, historical: false);
				return false;
			}
			if (selectedXenoHolder.isOverriden)
			{
				Messages.Message("WVC_XaG_GeneXenoGestator_GestationGenesMatch".Translate((matchPercent * 100).ToString()), null, MessageTypeDefOf.RejectInput, historical: false);
				return false;
			}
			return true;
		}

		public static void UpdAllMatchedXenotypes_ForPawns(List<Pawn> pawns, List<XenotypeHolder> xenotypeDefs, float percent = 0.6f)
		{
			foreach (Pawn pawn in pawns)
			{
				Dialog_XenotypeGestator.UpdAllMatchedXenotypeHolders(pawn, xenotypeDefs, percent);
			}
		}

		protected override void Accept()
		{
			Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("WVC_XaG_XenoTreeModeChangeDescFull".Translate(xenoTree.parent.LabelCap), StartChange));
		}

		public void StartChange()
		{
			xenoTree.SetupHolder(selectedXenoHolder);
			SoundDefOf.Tick_Low.PlayOneShotOnCamera();
			Close(doCloseSound: false);
		}

	}

}
