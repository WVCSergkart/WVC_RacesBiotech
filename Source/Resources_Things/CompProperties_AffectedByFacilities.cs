using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_AffectedByGenebanks : CompProperties_AffectedByFacilities
	{

		public bool autoLink = false;

		public CompProperties_AffectedByGenebanks()
		{
			compClass = typeof(CompAffectedByFacilities);
		}

		public override void ResolveReferences(ThingDef parentDef)
		{
			if (!autoLink)
			{
				return;
			}
			if (linkableFacilities.NullOrEmpty())
			{
				linkableFacilities = new();
			}
			List<ThingDef> thingDefs = DefDatabase<ThingDef>.AllDefsListForReading.Where((ThingDef thingDef) => !linkableFacilities.Contains(thingDef) && IsGenebank(thingDef)).ToList();
			if (thingDefs.NullOrEmpty())
			{
				return;
			}
			foreach (ThingDef thingDef in thingDefs)
			{
				CompProperties_Facility compProperties = thingDef.GetCompProperties<CompProperties_Facility>();
				if (compProperties == null)
				{
					continue;
				}
				if (compProperties.linkableBuildings.NullOrEmpty())
				{
					compProperties.linkableBuildings = new();
				}
				compProperties.linkableBuildings.Add(parentDef);
				linkableFacilities.Add(thingDef);
			}
		}

		public static bool IsGenebank(ThingDef thingDef)
		{
			if (thingDef.comps.NullOrEmpty())
			{
				return false;
			}
			foreach (CompProperties comp in thingDef.comps)
			{
				if (comp is CompProperties_GenepackContainer)
				{
					return true;
				}
			}
			return false;
		}

	}

}
