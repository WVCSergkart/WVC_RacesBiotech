using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class PawnRenderNode_Faceplate : PawnRenderNode
	{

		public PawnRenderNode_Faceplate(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree)
			: base(pawn, props, tree)
		{
		}

		protected override string TexPathFor(Pawn pawn)
		{
			if (!props.texPaths.NullOrEmpty())
			{
				return props.texPaths.RandomElement();
			}
			if (!props.texPaths.NullOrEmpty())
			{
				using (new RandBlock(TexSeedFor(pawn)))
				{
					return props.texPaths.RandomElement();
				}
			}
			return null;
		}

	}


}
