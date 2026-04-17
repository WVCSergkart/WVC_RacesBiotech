using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class PawnRenderNode_Attachments : PawnRenderNode
	{

		public PawnRenderNode_Attachments(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree)
			: base(pawn, props, tree)
		{
		}

		protected override string TexPathFor(Pawn pawn)
		{
			if (gene is IGeneCustomGraphic styleGene && styleGene.StyleGeneDef != null)
			{
				return styleGene.StyleGeneDef.TexPathByGender(pawn.gender);
			}
			return base.TexPathFor(pawn);
		}

		public override Color ColorFor(Pawn pawn)
		{
			if (gene is IGeneCustomGraphic styleGene && styleGene.CurrentColor != Color.white)
			{
				return styleGene.CurrentColor;
			}
			return base.ColorFor(pawn);
		}

	}

	public class PawnRenderNode_AttachmentsHead : Verse.PawnRenderNode_AttachmentHead
	{

		public PawnRenderNode_AttachmentsHead(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree)
			: base(pawn, props, tree)
		{
		}

		protected override string TexPathFor(Pawn pawn)
		{
			if (gene is IGeneCustomGraphic styleGene && styleGene.StyleGeneDef != null)
			{
				return styleGene.StyleGeneDef.TexPathByGender(pawn.gender);
			}
			return base.TexPathFor(pawn);
		}

		public override Color ColorFor(Pawn pawn)
		{
			if (gene is IGeneCustomGraphic styleGene && styleGene.CurrentColor != Color.white)
			{
				return styleGene.CurrentColor;
			}
			return base.ColorFor(pawn);
		}

	}

}
