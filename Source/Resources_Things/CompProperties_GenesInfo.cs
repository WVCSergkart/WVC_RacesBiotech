using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_GenesDisplayInfo : CompProperties
	{

		public int recacheFrequency = 11435;

		//public Type golemGizmoType = typeof(GeneGizmo_Golems);

		public CompProperties_GenesDisplayInfo()
		{
			compClass = typeof(CompDisplayInfoFromGenes);
		}

	}

	public class CompDisplayInfoFromGenes : ThingComp
	{

		public CompProperties_GenesDisplayInfo Props => (CompProperties_GenesDisplayInfo)props;

		// =================

		public List<Gene_InspectInfo> cachedInfoGenes = new();

		private int nextRecache = -1;

		// =================

		public override string CompInspectStringExtra()
		{
			if (parent.Faction != Faction.OfPlayer)
			{
				return null;
			}
			if (parent is Pawn pawn)
			{
				return Info(pawn);
			}
			return null;
		}

		public string Info(Pawn pawn)
		{
			if (pawn?.genes == null)
			{
				return null;
			}
			if (Find.TickManager.TicksGame >= nextRecache)
			{
				cachedInfoGenes = new();
				foreach (Gene gene in pawn.genes.GenesListForReading)
				{
					if (gene is Gene_InspectInfo geneInspectInfo && geneInspectInfo.Active)
					{
						cachedInfoGenes.Add(geneInspectInfo);
					}
				}
				nextRecache = Find.TickManager.TicksGame + Props.recacheFrequency;
			}
			string info = null;
			int count = 0;
			foreach (Gene_InspectInfo gene in cachedInfoGenes)
			{
				string geneText = gene.GetInspectInfo();
				if (geneText.NullOrEmpty())
				{
					continue;
				}
				if (count > 0)
				{
					info += "\n";
				}
				info += geneText;
				count++;
			}
			return info;
		}

	}

}
