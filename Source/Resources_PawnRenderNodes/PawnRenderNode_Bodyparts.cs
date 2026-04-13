using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class PawnRenderNode_Bodyparts : PawnRenderNode
	{

		public PawnRenderNode_Bodyparts(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree)
			: base(pawn, props, tree)
		{
		}

		protected override string TexPathFor(Pawn pawn)
		{
			if (gene is Gene_Bodyparts styleGene && styleGene.StyleGeneDef != null)
			{
				foreach (BodyTypeGraphicData bodyTypeGraphicPath in styleGene.StyleGeneDef.bodyTypeGraphicPaths)
				{
					if (pawn.story.bodyType == bodyTypeGraphicPath.bodyType)
					{
						return bodyTypeGraphicPath.texturePath;
					}
				}
			}
			return base.TexPathFor(pawn);
		}


	}

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


	}

}
