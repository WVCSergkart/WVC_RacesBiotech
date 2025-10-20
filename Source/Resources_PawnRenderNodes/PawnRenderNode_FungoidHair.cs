using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class PawnRenderNode_FungoidHair : PawnRenderNode
	{

		public PawnRenderNode_FungoidHair(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree)
			: base(pawn, props, tree)
		{
		}

        public override Color ColorFor(Pawn pawn)
        {
			if (gene is Gene_FungoidHair geneHair && geneHair.color != null)
            {
				return geneHair.color;
            }
            return base.ColorFor(pawn);
		}

	}

}
