using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Duplicator : Gene, IGeneNotifyGenesChanged
	{

		private GeneExtension_Giver cachedGeneExtension;
		public GeneExtension_Giver Giver
		{
			get
			{
				if (cachedGeneExtension == null)
				{
					cachedGeneExtension = def.GetModExtension<GeneExtension_Giver>();
				}
				return cachedGeneExtension;
			}
		}

		//public override string LabelCap => base.LabelCap + " (" + "WVC_IsDuplcate".Translate((SourcePawn != pawn).ToStringYesNo()) + ")";

		private Pawn cachedSourcePawn;
		public Pawn SourcePawn
		{
			get
			{
				if (cachedSourcePawn == null)
				{
					cachedSourcePawn = DuplicateUtility.GetSourceCyclic(pawn);
				}
				return cachedSourcePawn;
			}
		}

		public bool SourceIsAlive => SourcePawn?.Dead == false;

		private List<Pawn> cachedDuplicates;
		public List<Pawn> PawnDuplicates
		{
			get
			{
				if (cachedDuplicates == null)
				{
					cachedDuplicates = PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive.Where((target) => target != SourcePawn && DuplicateUtility.GetSourceCyclic(target) == SourcePawn).ToList();
				}
				return cachedDuplicates;
			}
		}

		private List<Pawn> cachedDuplicatesWithSource;
		public List<Pawn> PawnDuplicates_WithSource
		{
			get
			{
				if (cachedDuplicatesWithSource == null)
				{
					List<Pawn> list = PawnDuplicates.ToList();
					list.Add(SourcePawn);
					cachedDuplicatesWithSource = list;
				}
				return cachedDuplicatesWithSource;
			}
		}

		public List<GeneralHolder> PossibleGenesOutcome => Giver?.geneDefWithChances;
		public bool IsXenogene => pawn.genes.Xenogenes.Contains(this);

		public bool TryAddNewSubGene(bool forDuplicates)
		{
			IEnumerable<GeneralHolder> enumerable = PossibleGenesOutcome?.Where((geneWith) => (!geneWith.forDuplicates || forDuplicates) && geneWith.reqDupesCount <= PawnDuplicates.Count && !XaG_GeneUtility.ConflictWith(geneWith.geneDef, pawn.genes.GenesListForReading));
			if (!enumerable.EnumerableNullOrEmpty() && enumerable.TryRandomElementByWeight((geneWith) => geneWith.chance, out GeneralHolder result))
			{
				pawn.genes.AddGene(result.geneDef, IsXenogene);
				Messages.Message("WVC_XaG_GeneDuplicator_AddNewSubGene".Translate(pawn.NameShortColored, result.geneDef.label), pawn, MessageTypeDefOf.NeutralEvent, historical: false);
				return true;
			}
			return false;
		}

		public override void PostAdd()
		{
			base.PostAdd();
			if (MiscUtility.GameStarted() && SourcePawn != pawn)
			{
				SourcePawn.genes?.GetFirstGeneOfType<Gene_Duplicator>()?.Notify_GenesChanged(null);
			}
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: TryDuplicate",
					action = delegate
					{
						TryDuplicate();
					}
				};
			}
		}

		public void Notify_DuplicateCreated(Pawn newDupe)
		{
			foreach (Gene gene in pawn.genes.GenesListForReading)
			{
				if (gene is Gene_Duplicator_MeSource dependant && dependant.Active)
				{
					dependant.Notify_DuplicateCreated(newDupe);
				}
			}
		}

		public StatDef StatDef => Giver.statDef;

		public void Notify_GenesChanged(Gene changedGene)
		{
			//Log.Error(pawn.NameFullColored.ToString());
			cachedDuplicates = null;
			cachedDuplicatesWithSource = null;
		}

		//public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
		//{
		//	if (!Active)
		//	{
		//		return;
		//	}
		//	Notify_GenesChanged(null);
		//	foreach (Pawn dupe in PawnDuplicates_WithSource)
		//	{
		//		if (!dupe.Dead)
		//		{
		//			dupe.genes.GetFirstGeneOfType<Gene_Duplicator>();
		//		}
		//	}
		//}

		//public void Notify_DuplicateDied()
		//{
		//	foreach (Gene gene in pawn.genes.GenesListForReading)
		//	{

		//	}
		//}


		public bool? CanDuplicate()
		{
			return pawn.abilities?.GetAbility(def.abilities.First())?.OnCooldown;
		}

		public void TryDuplicate()
		{
			pawn.abilities?.GetAbility(def.abilities.First())?.Activate(pawn, pawn);
		}

		public void ResetAbility()
		{
			pawn.abilities?.GetAbility(def.abilities.First())?.ResetCooldown();
		}

		public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
		{
			StatDef stat = StatDef;
			if (stat != null)
			{
				yield return new StatDrawEntry(StatCategoryDefOf.Genetics, stat.LabelCap, pawn.GetStatValue(stat).ToStringByStyle(stat.toStringStyle), stat.description, stat.displayPriorityInCategory);
				//yield return new StatDrawEntry(StatCategoryDefOf.Genetics, stat);
			}
			yield return new StatDrawEntry(StatCategoryDefOf.Genetics, "WVC_IsDuplcate".Translate(), (SourcePawn != pawn || pawn.IsDuplicate).ToStringYesNo(), "WVC_IsDuplcateDesc".Translate(), 500);
		}

	}

}
