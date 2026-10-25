using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_FleshlingCreator : XaG_Gene, IGeneRecacheable, IGeneOverriddenBy, IGeneNotifyLifeStageStarted
	{

		public override void PostAdd()
		{
			base.PostAdd();
			Recache();
		}

		private static HashSet<Gene_FleshlingCreator> cachedFleshlingGenes;
		public static HashSet<Gene_FleshlingCreator> MasterGenes
		{
			get
			{
				if (cachedFleshlingGenes == null)
				{
					HashSet<Gene_FleshlingCreator> list = new();
					foreach (Pawn pawn in PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_Colonists)
					{
						Gene_FleshlingCreator builder = pawn.genes?.GetFirstGeneOfType<Gene_FleshlingCreator>();
						if (builder != null)
						{
							list.Add(builder);
						}
					}
					cachedFleshlingGenes = list;
				}
				return cachedFleshlingGenes;
			}
		}

		public override void TickInterval(int delta)
		{
			if (pawn.IsHashIntervalTick(59676, delta))
			{
				DoAction();
			}
		}

		private void DoAction()
		{
			string phase = "init";
			try
			{
				phase = "check pawn";
				if (XaG_GeneUtility.SkipIf_FactionMap(pawn))
				{
					return;
				}
				phase = "count fleshlings";
				int mutants = 0;
				foreach (Pawn mutant in PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive)
				{
					if (mutant.mutant != null && mutant.Faction == pawn.Faction && mutant.mutant.Def == Extension_Giver.mutantDef)
					{
						mutants++;
					}
				}
				phase = "try create duplicate";
				if (mutants < (MasterGenes.Count * 3))
				{
					if (DuplicateUtility.TryDuplicatePawn(pawn, pawn.PositionHeld, out Pawn duplicate))
					{
						phase = "set age";
						AgelessUtility.SetAgeInYears(duplicate, 18);
						phase = "set mutant def";
						MutantUtility.SetPawnAsMutantInstantly(duplicate, Extension_Giver.mutantDef);
						phase = "final debug";
						MiscUtility.Notify_DebugPawn(duplicate);
						phase = "effects";
						EffectsUtility.MeatSplatter(pawn, FleshbeastUtility.MeatExplosionSize.Normal);
					}
				}
			}
			catch (Exception arg)
			{
				Log.Error($"Failed spawn mutant on phase {phase}. Reason: {arg.Message}");
			}
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: SpawnFleshling",
					action = delegate
					{
						DoAction();
					}
				};
			}
			yield return new Command_Action
			{
				defaultLabel = Extension_Giver.mainButtonDef.LabelCap,
				defaultDesc = Extension_Giver.mainButtonDef.description,
				icon = def.Icon,
				action = delegate
				{
					Extension_Giver.mainButtonDef.Worker.InterfaceTryActivate();
				}
			};
		}

		public void Notify_GenesRecache(Gene changedGene)
		{
			Recache();
		}

		private static void Recache()
		{
			cachedFleshlingGenes = null;
			HediffWithComps_FleshmassDuplicate.fleshlingsChecked = false;
		}

		public void Notify_OverriddenBy(Gene overriddenBy)
		{
			Recache();
		}

		public void Notify_Override()
		{
			Recache();
		}

		public override void PostRemove()
		{
			base.PostRemove();
			Recache();
		}

		public void Notify_LifeStageStarted()
		{
			Recache();
		}

		public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
		{
			Recache();
		}

	}

}
