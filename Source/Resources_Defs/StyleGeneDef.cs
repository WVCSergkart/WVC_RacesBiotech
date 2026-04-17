using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class StyleGeneDef : StyleItemDef
	{

		public List<BodyTypeGraphicData> bodyTypeGraphicPaths;

		public int uniqueStyleId = 0;

		public List<int> allowedStyles;

		public float selectionWeight = 1f;

		[NoTranslate]
		public string texPathFemale;

		//public bool isGeneralOption = false;
		//public bool useIcon;

		public List<string> geneDefs;

		public List<GeneDef> GeneDefs
		{
			get
			{
				if (xenotypeDef == null)
				{
					return new();
				}
				List<GeneDef> list = new();
				foreach (GeneDef gene in xenotypeDef.genes)
				{
					if (geneDefs.Contains(gene.defName))
					{
						list.Add(gene);
					}
				}
				return list;
			}
		}

		public XenotypeDef xenotypeDef;

		public bool AllowedForStyle(int styleId)
		{
			if (uniqueStyleId == styleId)
			{
				return true;
			}
			if (allowedStyles != null)
			{
				return allowedStyles.Contains(styleId);
			}
			return false;
		}

		public static StyleGeneDef GetRandomDefForStyleGene(IGeneCustomGraphic gene)
		{
			if (DefDatabase<StyleGeneDef>.AllDefsListForReading.Where(style => style.selectionWeight > 0f && style.AllowedForStyle(gene.StyleId)).TryRandomElementByWeight(style => style.selectionWeight, out StyleGeneDef styleGeneDef))
			{
				return styleGeneDef;
			}
			return null;
		}

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
