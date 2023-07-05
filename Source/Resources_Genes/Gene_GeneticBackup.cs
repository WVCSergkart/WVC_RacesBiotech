using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	public class Gene_GeneticBackup : Gene
	{

		public List<GeneDef> xenoGenesBackup = new();
		public List<GeneDef> endoGenesBackup = new();
		public XenotypeIconDef iconDef = null;
		public string xenotypeName = null;
		public bool shouldDoBackup = true;

		public override void Tick()
		{
			base.Tick();
			// if (!pawn.IsHashIntervalTick(60000))
			if (!pawn.IsHashIntervalTick(1500))
			{
				return;
			}
			if (shouldDoBackup && Active)
			{
				List<Gene> list1 = pawn.genes.Endogenes;
				foreach (Gene item in list1)
				{
					endoGenesBackup.Add(item.def);
				}
				List<Gene> list2 = pawn.genes.Xenogenes;
				foreach (Gene item in list2)
				{
					xenoGenesBackup.Add(item.def);
				}
				iconDef = pawn.genes.iconDef;
				xenotypeName = pawn.genes.xenotypeName;
				Messages.Message("WVC_XaG_Gene_GeneticBackup_Full".Translate(pawn.LabelIndefinite().CapitalizeFirst()), pawn, MessageTypeDefOf.PositiveEvent);
				shouldDoBackup = false;
			}
			// if (Active)
			// {
				// foreach (Gene item in pawn.genes.Endogenes)
				// {
					// endoGenesBackup.Add(item.def);
				// }
				// foreach (Gene item in pawn.genes.Xenogenes)
				// {
					// xenoGenesBackup.Add(item.def);
				// }
				// iconDef = pawn.genes.iconDef;
				// xenotypeName = pawn.genes.xenotypeName;
				// Messages.Message("WVC_XaG_Gene_GeneticBackup_Full".Translate(pawn.LabelIndefinite().CapitalizeFirst()), pawn, MessageTypeDefOf.PositiveEvent);
			// }
		}

		public override void PostRemove()
		{
			base.PostRemove();
			// Does not work because it is executed almost immediately, which leads to a crash
			if (!shouldDoBackup)
			{
				Pawn_GeneTracker genes = pawn.genes;
				for (int numXenogenes = genes.Xenogenes.Count - 1; numXenogenes >= 0; numXenogenes--)
				{
					pawn.genes?.RemoveGene(genes.Xenogenes[numXenogenes]);
				}
				for (int numEndogenes = genes.Endogenes.Count - 1; numEndogenes >= 0; numEndogenes--)
				{
					pawn.genes?.RemoveGene(genes.Endogenes[numEndogenes]);
				}
				for (int i = 0; i < endoGenesBackup.Count; i++)
				{
					pawn.genes?.AddGene(endoGenesBackup[i], false);
				}
				for (int i = 0; i < xenoGenesBackup.Count; i++)
				{
					pawn.genes?.AddGene(xenoGenesBackup[i], true);
				}
				pawn.genes.xenotypeName = xenotypeName;
				pawn.genes.iconDef = iconDef;
				Messages.Message("WVC_XaG_Gene_GeneticBackup_Empty".Translate(pawn.LabelIndefinite().CapitalizeFirst()), pawn, MessageTypeDefOf.PositiveEvent);
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			// Scribe_Collections.Look(ref endoGenesBackup, "currentPawnEndogenesBackup", LookMode.Def);
			// Scribe_Collections.Look(ref xenoGenesBackup, "currentPawnXenogenesBackup", LookMode.Def);
			Scribe_Defs.Look(ref iconDef, "pawnXenotypeIconBackup");
			Scribe_Values.Look(ref xenotypeName, "pawnXenotypeNameBackup");
			Scribe_Values.Look(ref shouldDoBackup, "shouldDoBackup", defaultValue: true);
		}
	}

}
