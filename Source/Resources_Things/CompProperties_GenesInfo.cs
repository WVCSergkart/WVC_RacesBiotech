using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_Humanlike : CompProperties
	{

		public bool shouldResurrect = true;

		public int recacheFrequency = 60;

		public IntRange resurrectionDelay = new(6000, 9000);

		public string uniqueTag = "XaG_Undead";

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

		[Unsaved(false)]
		private List<IGeneInspectInfo> cachedInfoGenes;

		public List<IGeneInspectInfo> InfoGenes
		{
			get
			{
				if (cachedInfoGenes == null)
				{
					cachedInfoGenes = new();
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
			cachedInfoGenes = null;
			cachedFloatMenuOptionsGenes = null;
			// isBloodeater = null;
		}

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

		private string info = null;

		private int nextRecache = -1;

		public string Info(Pawn pawn)
		{
			nextRecache--;
			if (nextRecache <= 0)
			{
				if (pawn?.genes == null)
				{
					return null;
				}
				info = null;
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
				nextRecache = Props.recacheFrequency;
			}
			return info;
		}

		// =============

		[Unsaved(false)]
		private List<IGeneFloatMenuOptions> cachedFloatMenuOptionsGenes;

		public List<IGeneFloatMenuOptions> FloatMenuOptions
		{
			get
			{
				if (cachedFloatMenuOptionsGenes == null)
				{
					cachedFloatMenuOptionsGenes = new();
					if (parent is Pawn pawn)
					{
						foreach (Gene gene in pawn.genes.GenesListForReading)
						{
							if (gene is IGeneFloatMenuOptions geneInspectInfo && gene.Active)
							{
								cachedFloatMenuOptionsGenes.Add(geneInspectInfo);
							}
						}
					}
				}
				return cachedFloatMenuOptionsGenes;
			}
		}

		public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
		{
			foreach (IGeneFloatMenuOptions gene in FloatMenuOptions)
			{
				foreach (FloatMenuOption gizmo in gene.CompFloatMenuOptions(selPawn))
				{
					yield return gizmo;
				}
			}
		}

	}

}
