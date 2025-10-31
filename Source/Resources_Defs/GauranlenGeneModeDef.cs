using System.Collections.Generic;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class GauranlenGeneModeDef : Def
	{

		public GauranlenTreeModeDef useDescriptionFromDef = null;

		public GauranlenGeneModeDef previousStage;

		public PawnKindDef pawnKindDef;

		public ThingDef newDryadDef;

		public List<MemeDef> requiredMemes;

		public List<GeneDef> requiredGenes;

		public List<StatDef> displayedStats;

		public List<DefHyperlink> hyperlinks = new();

		private string cachedDescription;

		public string Description
		{
			get
			{
				if (cachedDescription == null)
				{
					cachedDescription = description;
					CompProperties_Spawner compProperties_Spawner = pawnKindDef?.race.GetCompProperties<CompProperties_Spawner>();
					if (compProperties_Spawner != null)
					{
						cachedDescription = cachedDescription + "\n\n" + "DryadProducesResourcesDesc".Translate(NamedArgumentUtility.Named(pawnKindDef, "DRYAD"), GenLabel.ThingLabel(compProperties_Spawner.thingToSpawn, null, compProperties_Spawner.spawnCount).Named("RESOURCES"), compProperties_Spawner.spawnIntervalRange.max.ToStringTicksToPeriod().Named("DURATION")).Resolve().CapitalizeFirst();
					}
				}
				return cachedDescription;
			}
		}

		public override void ResolveReferences()
		{
			base.ResolveReferences();
			if (useDescriptionFromDef != null)
			{
				description = useDescriptionFromDef.description;
			}
			hyperlinks.Add(new DefHyperlink(pawnKindDef.race));
			CompProperties_Spawner compProperties_Spawner = pawnKindDef?.race?.GetCompProperties<CompProperties_Spawner>();
			if (compProperties_Spawner != null)
			{
				hyperlinks.Add(new DefHyperlink(compProperties_Spawner.thingToSpawn));
			}
		}

	}

}
