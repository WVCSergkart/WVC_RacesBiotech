using System.Collections.Generic;
using Verse;


namespace WVC_XenotypesAndGenes
{
    public class ColorGeneTemplateDef : Def
	{

		public int displayOrderOffset = 1;

		[NoTranslate]
		public string iconPath;

		public GeneCategoryDef displayCategory;

		public float displayOrderInCategory;

		public bool randomChosen = true;

		public int biostatCpx = 0;

		public int biostatMet = 0;

		public int biostatArc = 0;

		public bool skinColor = true;

		public List<string> exclusionTags;

		public float selectionWeight = 0.0001f;

		public bool canGenerateInGeneSet = true;

	}

}
