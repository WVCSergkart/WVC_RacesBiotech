using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class StyleGeneDef : StyleItemDef
	{

		public int uniqueStyleId = 0;

		public override Graphic GraphicFor(Pawn pawn, Color color)
		{
			if (noGraphic)
			{
				return null;
			}
			return GraphicDatabase.Get<Graphic_Multi>(texPath, overrideShaderTypeDef?.Shader ?? ShaderDatabase.CutoutHair, Vector2.one, color);
		}

	}

}
