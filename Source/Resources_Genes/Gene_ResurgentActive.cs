using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	public class Gene_ResurgentDependent : Gene
	{

		public override bool Active
		{
			get
			{
				if (base.Active)
				{
					if (pawn?.genes != null)
					{
						return ResurgentCells(pawn, def);
					}
				}
				return base.Active;
			}
		}

		public bool ResurgentCells(Pawn pawn, GeneDef def)
		{
			Gene_ResurgentCells gene_Resurgent = pawn.genes?.GetFirstGeneOfType<Gene_ResurgentCells>();
			if (gene_Resurgent != null)
			{
				if (gene_Resurgent.Value >= gene_Resurgent.MinLevelForAlert)
				{
					return true;
				}
			}
			return false;
		}

	}

	public class Gene_ResurgentActive : Gene
	{

		public override bool Active
		{
			get
			{
				if (base.Active)
				{
					if (pawn?.genes != null)
					{
						return ResurgentCells(pawn, def);
					}
				}
				return false;
			}
		}

		public bool ResurgentCells(Pawn pawn, GeneDef def)
		{
			Gene_ResurgentCells gene_Resurgent = pawn.genes?.GetFirstGeneOfType<Gene_ResurgentCells>();
			if (gene_Resurgent != null)
			{
				if (gene_Resurgent.Value >= def.resourceLossPerDay)
				{
					return true;
				}
			}
			return false;
		}

	}
}
