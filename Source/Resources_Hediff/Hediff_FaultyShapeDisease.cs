using RimWorld;
using System;
using Verse;

namespace WVC_XenotypesAndGenes
{
	//public class Hediff_FaultyShapeDisease : Hediff
	//{

	//	public override bool Visible => true;

	//	public override bool ShouldRemove => !pawn.IsShapeshifter() || ScenPart_PawnModifier_Shapeshifter.Anomaly_ScenarioCompleted;

	//	public HediffStage curStage;
	//	public override HediffStage CurStage
	//	{
	//		get
	//		{
	//			if (curStage == null)
	//			{
	//				curStage = new();
	//			}
	//			return curStage;
	//		}
	//	}

	//	public override void PostTickInterval(int delta)
	//	{
	//		base.PostTickInterval(delta);
	//		if (pawn.IsHashIntervalTick(60000, delta))
	//		{
	//			FaultyShape();
	//		}
	//	}

	//	private void FaultyShape()
	//	{
	//		try
	//		{
	//			pawn.genes?.GetFirstGeneOfType<Gene_Shapeshifter>()?.TryOffsetResource(5);
	//			int anomalyLevel = (ModsConfig.AnomalyActive ? Find.Anomaly.Level : 0);
	//			if (pawn.genes.Endogenes.Count > 35 + anomalyLevel)
	//			{
	//				Gene gene = pawn.genes.Endogenes.RandomElement();
	//				pawn.genes.RemoveGene(gene);
	//				Message(pawn, gene);
	//				return;
	//			}
	//			if (pawn.genes.Xenogenes.Count > XenotypeDefOf.Sanguophage.genes.Count + 3 + anomalyLevel)
	//			{
	//				Gene gene = pawn.genes.Xenogenes.RandomElement();
	//				pawn.genes.RemoveGene(gene);
	//				Message(pawn, gene);
	//				return;
	//			}
	//		}
	//		catch (Exception arg)
	//		{
	//			Log.Error("Failed remove shapeshifter random gene. Reason: " + arg.Message);
	//		}

	//		static void Message(Pawn pawn, Gene gene)
	//		{
	//			if (PawnUtility.ShouldSendNotificationAbout(pawn))
	//			{
	//				Messages.Message("WVC_XaG_FaultyShapeDisease".Translate(pawn, gene.Label), pawn, MessageTypeDefOf.NegativeEvent);
	//			}
	//		}
	//	}

	//}
}
