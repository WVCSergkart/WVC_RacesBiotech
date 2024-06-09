using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_UseEffect_GeneRestoration : CompProperties_UseEffect
	{

		public int daysDelay = 8;

		public bool canBeUsedInCaravan = false;

		// public List<ShapeshiftModeDef> unlockModes;

		public GeneDef geneDef;

		public HediffDef hediffDef;

		public List<HediffDef> hediffsToRemove;

		public bool disableShapeshiftComaAfterUse = false;

		public CompProperties_UseEffect_GeneRestoration()
		{
			compClass = typeof(CompUseEffect_GeneRestoration);
		}

		public override void ResolveReferences(ThingDef parentDef)
		{
			if (!hediffsToRemove.NullOrEmpty())
			{
				if (parentDef.descriptionHyperlinks.NullOrEmpty())
				{
					parentDef.descriptionHyperlinks = new();
				}
				foreach (HediffDef hediffDef in hediffsToRemove)
				{
					parentDef.descriptionHyperlinks.Add(new DefHyperlink(hediffDef));
				}
			}
		}

	}

}
