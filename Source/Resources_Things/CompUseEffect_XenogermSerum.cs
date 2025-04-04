using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_UseEffect_XenogermSerum : CompProperties_UseEffect
	{

		public XenotypeDef endotypeDef = null;
		public XenotypeDef xenotypeDef = null;

		public JobDef jobDef;

		public ThingDef moteDef;

		public List<GeneDef> possibleGenes;

		public List<XenotypeDef> possibleXenotypes;

		public XenotypeType xenotypeType = (XenotypeType)0;

		public bool removeEndogenes = false;

		public bool removeXenogenes = true;

		public bool removeSkinColor = true;

		public int daysDelay = 8;

		public bool canBeUsedInCaravan = false;

		// public List<ShapeshiftModeDef> unlockModes;

		public GeneDef geneDef;

		public HediffDef hediffDef;

		public List<HediffDef> hediffsToRemove;

		public bool disableShapeshiftComaAfterUse = false;

		public bool disableShapeshiftGenesRegrowAfterUse = false;

		public JobDef retuneJob;

		[MustTranslate]
		public string jobString;

		public List<ResearchProjectDef> researchPrerequisites;

		public enum XenotypeType
		{
			Base,
			Archite
		}

		public CompProperties_UseEffect_XenogermSerum()
		{
			compClass = typeof(CompUseEffect);
		}

		public override void ResolveReferences(ThingDef parentDef)
		{
			if (compClass == typeof(CompUseEffect))
			{
				Log.Error(parentDef.defName + " has CompProperties_UseEffect_XenogermSerum with CompUseEffect compClass.");
			}
			if (compClass == typeof(CompUseEffect_GeneGiver) && possibleGenes.NullOrEmpty())
			{
				Log.Error(parentDef.defName + " has CompUseEffect_GeneGiver compClass with null possibleGenes.");
			}
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
			if (parentDef.stackLimit > 1)
			{
				parentDef.stackLimit = 1;
				Log.Warning(parentDef.defName + " should never stack. Stack size changed to 1");
			}
		}

	}

	public class CompUseEffect_XenogermSerum : CompUseEffect
	{

		public XenotypeDef xenotype = null;

		public SaveableXenotypeHolder xenotypeHolder = null;

		public void SetupHolder(XenotypeHolder holder)
		{
			xenotypeHolder =  new SaveableXenotypeHolder(holder);
		}

		//public bool customMode = false;

		public CompProperties_UseEffect_XenogermSerum Props => (CompProperties_UseEffect_XenogermSerum)props;

		public override void PostPostMake()
		{
			base.PostPostMake();
			if (!Props.possibleXenotypes.NullOrEmpty())
			{
				xenotype = Props.possibleXenotypes.RandomElement();
			}
			else if (xenotypeHolder == null)
			{
				List<XenotypeHolder> xenotypeDef = ListsUtility.GetWhiteListedXenotypesHolders(true);
				switch (Props.xenotypeType)
				{
					case CompProperties_UseEffect_XenogermSerum.XenotypeType.Base:
						SetupHolder(xenotypeDef.Where((XenotypeHolder randomXenotypeDef) => !XaG_GeneUtility.XenotypeHasArchites(randomXenotypeDef)).RandomElement());
						break;
					case CompProperties_UseEffect_XenogermSerum.XenotypeType.Archite:
						SetupHolder(xenotypeDef.Where((XenotypeHolder randomXenotypeDef) => XaG_GeneUtility.XenotypeHasArchites(randomXenotypeDef)).RandomElement());
						break;
				}
				if (xenotypeHolder == null)
				{
					Log.Error("Generated serum with null xenotype. Choose random.");
					SetupHolder(xenotypeDef.RandomElement());
				}
				if (xenotypeHolder == null)
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

		public virtual void Notify_SerumCrafted(Pawn pawn)
		{
			xenotype = null;
			xenotypeHolder = null;
		}

		public override string TransformLabel(string label)
		{
			if (xenotypeHolder != null)
			{
				return parent.def.label + " (" + xenotypeHolder.Label + ")";
			}
			if (xenotype == null)
			{
				return parent.def.label + " (" + "WVC_XaG_Untuned".Translate() + ")";
			}
			return parent.def.label + " (" + xenotype.label + ")";
		}

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Defs.Look(ref xenotype, "xenotypeDef");
			Scribe_Deep.Look(ref xenotypeHolder, "xenotypeHolder");
		}

		public override void DoEffect(Pawn pawn)
		{
			if (xenotype != null)
			{
				ReimplanterUtility.SetXenotype(pawn, xenotype);
			}
			else
			{
				if (xenotypeHolder == null)
				{
					Log.Error("Xenotype is still null. Do not report this to the developer, you yourself created this creepy world filled with bugs. To fix the situation, reset the filter in the " + "WVC_BiotechSettings".Translate() + " mod settings and restart the game.");
					return;
				}
				ReimplanterUtility.SetXenotype(pawn, xenotypeHolder);
			}
			//pawn.health.AddHediff(HediffDefOf.XenogerminationComa);
			//GeneUtility.UpdateXenogermReplication(pawn);
			ReimplanterUtility.UpdateXenogermReplication_WithComa(pawn);
			if (PawnUtility.ShouldSendNotificationAbout(pawn))
			{
				int max = HediffDefOf.XenogerminationComa.CompProps<HediffCompProperties_Disappears>().disappearsAfterTicks.max;
				Find.LetterStack.ReceiveLetter("LetterLabelGenesImplanted".Translate(), "WVC_LetterTextGenesImplanted".Translate(pawn.Named("TARGET"), max.ToStringTicksToPeriod().Named("COMADURATION")), LetterDefOf.NeutralEvent, new LookTargets(pawn));
			}
			ReimplanterUtility.PostSerumUsedHook(pawn, true);
		}

		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: SetXenoholderNull",
					action = delegate
					{
						xenotype = null;
						xenotypeHolder = null;
					}
				};
			}
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
			// CompUseEffect_XenogermSerum otherXeno = other.TryGetComp<CompUseEffect_XenogermSerum>();
			// if (otherXeno != null && otherXeno.xenotype != null && otherXeno.xenotype == xenotype && otherXeno.customMode == customMode)
			// {
				// return true;
			// }
			return false;
		}

		public override AcceptanceReport CanBeUsedBy(Pawn p)
		{
			if (xenotype == null && xenotypeHolder == null)
			{
				return "WVC_XaG_SuremRetuneShouldBeTunedWarn_Label".Translate();
			}
			if (!ReimplanterUtility.IsHuman(p))
			{
				return "WVC_PawnIsAndroidCheck".Translate();
			}
			// if (p.health.hediffSet.HasHediff(HediffDefOf.XenogermReplicating))
			// {
				// return "WVC_XaG_GeneShapeshifter_DisabledGenesRegrowing".Translate();
			// }
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
