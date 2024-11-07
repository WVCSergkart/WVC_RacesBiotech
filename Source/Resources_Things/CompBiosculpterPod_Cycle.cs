using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_XenosculpterPod : CompProperties_BiosculpterPod
	{

		public ThingDef inheritFromDef;

		public CompProperties_XenosculpterPod()
		{
			compClass = typeof(CompXenosculpterPod);
		}

		public override void ResolveReferences(ThingDef parentDef)
		{
			if (inheritFromDef != null)
			{
				parentDef.description = inheritFromDef.description;
				parentDef.statBases = inheritFromDef.statBases;
				parentDef.building = inheritFromDef.building;
				parentDef.thingCategories = inheritFromDef.thingCategories;
				parentDef.inspectorTabs = inheritFromDef.inspectorTabs;
				parentDef.placeWorkers = inheritFromDef.placeWorkers;
				foreach (CompProperties comp in inheritFromDef.comps)
				{
					if (comp is CompProperties_BiosculpterPod || comp is CompProperties_BiosculpterPod_BaseCycle)
					{
						continue;
					}
					if (!parentDef.comps.Contains(comp))
					{
						parentDef.comps.Add(comp);
					}
				}
				CompProperties_BiosculpterPod biosculpterPod = inheritFromDef?.GetCompProperties<CompProperties_BiosculpterPod>();
				if (biosculpterPod != null)
				{
					enterSound = biosculpterPod.enterSound;
					exitSound = biosculpterPod.exitSound;
					operatingEffecter = biosculpterPod.operatingEffecter;
					readyEffecter = biosculpterPod.readyEffecter;
					biotunedCycleSpeedFactor = biosculpterPod.biotunedCycleSpeedFactor;
					selectCycleColor = biosculpterPod.selectCycleColor;
				}
			}
		}

	}

	public class CompXenosculpterPod : CompBiosculpterPod
	{

		public SaveableXenotypeHolder xenotypeHolder;

		public List<Thing> ConnectedFacilities => parent.TryGetComp<CompAffectedByFacilities>()?.LinkedFacilitiesListForReading;

		public void SetupHolder(XenotypeHolder holder)
		{
			xenotypeHolder = new SaveableXenotypeHolder(holder);
		}

		public override void PostExposeData()
		{
			base.PostExposeData();
			//Scribe_Values.Look(ref additionalCycleDays, Props.uniqueTag + "_additionalCycleDays", 0);
			Scribe_Deep.Look(ref xenotypeHolder, "xenotypeHolder");
		}

		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			if (Occupant == null)
			{
				yield return new Command_Action
				{
					defaultLabel = "WVC_XaG_XenoTreeXenotypeChooseLabel".Translate(),
					defaultDesc = "WVC_XaG_XenotypeHolderCycleStarted_SelectXenotypeDesc".Translate(),
					icon = GetIcon(),
					action = delegate
					{
						Find.WindowStack.Add(new Dialog_BiosculpterPod(this));
					}
				};
			}
			yield return new Command_Toggle
			{
				defaultLabel = "BiosculpterAutoLoadNutritionLabel".Translate(),
				defaultDesc = "BiosculpterAutoLoadNutritionDescription".Translate(),
				icon = (autoLoadNutrition ? TexCommand.ForbidOff : TexCommand.ForbidOn),
				isActive = () => autoLoadNutrition,
				toggleAction = delegate
				{
					autoLoadNutrition = !autoLoadNutrition;
				}
			};
			foreach (Gizmo gizmo in base.CompGetGizmosExtra())
			{
				if (gizmo is Command_Toggle)
				{
					continue;
				}
				yield return gizmo;
			}
		}

		private Texture2D GetIcon()
		{
			if (xenotypeHolder != null)
			{
				if (xenotypeHolder.iconDef != null)
				{
					return xenotypeHolder.iconDef.Icon;
				}
				else
				{
					return xenotypeHolder.xenotypeDef.Icon;
				}
			}
			return XenotypeDefOf.Baseliner.Icon;
		}

		public List<GeneDef> GetGenes()
		{
			List<GeneDef> genes = new();
			if (ConnectedFacilities == null)
			{
				return genes;
			}
			foreach (Thing item in ConnectedFacilities)
			{
				CompGenepackContainer compGenepackContainer = item.TryGetComp<CompGenepackContainer>();
				if (compGenepackContainer == null)
				{
					continue;
				}
				bool flag = item.TryGetComp<CompPowerTrader>()?.PowerOn ?? true;
				if (!flag)
				{
					continue;
				}
				foreach (Genepack genepack in compGenepackContainer.ContainedGenepacks)
				{
					foreach (GeneDef geneDef in genepack.GeneSet.GenesListForReading)
					{
						if (genes.Contains(geneDef))
						{
							continue;
						}
						genes.Add(geneDef);
					}
				}
			}
			return genes;
		}

	}

	public class CompProperties_BiosculpterPod_XenogermCycle : CompProperties_BiosculpterPod_BaseCycle
	{

		public List<HediffDef> hediffsToRemove;

		public string uniqueTag = "XaG_Cycle";

	}

	public class CompBiosculpterPod_XenogermCycle : CompBiosculpterPod_Cycle
	{

		public new CompProperties_BiosculpterPod_XenogermCycle Props => (CompProperties_BiosculpterPod_XenogermCycle)props;

		public override void CycleCompleted(Pawn pawn)
		{
			HediffUtility.RemoveHediffsFromList(pawn, Props.hediffsToRemove);
			Messages.Message("WVC_XaG_XenogermCycleCompleted".Translate(pawn.Named("PAWN")), pawn, MessageTypeDefOf.PositiveEvent);
		}

	}

	public class CompBiosculpterPod_XenotypeNullifierCycle : CompBiosculpterPod_Cycle
	{

		public override void CycleCompleted(Pawn pawn)
		{
			ReimplanterUtility.SetXenotype(pawn, XenotypeDefOf.Baseliner);
			Messages.Message("WVC_XaG_XenogermCycleCompleted".Translate(pawn.Named("PAWN")), pawn, MessageTypeDefOf.PositiveEvent);
		}

	}

	public class CompBiosculpterPod_XenotypeHolderCycle : CompBiosculpterPod_Cycle
	{

		//public int additionalCycleDays = 0;

		public new CompProperties_BiosculpterPod_XenogermCycle Props => (CompProperties_BiosculpterPod_XenogermCycle)props;

		//public void StartCycle()
		//{
		//	Find.WindowStack.Add(new Dialog_BiosculpterPod(this));
		//}

		//public void ResetCycle()
		//{
		//	xenotypeHolder = null;
		//}

		public override void CycleCompleted(Pawn pawn)
		{
			if (Biosculptor.xenotypeHolder != null)
			{
				List<GeneDef> genes = XaG_GeneUtility.ConvertGenesInGeneDefs(pawn?.genes?.GenesListForReading);
				List<GeneDef> sculptorGenes = Biosculptor.GetGenes();
				if (!sculptorGenes.NullOrEmpty())
				{
					genes.AddRange(sculptorGenes);
				}
                float chance = GetChance(XaG_GeneUtility.GetMatchingGenesList(genes, Biosculptor.xenotypeHolder.genes).Count, Biosculptor.xenotypeHolder.genes.Count);
                if (Rand.Chance(chance))
				{
					ReimplanterUtility.SetXenotype(pawn, Biosculptor.xenotypeHolder);
				}
				else
				{
					ReimplanterUtility.SetBrokenXenotype(pawn, Biosculptor.xenotypeHolder, 1f - chance);
				}
			}
			else
			{
				ReimplanterUtility.SetXenotype(pawn, XenotypeDefOf.Baseliner);
			}
			Messages.Message("WVC_XaG_XenogermCycleCompleted".Translate(pawn.Named("PAWN")), pawn, MessageTypeDefOf.PositiveEvent);
			//ResetCycle();
		}

		public float GetChance(int matchingGenes, int totalGenes)
		{
			if (matchingGenes <= 0)
            {
				return 0f;
            }
			return matchingGenes / totalGenes;
		}

		private CompXenosculpterPod cachedBioscultor;

		public CompXenosculpterPod Biosculptor
        {
			get
			{
				if (cachedBioscultor == null)
				{
					cachedBioscultor = parent.TryGetComp<CompXenosculpterPod>();
				}
				return cachedBioscultor;
			}
        }

    }

}
