using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_Xenotype : CompProperties
	{

		// public bool shouldResurrect = true;

		// public int recacheFrequency = 60;

		// public IntRange resurrectionDelay = new(6000, 9000);

		public string uniqueTag = "TrueXenotype";

		public CompProperties_Xenotype()
		{
			compClass = typeof(CompXenotype);
		}

		// public override void ResolveReferences(ThingDef parentDef)
		// {
			// if (shouldResurrect && parentDef.race?.corpseDef != null)
			// {
				// if (parentDef.race.corpseDef.GetCompProperties<CompProperties_UndeadCorpse>() != null)
				// {
					// return;
				// }
				// CompProperties_UndeadCorpse undead_comp = new();
				// undead_comp.resurrectionDelay = resurrectionDelay;
				// undead_comp.uniqueTag = uniqueTag;
				// parentDef.race.corpseDef.comps.Add(undead_comp);
			// }
		// }

	}

	public class CompXenotype : ThingComp
	{

		public CompProperties_Xenotype Props => (CompProperties_Xenotype)props;

		private bool xenotypeUpdated = false;

		// =================

		public override void CompTick()
		{
			Tick();
		}

		public override void CompTickRare()
		{
			Tick();
		}

		public override void CompTickLong()
		{
			Tick();
		}

		public void Tick()
		{
			if (xenotypeUpdated)
			{
				return;
			}
			if (parent is not Pawn pawn || !pawn.Spawned)
			{
				return;
			}
			if (TryUpdatedXenotype(pawn))
			{
				xenotypeUpdated = true;
			}
		}

		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				Command_Action command_Action = new()
				{
					defaultLabel = "DEV: TryUpdatedXenotype",
					action = delegate
					{
						TryUpdatedXenotype(parent as Pawn);
					}
				};
				yield return command_Action;
			}
		}

		public bool TryUpdatedXenotype(Pawn pawn)
		{
			if (pawn != null)
			{
				List<XenotypeDef> xenotypes = DefDatabase<XenotypeDef>.AllDefsListForReading;
				List<GeneDef> pawnGenes = XaG_GeneUtility.ConvertGenesInGeneDefs(pawn.genes.GenesListForReading);
				Dictionary<XenotypeDef, float> matchedXenotypes = new();
				foreach (XenotypeDef xenotypeDef in xenotypes)
				{
					if (xenotypeDef.genes.NullOrEmpty())
					{
						continue;
					}
					bool match = true;
					float matchingGenesCount = 0f;
					foreach (GeneDef geneDef in xenotypeDef.genes)
					{
						if (!pawnGenes.Contains(geneDef))
						{
							match = false;
							break;
						}
						matchingGenesCount++;
					}
					if (!match)
					{
						continue;
					}
					// float matchingGenesCount = XaG_GeneUtility.GetMatchingGenesList(pawn.genes.GenesListForReading, xenotypeDef.genes).Count;
					matchedXenotypes[xenotypeDef] = matchingGenesCount / xenotypeDef.genes.Count;
				}
				XenotypeDef resultXenotype = XenotypeDefOf.Baseliner;
				float currentMatchValue = 0f;
				if (!matchedXenotypes.NullOrEmpty())
				{
					foreach (var item in matchedXenotypes)
					{
						if (item.Value > currentMatchValue)
						{
							currentMatchValue = item.Value;
							resultXenotype = item.Key;
						}
					}
				}
				ReimplanterUtility.SetXenotypeDirect(null, pawn, resultXenotype);
				return true;
			}
			return false;
		}

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look(ref xenotypeUpdated, Props.uniqueTag + "_xenotypeUpdated", false);
		}

	}

}
