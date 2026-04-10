using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class StyleGeneDef : StyleItemDef
	{

		public List<BodyTypeGraphicData> bodyTypeGraphicPaths;

		public int uniqueStyleId = 0;

		[NoTranslate]
		public string texPathFemale;

		//public bool isGeneralOption = false;
		//public bool useIcon;

		public List<GeneDef> geneDefs;

		public override Texture2D Icon
		{
			get
			{
				if (geneDefs != null)
				{
					return ContentFinder<Texture2D>.Get(iconPath);
				}
				return base.Icon;
			}
		}

		public string TexPathByGender(Gender gender)
		{
			if (!texPathFemale.NullOrEmpty() && gender == Gender.Female)
			{
				return texPathFemale;
			}
			return texPath;
		}

		// Dummy
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
