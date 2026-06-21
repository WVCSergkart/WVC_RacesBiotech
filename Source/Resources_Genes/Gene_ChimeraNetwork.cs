using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_ChimeraNetwork : Gene_ChimeraDependant, IGeneCustomChimeraEater
	{

		//public GeneExtension_Undead Undead => def?.GetModExtension<GeneExtension_Undead>();

		public string ChimeraEater_Name => "WVC_Share";
		public TaggedString ChimeraEater_Desc => "WVC_XaG_GeneChimeraNetwork_Desc";

		public void ChimeraEater(ref List<GeneDef> selectedGenes)
		{
			foreach (GeneDef geneDef in selectedGenes)
			{
				Chimera.RemoveCollectedGene(geneDef);
				try
				{
					if (Rand.Chance(0.12f))
					{
						Gene_Chimera.TryGetUniqueGene(Chimera, pawn, Extension_Undead.chimeraConditionalGenes);
					}
				}
				catch (Exception arg)
				{
					Log.Error("Failed obtaine gene. Reason: " + arg);
				}
			}
		}

		//public override bool BlockChimeraEat => true;

		private static int lastShareTick;

		public override void TickInterval(int delta)
		{
			//base.TickInterval(delta);
			if (!pawn.IsHashIntervalTick(69167, delta))
			{
				return;
			}
			if (pawn.Faction != Faction.OfPlayer)
			{
				return;
			}
			if (lastShareTick > Find.TickManager.TicksGame)
			{
				return;
			}
			ShareGenes();
		}

		public void ShareGenes()
		{
			IEnumerable<Pawn> pawns = PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_Colonists.Where((target) => target != pawn && target.genes != null);
			if (!pawns.Any())
			{
				return;
			}
			lastShareTick = Find.TickManager.TicksGame + 30000;
			List<GeneDef> collectedGenes = Chimera.CollectedGenes;
			foreach (Pawn item in pawns)
			{
				Gene_ChimeraNetwork network = item.genes.GetFirstGeneOfType<Gene_ChimeraNetwork>();
				if (network == null)
				{
					continue;
				}
				if (Chimera.TryGetGene(network.Chimera.CollectedGenes, out GeneDef result))
				{
					Messages.Message("WVC_XaG_GeneGeneticThief_GeneCopied".Translate(pawn.NameShortColored, result.label), pawn, MessageTypeDefOf.NeutralEvent, historical: false);
				}
				else if (network.Chimera.TryGetGene(collectedGenes, out GeneDef result2))
				{
					Messages.Message("WVC_XaG_GeneGeneticThief_GeneCopied".Translate(item.NameShortColored, result2.label), item, MessageTypeDefOf.NeutralEvent, historical: false);
				}
			}
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: ShareGenes",
					action = delegate
					{
						ShareGenes();
					}
				};
			}
		}
	}

}
