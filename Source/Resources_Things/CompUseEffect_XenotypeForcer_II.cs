using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{

	public class CompUseEffect_XenotypeForcer_II : CompUseEffect
	{
		public XenotypeDef xenotype = null;

		public CompProperties_UseEffect_XenotypeForcer_II Props => (CompProperties_UseEffect_XenotypeForcer_II)props;

		public override void PostPostMake()
		{
			base.PostPostMake();
			if (xenotype == null)
			{
				List<XenotypeDef> xenotypeDef = XenotypeFilterUtility.WhiteListedXenotypes(true);
				switch (Props.xenotypeType)
				{
					case CompProperties_UseEffect_XenotypeForcer_II.XenotypeType.Base:
						xenotype = xenotypeDef.Where((XenotypeDef randomXenotypeDef) => !SerumUtility.XenotypeHasArchites(randomXenotypeDef)).RandomElement();
						break;
					case CompProperties_UseEffect_XenotypeForcer_II.XenotypeType.Archite:
						xenotype = xenotypeDef.Where((XenotypeDef randomXenotypeDef) => SerumUtility.XenotypeHasArchites(randomXenotypeDef)).RandomElement();
						break;
						// case CompProperties_UseEffect_XenotypeForcer_II.XenotypeType.Hybrid:
						// xenotype = xenotypeDef.Where((XenotypeDef randomXenotypeDef) => SerumUtility.XenotypeHasArchites(randomXenotypeDef)).RandomElement();
						// break;
				}
				if (xenotype == null)
				{
					// We assign a random xenotype if there are no alternatives.
					Log.Error("Generated serum with null xenotype. Choose random.");
					xenotype = xenotypeDef.RandomElement();
				}
			}
			// descriptionHyperlinks = new List<DefHyperlink> { xenotype };
			// if (this.DescriptionHyperlinks != null)
			// {
			// this.DescriptionHyperlinks.Add(xenotype);
			// }
		}

		public override string TransformLabel(string label)
		{
			if (xenotype == null)
			{
				return parent.def.label + " (" + "ERR" + ")";
			}
			return parent.def.label + " (" + xenotype.label + ")";
		}

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Defs.Look(ref xenotype, "xenotypeDef");
		}

		public override void DoEffect(Pawn pawn)
		{
			// SerumUtility.HumanityCheck(pawn);
			if (SerumUtility.HumanityCheck(pawn))
			{
				return;
			}
			if (pawn.health.hediffSet.HasHediff(HediffDefOf.XenogermReplicating))
			{
				pawn.health.RemoveHediff(pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.XenogermReplicating));
				return;
			}
			if (xenotype == null)
			{
				// This is already a problem created by the player. Occurs if the player has blacklisted ALL xenotypes.
				Log.Error("Xenotype is still null. Do not report this to the developer, you yourself created this creepy world filled with bugs. To fix the situation, reset the filter in the " + "WVC_BiotechSettings".Translate() + " mod settings and restart the game.");
				// The following code will try a few more times to try to assign the xenotype, and if it doesn't work, it will spam everything with errors.
			}
			bool perfectCandidate = SerumUtility.HasCandidateGene(pawn);
			List<string> blackListedXenotypes = XenotypeFilterUtility.BlackListedXenotypesForSerums(false);
			SerumUtility.XenotypeSerum(pawn, blackListedXenotypes, xenotype, Props.removeEndogenes, Props.removeXenogenes);
			// switch (Props.xenotypeForcerType)
			// {
			// case CompProperties_UseEffect_XenotypeForcer_II.XenotypeForcerType.Base:
			// break;
			// case CompProperties_UseEffect_XenotypeForcer_II.XenotypeForcerType.Hybrid:
			// SerumUtility.HybridXenotypeSerum(pawn, blackListedXenotypes, xenotype);
			// break;
			// case CompProperties_UseEffect_XenotypeForcer_II.XenotypeForcerType.Custom:
			// SerumUtility.CustomXenotypeSerum(pawn, blackListedXenotypes);
			// break;
			// case CompProperties_UseEffect_XenotypeForcer_II.XenotypeForcerType.CustomHybrid:
			// SerumUtility.CustomHybridXenotypeSerum(pawn, blackListedXenotypes);
			// break;
			// }
			if (!perfectCandidate)
			{
				pawn.health.AddHediff(HediffDefOf.XenogerminationComa);
			}
			GeneUtility.UpdateXenogermReplication(pawn);
			if (PawnUtility.ShouldSendNotificationAbout(pawn))
			{
				int max = HediffDefOf.XenogerminationComa.CompProps<HediffCompProperties_Disappears>().disappearsAfterTicks.max;
				Find.LetterStack.ReceiveLetter("LetterLabelGenesImplanted".Translate(), "WVC_LetterTextGenesImplanted".Translate(pawn.Named("TARGET"), max.ToStringTicksToPeriod().Named("COMADURATION")), LetterDefOf.NeutralEvent, new LookTargets(pawn));
			}
		}

		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			if (Props.retuneJob == null)
			{
				yield break;
			}
			Command_Action command_Action = new()
			{
				defaultLabel = "WVC_XaG_SuremRetuneJob_Label".Translate(),
				defaultDesc = "WVC_XaG_SuremRetuneJob_Desc".Translate(),
				icon = parent.def.uiIcon,
				action = delegate
				{
					List<FloatMenuOption> list = new();
					List<Pawn> list2 = parent.MapHeld.mapPawns.SpawnedPawnsInFaction(Faction.OfPlayer);
					for (int i = 0; i < list2.Count; i++)
					{
						Pawn pawn = list2[i];
						if (pawn.IsColonistPlayerControlled && pawn.CanReach(parent, PathEndMode.ClosestTouch, Danger.Deadly) && !pawn.Downed && !pawn.Dead)
						{
							list.Add(new FloatMenuOption(pawn.LabelShort, delegate
							{
								pawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(Props.retuneJob, parent), JobTag.Misc);
							}, pawn, Color.white));
						}
					}
					if (list.Any())
					{
						Find.WindowStack.Add(new FloatMenu(list));
					}
					if (list.NullOrEmpty())
					{
						Messages.Message("WVC_XaG_GeneralTargetTooFar".Translate(), null, MessageTypeDefOf.RejectInput, historical: false);
					}
				}
			};
			if (!AllProjectsFinished(Props.researchPrerequisites, out ResearchProjectDef nonResearched))
			{
				command_Action.Disable("WVC_XaG_SuremRetuneJob_ResearchPrerequisites".Translate(nonResearched.LabelCap));
			}
			yield return command_Action;
		}

		public static bool AllProjectsFinished(List<ResearchProjectDef> researchProjects, out ResearchProjectDef nonResearched)
		{
			nonResearched = null;
			if (!researchProjects.NullOrEmpty())
			{
				for (int i = 0; i < researchProjects?.Count; i++)
				{
					if (researchProjects[i] == null || !researchProjects[i].IsFinished)
					{
						nonResearched = researchProjects[i];
						return false;
					}
				}
				return true;
			}
			return true;
		}

	}

}
