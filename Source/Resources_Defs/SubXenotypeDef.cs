using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using Verse;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static UnityEngine.GridBrushBase;

namespace WVC_XenotypesAndGenes
{

	/// <summary>
	/// This isn't a true sub-xenotype in the traditional sense. 
	/// It's used by shapeshifter genes for alternative (XML-based) xenotype variations (XML-Custom_Xenotypes).
	/// This is necessary to avoid creating an entire xenotype based on deviations of one or two genes.
	/// It can be used in other genes as well, as it's called through the universal GetAllXenotypesHolders().
	/// Perhaps the morpher uses it too.
	/// </summary>
	public class SubXenotypeDef : Def
	{

		public bool IsNull => iconDef == null || genes.NullOrEmpty();

		//public XenotypeDef xenotypeDef;
		public XenotypeIconDef iconDef;
		public float displayPriority = 0;
		public bool inheritable = true;
		public List<GeneDef> genes;

	}

}
