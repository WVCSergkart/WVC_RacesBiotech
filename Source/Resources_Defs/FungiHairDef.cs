using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class StyleGeneDef : StyleItemDef
	{

		public override Graphic GraphicFor(Pawn pawn, Color color)
		{
			return null;
		}

	}

	// Dummy def for styling
	public class FungiHairDef : StyleGeneDef
	{

		//public int textId = 0;

		public override Graphic GraphicFor(Pawn pawn, Color color)
		{
			if (noGraphic)
			{
				return null;
			}
			return GraphicDatabase.Get<Graphic_Multi>(texPath, overrideShaderTypeDef?.Shader ?? ShaderDatabase.CutoutHair, Vector2.one, color);
		}

	}

	////Dev
	//public class EyesDef : StyleGeneDef
	//{

	//}

	////Dev
	//public class HoloeyesDef : StyleGeneDef
	//{

	//}

}
