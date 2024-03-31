using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{

	public class CompAbilityEffect_ReimplanterThrallMaker : CompAbilityEffect
	{

		public new CompProperties_AbilityReimplanter Props => (CompProperties_AbilityReimplanter)props;

		[Unsaved(false)]
		private Gene_ThrallMaker cachedReimplanterGene;

		public Gene_ThrallMaker ReimplanterGene
		{
			get
			{
				if (cachedReimplanterGene == null || !cachedReimplanterGene.Active)
				{
					cachedReimplanterGene = parent?.pawn?.genes?.GetFirstGeneOfType<Gene_ThrallMaker>();
				}
				return cachedReimplanterGene;
			}
		}

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			base.Apply(target, dest);
			Pawn innerPawn = ((Corpse)target.Thing).InnerPawn;
			if (innerPawn != null)
			{
				UndeadUtility.ResurrectWithSickness(innerPawn, null);
				if ((innerPawn.Faction == null || innerPawn.Faction != Faction.OfPlayer))
				{
					RecruitUtility.Recruit(innerPawn, Faction.OfPlayer, parent.pawn);
				}
				DuplicateUtility.NullifyBackstory(innerPawn);
				DuplicateUtility.NullifySkills(innerPawn);
				if (ModsConfig.IdeologyActive)
				{
					innerPawn.ideo.SetIdeo(parent.pawn.ideo.Ideo);
				}
				ThrallMaker(innerPawn, ReimplanterGene.thrallDef);
				foreach (GeneDef item in Props.geneDefs)
				{
					innerPawn.genes?.AddGene(item, false);
				}
				if (WVC_Biotech.settings.thrallMaker_ThrallsInheritMasterGenes)
				{
					foreach (GeneDef item in Props.inheritableGenes)
					{
						if (!parent.pawn.genes.HasGene(item))
						{
							continue;
						}
						if (item.prerequisite != null && !innerPawn.genes.HasGene(item.prerequisite))
						{
							continue;
						}
						if (!innerPawn.genes.HasGene(item))
						{
							innerPawn.genes?.AddGene(item, false);
						}
					}
				}
				DuplicateUtility.RemoveAllGenes_Overridden(innerPawn);
				GeneUtility.UpdateXenogermReplication(innerPawn);
				FleckMaker.AttachedOverlay(innerPawn, FleckDefOf.FlashHollow, new Vector3(0f, 0f, 0.26f));
				if (PawnUtility.ShouldSendNotificationAbout(parent.pawn) || PawnUtility.ShouldSendNotificationAbout(innerPawn))
				{
					Find.LetterStack.ReceiveLetter("WVC_XaG_LetterLabelThrallImplanted".Translate(), "WVC_XaG_LetterTextThrallIImplanted".Translate(parent.pawn.Named("CASTER"), innerPawn.Named("TARGET")) + "\n\n" + ReimplanterGene.thrallDef.description, WVC_GenesDefOf.WVC_XaG_UndeadEvent, new LookTargets(parent.pawn, innerPawn));
				}
			}
		}

		public static void ThrallMaker(Pawn pawn, ThrallDef thrallDef)
		{
			Pawn_GeneTracker genes = pawn.genes;
			foreach (Gene item in genes.Xenogenes.ToList())
			{
				pawn.genes?.RemoveGene(item);
			}
			foreach (Gene item in genes.Endogenes.ToList())
			{
				pawn.genes?.RemoveGene(item);
			}
			ReimplanterUtility.UnknownXenotype(pawn, thrallDef.label, thrallDef.xenotypeIconDef);
			bool xenotypeHasSkinColor = false;
			bool xenotypeHasHairColor = false;
			List<GeneDef> xenotypeGenes = thrallDef.genes;
			for (int i = 0; i < xenotypeGenes.Count; i++)
			{
				pawn.genes?.AddGene(xenotypeGenes[i], false);
				if (xenotypeGenes[i].skinColorBase != null || xenotypeGenes[i].skinColorOverride != null)
				{
					xenotypeHasSkinColor = true;
				}
				if (xenotypeGenes[i].hairColorOverride != null)
				{
					xenotypeHasHairColor = true;
				}
			}
			if (!xenotypeHasSkinColor)
			{
				pawn.genes?.AddGene(WVC_GenesDefOf.Skin_SheerWhite, false);
			}
			if (!xenotypeHasHairColor)
			{
				pawn.genes?.AddGene(WVC_GenesDefOf.Hair_SnowWhite, false);
			}
		}

		public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
		{
			if (target.HasThing && target.Thing is Corpse corpse)
			{
				if (corpse.GetRotStage() == RotStage.Dessicated)
				{
					if (throwMessages)
					{
						Messages.Message("MessageCannotResurrectDessicatedCorpse".Translate(), corpse, MessageTypeDefOf.RejectInput, historical: false);
					}
					return false;
				}
				Pawn innerPawn = corpse.InnerPawn;
				if (!innerPawn.IsHuman() || ReimplanterGene == null)
				{
					if (throwMessages)
					{
						Messages.Message("WVC_PawnIsAndroidCheck".Translate(), innerPawn, MessageTypeDefOf.RejectInput, historical: false);
					}
					return false;
				}
			}
			return base.Valid(target, throwMessages);
		}

		public override IEnumerable<Mote> CustomWarmupMotes(LocalTargetInfo target)
		{
			yield return MoteMaker.MakeAttachedOverlay(((Corpse)target.Thing).InnerPawn, ThingDefOf.Mote_XenogermImplantation, new Vector3(0f, 0f, 0.3f));
		}

		// public override string CompInspectStringExtra()
		// {
			// return null;
		// }

	}
}
