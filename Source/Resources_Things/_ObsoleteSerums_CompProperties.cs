using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{

	[Obsolete]
	public class CompProperties_UseEffect_XenotypeForcer_II : CompProperties_UseEffect
	{

		public XenotypeDef endotypeDef = null;
		public XenotypeDef xenotypeDef = null;

		public List<GeneDef> possibleGenes;

		public List<XenotypeDef> possibleXenotypes;

		public XenotypeType xenotypeType = (XenotypeType)0;

		public bool removeEndogenes = false;

		public bool removeXenogenes = true;

		public JobDef retuneJob;

		[MustTranslate]
		public string jobString;

		public List<ResearchProjectDef> researchPrerequisites;

		public enum XenotypeType
		{
			Base,
			Archite
		}

		public override void ResolveReferences(ThingDef parentDef)
		{
			if (compClass == typeof(ThingComp))
			{
				Log.Error(parentDef.defName + " has CompProperties_UseEffect_XenotypeForcer_II with ThingComp compClass. Will be used CompUseEffect_XenotypeForcer_II instead.");
				compClass = typeof(CompUseEffect_XenotypeForcer_II);
			}
			if (compClass == typeof(CompUseEffect_GeneGiver) && possibleGenes.NullOrEmpty())
			{
				Log.Error(parentDef.defName + " has CompUseEffect_GeneGiver compClass with null possibleGenes.");
			}
			Log.Warning(parentDef.defName + " uses an outdated serum component.");
		}

		// public CompProperties_UseEffect_XenotypeForcer_II()
		// {
		// compClass = typeof(CompUseEffect_XenotypeForcer_II);
		// }
	}

	[Obsolete]
	public class CompProperties_UseEffect_XenotypeForcer : CompProperties_UseEffect
	{

		// public bool hybrid = false;

		public XenotypeForcerType xenotypeForcerType = (XenotypeForcerType)0;

		public XenotypeDef xenotypeDef;

		// public XenotypeDef perfectCandidate;

		// public bool setXenotypeDirect = false;

		public bool removeEndogenes = false;

		public bool removeXenogenes = true;

		public bool removeSkinColor = true;

		// public bool useCustomXenotype = false;

		public CompProperties_UseEffect_XenotypeForcer()
		{
			compClass = typeof(CompUseEffect_XenotypeForcer);
		}

		public enum XenotypeForcerType
		{
			Base,
			Hybrid,
			Custom,
			CustomHybrid
		}

		public override void ResolveReferences(ThingDef parentDef)
		{
			Log.Warning(parentDef.defName + " uses an outdated serum component.");
		}

	}

	[Obsolete]
	public class CompProperties_UseEffect_GeneRestoration : CompProperties_UseEffect
	{

		public int daysDelay = 8;

		public bool canBeUsedInCaravan = false;

		// public List<ShapeshiftModeDef> unlockModes;

		public GeneDef geneDef;

		public HediffDef hediffDef;

		public List<HediffDef> hediffsToRemove;

		public bool disableShapeshiftComaAfterUse = false;

		public bool disableShapeshiftGenesRegrowAfterUse = false;

		public CompProperties_UseEffect_GeneRestoration()
		{
			compClass = typeof(CompUseEffect_GeneRestoration);
		}

		public override void ResolveReferences(ThingDef parentDef)
		{
			if (!hediffsToRemove.NullOrEmpty())
			{
				if (parentDef.descriptionHyperlinks.NullOrEmpty())
				{
					parentDef.descriptionHyperlinks = new();
				}
				foreach (HediffDef hediffDef in hediffsToRemove)
				{
					parentDef.descriptionHyperlinks.Add(new DefHyperlink(hediffDef));
				}
			}
			Log.Warning(parentDef.defName + " uses an outdated serum component.");
		}

	}

	[Obsolete]
	public class CompProperties_TargetEffect_DoJobOnTarget : CompProperties
	{
		public JobDef jobDef;

		public ThingDef moteDef;

		public XenotypeDef xenotypeDef = null;

		public CompProperties_TargetEffect_DoJobOnTarget()
		{
			compClass = typeof(CompTargetEffect_DoJobOnTarget);
		}

		public override void ResolveReferences(ThingDef parentDef)
		{
			Log.Warning(parentDef.defName + " uses an outdated serum component.");
		}

	}

	[Obsolete]
	public class CompUseEffect_XenotypeForcer : CompUseEffect
	{
		public CompProperties_UseEffect_XenotypeForcer Props => (CompProperties_UseEffect_XenotypeForcer)props;

		public override void DoEffect(Pawn pawn)
		{
			// Humanity check
			if (SerumUtility.HumanityCheck(pawn))
			{
				return;
			}
			// Main
			if (pawn.health.hediffSet.HasHediff(HediffDefOf.XenogermReplicating))
			{
				pawn.health.RemoveHediff(pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.XenogermReplicating));
				return;
			}
			List<string> blackListedXenotypes = XenotypeFilterUtility.BlackListedXenotypesForSerums(false);
			switch (Props.xenotypeForcerType)
			{
				case CompProperties_UseEffect_XenotypeForcer.XenotypeForcerType.Base:
					SerumUtility.XenotypeSerum(pawn, blackListedXenotypes, Props.xenotypeDef, Props.removeEndogenes, Props.removeXenogenes);
					break;
				case CompProperties_UseEffect_XenotypeForcer.XenotypeForcerType.Hybrid:
					SerumUtility.HybridXenotypeSerum(pawn, blackListedXenotypes, Props.xenotypeDef);
					break;
				case CompProperties_UseEffect_XenotypeForcer.XenotypeForcerType.Custom:
					SerumUtility.CustomXenotypeSerum(pawn, blackListedXenotypes);
					break;
				case CompProperties_UseEffect_XenotypeForcer.XenotypeForcerType.CustomHybrid:
					SerumUtility.CustomHybridXenotypeSerum(pawn, blackListedXenotypes);
					break;
			}
			pawn.health.AddHediff(HediffDefOf.XenogerminationComa);
			GeneUtility.UpdateXenogermReplication(pawn);
			if (PawnUtility.ShouldSendNotificationAbout(pawn))
			{
				int max = HediffDefOf.XenogerminationComa.CompProps<HediffCompProperties_Disappears>().disappearsAfterTicks.max;
				Find.LetterStack.ReceiveLetter("LetterLabelGenesImplanted".Translate(), "WVC_LetterTextGenesImplanted".Translate(pawn.Named("TARGET"), max.ToStringTicksToPeriod().Named("COMADURATION")), LetterDefOf.NeutralEvent, new LookTargets(pawn));
			}
			ReimplanterUtility.PostSerumUsedHook(pawn, true);
		}

		public override AcceptanceReport CanBeUsedBy(Pawn p)
		{
			if (!ReimplanterUtility.IsHuman(p))
			{
				return "WVC_PawnIsAndroidCheck".Translate();
			}
			if (p.health.hediffSet.HasHediff(HediffDefOf.XenogermReplicating))
			{
				return "WVC_XaG_GeneShapeshifter_DisabledGenesRegrowing".Translate();
			}
			return true;
		}

	}

	[Obsolete]
	public class CompUseEffect_XenotypeForcer_II : CompUseEffect
	{
		public XenotypeDef xenotype = null;

		public CompProperties_UseEffect_XenotypeForcer_II Props => (CompProperties_UseEffect_XenotypeForcer_II)props;

		public override void PostPostMake()
		{
			base.PostPostMake();
			if (!Props.possibleXenotypes.NullOrEmpty())
			{
				xenotype = Props.possibleXenotypes.RandomElement();
			}
			if (xenotype == null)
			{
				List<XenotypeDef> xenotypeDef = XenotypeFilterUtility.WhiteListedXenotypes(true);
				switch (Props.xenotypeType)
				{
					case CompProperties_UseEffect_XenotypeForcer_II.XenotypeType.Base:
						xenotype = xenotypeDef.Where((XenotypeDef randomXenotypeDef) => !XaG_GeneUtility.XenotypeHasArchites(randomXenotypeDef)).RandomElement();
						break;
					case CompProperties_UseEffect_XenotypeForcer_II.XenotypeType.Archite:
						xenotype = xenotypeDef.Where((XenotypeDef randomXenotypeDef) => XaG_GeneUtility.XenotypeHasArchites(randomXenotypeDef)).RandomElement();
						break;
				}
				if (xenotype == null)
				{
					Log.Error("Generated serum with null xenotype. Choose random.");
					xenotype = xenotypeDef.RandomElement();
				}
				if (xenotype == null)
				{
					Log.Error("Xenotype is still null. Do not report this to the developer, you yourself created this creepy world filled with bugs. To fix the situation, reset the filter in the " + "WVC_BiotechSettings".Translate() + " mod settings and restart the game.");
				}
			}
		}

		// public override void PostSpawnSetup(bool respawningAfterLoad)
		// {
		// if (!respawningAfterLoad)
		// {
		// return;
		// }
		// if (parent.Map != null)
		// {
		// xenotype = null;
		// }
		// }

		public override string TransformLabel(string label)
		{
			if (xenotype == null)
			{
				// return parent.def.label + " " + "WVC_XaG_SuremRetuneUntuned_Label".Translate();
				return parent.def.label + " (ERR)";
				// return "(ERROR: XENOTYPES LIST IS NULL)";
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
			//bool perfectCandidate = SerumUtility.HasCandidateGene(pawn);
			List<string> blackListedXenotypes = XenotypeFilterUtility.BlackListedXenotypesForSerums(false);
			SerumUtility.XenotypeSerum(pawn, blackListedXenotypes, xenotype, Props.removeEndogenes, Props.removeXenogenes);
			//if (!perfectCandidate)
			//{
			//}
			pawn.health.AddHediff(HediffDefOf.XenogerminationComa);
			GeneUtility.UpdateXenogermReplication(pawn);
			if (PawnUtility.ShouldSendNotificationAbout(pawn))
			{
				int max = HediffDefOf.XenogerminationComa.CompProps<HediffCompProperties_Disappears>().disappearsAfterTicks.max;
				Find.LetterStack.ReceiveLetter("LetterLabelGenesImplanted".Translate(), "WVC_LetterTextGenesImplanted".Translate(pawn.Named("TARGET"), max.ToStringTicksToPeriod().Named("COMADURATION")), LetterDefOf.NeutralEvent, new LookTargets(pawn));
			}
			ReimplanterUtility.PostSerumUsedHook(pawn, true);
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
			if (!MiscUtility.AllProjectsFinished(Props.researchPrerequisites, out ResearchProjectDef nonResearched))
			{
				command_Action.Disable("WVC_XaG_SuremRetuneJob_ResearchPrerequisites".Translate(nonResearched.LabelCap));
			}
			yield return command_Action;
		}

		public override bool AllowStackWith(Thing other)
		{
			CompUseEffect_XenotypeForcer_II otherXeno = other.TryGetComp<CompUseEffect_XenotypeForcer_II>();
			if (otherXeno != null && otherXeno.xenotype != null && otherXeno.xenotype == xenotype)
			{
				return true;
			}
			return false;
		}

		public override AcceptanceReport CanBeUsedBy(Pawn p)
		{
			if (xenotype == null)
			{
				return "WVC_XaG_SuremRetuneShouldBeTunedWarn_Label".Translate();
			}
			if (!ReimplanterUtility.IsHuman(p))
			{
				return "WVC_PawnIsAndroidCheck".Translate();
			}
			if (p.health.hediffSet.HasHediff(HediffDefOf.XenogermReplicating))
			{
				return "WVC_XaG_GeneShapeshifter_DisabledGenesRegrowing".Translate();
			}
			return true;
		}

		public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
		{
			if (Props.retuneJob == null || Props.jobString.NullOrEmpty())
			{
				yield break;
			}
			if (!selPawn.CanReach(parent, PathEndMode.ClosestTouch, Danger.Deadly))
			{
				yield break;
			}
			if (selPawn.CurJob != null && selPawn.CurJob.def == Props.retuneJob && selPawn.CurJob.targetA.Thing == parent)
			{
				yield break;
			}
			if (!MiscUtility.AllProjectsFinished(Props.researchPrerequisites, out ResearchProjectDef nonResearched))
			{
				yield return new FloatMenuOption(Props.jobString + " (" + "WVC_XaG_SuremRetuneJob_ResearchPrerequisites".Translate(nonResearched.LabelCap) + ")", null);
				yield break;
			}
			yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(Props.jobString + " " + parent.LabelShort, delegate
			{
				Job job = JobMaker.MakeJob(Props.retuneJob, parent);
				selPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
			}), selPawn, parent);
		}

	}

}