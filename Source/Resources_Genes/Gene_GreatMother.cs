using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_GreatMother : XaG_Gene, IGeneRecacheable, IGenePregnantHuman, IGeneOverriddenBy
	{

		public void Notify_GenesRecache(Gene changedGene)
		{
			ResetTick();
		}

		public virtual void ResetTick(int newTick = 60)
		{
			tick = newTick;
		}

		private int tick = 60;
		public override void TickInterval(int delta)
		{
			if (tick > 0)
			{
				tick--;
				if (tick <= 0)
				{
					ChangeGeneSet();
				}
			}
		}

		protected virtual void ChangeGeneSet()
		{
			if (def is not GeneDef_XaG geneDef_XaG || geneDef_XaG.genesTrans == null)
			{
				return;
			}
			GeneSet geneSet = GeneSetHelper() ?? pawn.health?.hediffSet?.GetFirstHediff<Hediff_Pregnant>()?.geneSet;
			if (geneSet != null)
			{
				UpdGeneSet(geneDef_XaG, geneSet);
			}
		}

		/// <summary>
		/// Mod support helper
		/// The gene is not intended for use with non-vanilla pregnancies, but there is the possibility of adding support.
		/// </summary>
		/// <returns></returns>
		private GeneSet GeneSetHelper()
		{
			return pawn.health?.hediffSet?.GetFirstHediff<Hediff_BudPregnancy>()?.GeneSet;
		}

		protected void UpdGeneSet(GeneDef_XaG geneDef_XaG, GeneSet geneSet)
		{
			foreach (GeneralHolder generalHolder in geneDef_XaG.genesTrans)
			{
				if (!XaG_GeneUtility.HasEndogene(generalHolder.geneDef, pawn))
				{
					continue;
				}
				foreach (GeneDef holderGeneDef in generalHolder.genes)
				{
					foreach (GeneDef babyGeneDef in geneSet.GenesListForReading.ToList())
					{
						if (babyGeneDef.ConflictsWith(holderGeneDef))
						{
							geneSet.Debug_RemoveGene(babyGeneDef);
						}
						geneSet.AddGene(holderGeneDef);
					}
				}
			}
			geneSet.Debug_RemoveGene(def);
			geneSet.SortGenes();
		}

		public void Notify_PregnancyStarted(Hediff_Pregnant pregnancy)
		{
			ResetTick(160);
		}

		public bool Notify_CustomPregnancy(Hediff_Pregnant pregnancy)
		{
			return false;
		}

		public void Notify_OverriddenBy(Gene overriddenBy)
		{
			ResetTick();
		}

		public void Notify_Override()
		{
			ResetTick();
		}

	}

}
