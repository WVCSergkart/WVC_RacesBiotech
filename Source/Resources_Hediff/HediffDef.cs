using RimWorld;
using System.Collections.Generic;
using System.Text;
using Verse;


namespace WVC_XenotypesAndGenes
{

	public class StatModifierList
	{
		public List<StatModifier> statOffsets;
		public List<StatModifier> statFactors;
	}

	public class XaG_HediffDef : HediffDef
	{

		public StatModifierList statModifiers;
		// public StatDef statDef;
		// public bool useFactorInsteadOffset = false;

	}


}
