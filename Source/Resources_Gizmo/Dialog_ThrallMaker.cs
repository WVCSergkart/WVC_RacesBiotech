using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Dialog_ThrallMaker : Dialog_XenotypeHolderBasic
	{

		public Gene_ThrallMaker gene;

		protected override string Header => gene.LabelCap;

		public Dialog_ThrallMaker(Gene_ThrallMaker thisGene)
		{
			gene = thisGene;
			allXenotypes = thisGene.AllowedThralls;
			selectedXenoHolder = allXenotypes.RandomElement();
			UpdThrallHolders(allXenotypes);
			OnGenesChanged();
		}

		public override XenotypeHolder DefaultHolder()
		{
			return selectedXenoHolder;
		}

		public void UpdThrallHolders(List<XenotypeHolder> list)
		{
			foreach (XenotypeHolder item in list)
			{
				if (item is ThrallHolder thrallHolder && thrallHolder.thrallDef.reqGeneDef != null)
				{
					item.isOverriden = !XaG_GeneUtility.HasActiveGene(thrallHolder.thrallDef.reqGeneDef, gene.pawn);
				}
			}
		}

		protected override bool CanAccept()
		{
			if (selectedXenoHolder.isOverriden)
			{
				if (selectedXenoHolder is ThrallHolder holder)
				{
					Messages.Message("Requires".Translate() + ": " + holder.thrallDef.reqGeneDef.LabelCap, null, MessageTypeDefOf.RejectInput, historical: false);
				}
				return false;
			}
			return true;
		}

		protected override void Accept()
		{
			if (selectedXenoHolder is ThrallHolder holder)
			{
				gene.ThrallDef = holder.thrallDef;
			}
			Close(doCloseSound: true);
		}

	}

}
