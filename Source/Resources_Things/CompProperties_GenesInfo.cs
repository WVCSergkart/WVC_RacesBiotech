using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_Humanlike : CompProperties
	{

		public bool shouldResurrect = true;

		public int recacheFrequency = 11435;

		public IntRange resurrectionDelay = new(6000, 9000);

		public string uniqueTag = "XaG_Undead";

		//public Type golemGizmoType = typeof(GeneGizmo_Golems);

		public CompProperties_Humanlike()
		{
			compClass = typeof(CompHumanlike);
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

	public class CompHumanlike : ThingComp
	{

		public CompProperties_Humanlike Props => (CompProperties_Humanlike)props;

		// =================

		private List<IGeneInspectInfo> cachedInfoGenes = new();

		// public Pawn Pawn => parent as Pawn;

		public List<IGeneInspectInfo> InfoGenes
		{
			get
			{
				if (cachedInfoGenes.NullOrEmpty())
				{
					ResetInspectString();
					if (parent is Pawn pawn)
					{
						foreach (Gene gene in pawn.genes.GenesListForReading)
						{
							if (gene is IGeneInspectInfo geneInspectInfo && gene.Active)
							{
								cachedInfoGenes.Add(geneInspectInfo);
							}
						}
					}
				}
				return cachedInfoGenes;
			}
		}

		public void ResetInspectString()
		{
			cachedInfoGenes = new();
		}

		// private int nextRecache = -1;

		// public override void CompTick()
		// {
			// Log.Error("1");
		// }

		// public override void CompTickRare()
		// {
			// Log.Error("1");
		// }

		// public override void CompTickLong()
		// {
			// Log.Error("1");
		// }

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
			// if (Find.TickManager.TicksGame >= nextRecache)
			// {
				// cachedInfoGenes = new();
				// foreach (Gene gene in pawn.genes.GenesListForReading)
				// {
					// if (gene is IGeneInspectInfo geneInspectInfo && gene.Active)
					// {
						// cachedInfoGenes.Add(geneInspectInfo);
					// }
				// }
				// nextRecache = Find.TickManager.TicksGame + Props.recacheFrequency;
			// }
			string info = null;
			int count = 0;
			foreach (IGeneInspectInfo gene in InfoGenes)
			{
				string geneText = gene.GetInspectInfo;
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
