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

		public JobDef bloodeaterFeedingJobDef;

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

		// public Pawn Pawn => parent as Pawn;

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
			isBloodeater = null;
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

		public bool? isBloodeater;

		public static readonly CachedTexture FeedTex = new("WVC/UI/Genes/Gene_Bloodeater_v0");

		public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
		{
			if (Bloodeater())
			{
				if (parent is not Pawn pawn || !pawn.Downed)
				{
					yield break;
				}
				if (!GeneFeaturesUtility.CanBloodFeedNowWith(pawn, selPawn))
				{
					yield return new FloatMenuOption("WVC_NotEnoughBlood".Translate(), null);
					yield break;
				}
				yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("WVC_FeedWithBlood".Translate() + " " + pawn.LabelShort, delegate
				{
					Job job = JobMaker.MakeJob(Props.bloodeaterFeedingJobDef, parent);
					selPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
				}), selPawn, pawn);
			}
		}

		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			if (Bloodeater())
			{
				if (parent is not Pawn pawn || !pawn.Downed)
				{
					yield break;
				}
				yield return new Command_Action
				{
					defaultLabel = "WVC_FeedDownedBloodeaterForced".Translate(),
					defaultDesc = "WVC_FeedDownedBloodeaterForcedDesc".Translate(),
					icon = FeedTex.Texture,
					action = delegate
					{
						List<FloatMenuOption> list = new();
						List<Pawn> list2 = pawn.MapHeld.mapPawns.SpawnedPawnsInFaction(Faction.OfPlayer);
						for (int i = 0; i < list2.Count; i++)
						{
							Pawn absorber = list2[i];
							if (absorber.genes != null 
								&& GeneFeaturesUtility.CanBloodFeedNowWith(pawn, absorber))
							{
								list.Add(new FloatMenuOption(absorber.LabelShort, delegate
								{
									Job job = JobMaker.MakeJob(Props.bloodeaterFeedingJobDef, pawn);
									absorber.jobs.TryTakeOrderedJob(job, JobTag.Misc, false);
								}, absorber, Color.white));
							}
						}
						if (list.Any())
						{
							Find.WindowStack.Add(new FloatMenu(list));
						}
					}
				};
			}
		}

		public bool Bloodeater()
		{
			if (!isBloodeater.HasValue)
			{
				isBloodeater = UndeadUtility.IsBloodeater(parent as Pawn);
				// Log.Error("Pawn is bloodeater: " + );
			}
			return isBloodeater.Value;
		}

	}

}
