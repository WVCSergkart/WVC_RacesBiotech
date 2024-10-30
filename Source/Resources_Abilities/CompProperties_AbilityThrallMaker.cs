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
			ThrallDef thrallDef = ReimplanterGene?.thrallDef;
			if (thrallDef == null)
			{
				Log.Warning("Thrall maker has null thrallDef.");
				return;
			}
			Pawn innerPawn = ((Corpse)target.Thing).InnerPawn;
			if (innerPawn != null)
			{
				MakeThrall(thrallDef, innerPawn);
				FleckMaker.AttachedOverlay(innerPawn, FleckDefOf.FlashHollow, new Vector3(0f, 0f, 0.26f));
				if (PawnUtility.ShouldSendNotificationAbout(parent.pawn) || PawnUtility.ShouldSendNotificationAbout(innerPawn))
				{
					Find.LetterStack.ReceiveLetter("WVC_XaG_LetterLabelThrallImplanted".Translate(), "WVC_XaG_LetterTextThrallIImplanted".Translate(parent.pawn.Named("CASTER"), innerPawn.Named("TARGET")) + "\n\n" + ReimplanterGene.thrallDef.description, WVC_GenesDefOf.WVC_XaG_UndeadEvent, new LookTargets(parent.pawn, innerPawn));
				}
			}
			parent.StartCooldown((int)(60000 * WVC_Biotech.settings.thrallMaker_cooldownOverride));
		}

		private void MakeThrall(ThrallDef thrallDef, Pawn innerPawn)
        {
            MutantDef mutantDef = thrallDef?.mutantDef;
            GeneResourceUtility.TryResurrectWithSickness(innerPawn, null, false, 0.8f);
            SetStory(innerPawn);
            ReimplantGenes(thrallDef, innerPawn);
            if (ModsConfig.AnomalyActive && mutantDef != null)
            {
                MutantUtility.SetPawnAsMutantInstantly(innerPawn, mutantDef);
            }
            if (innerPawn.Map != null)
            {
                WVC_GenesDefOf.CocoonDestroyed.SpawnAttached(innerPawn, innerPawn.Map).Trigger(innerPawn, null);
            }
			GeneResourceUtility.ResurrectionSicknessWithCustomTick(innerPawn, new IntRange(1500, 3000));
        }

        // =================

        private void SetStory(Pawn innerPawn)
		{
			if (innerPawn.Faction != Faction.OfPlayer)
			{
				RecruitUtility.Recruit(innerPawn, Faction.OfPlayer, parent.pawn);
			}
			DuplicateUtility.NullifyBackstory(innerPawn);
			//DuplicateUtility.NullifySkills(innerPawn);
			DuplicateUtility.CopySkills(parent.pawn, innerPawn);
			if (ModsConfig.IdeologyActive)
			{
				innerPawn.ideo.SetIdeo(parent.pawn.ideo.Ideo);
			}
			innerPawn.skills?.Notify_SkillDisablesChanged();
			innerPawn.Notify_DisabledWorkTypesChanged();
			innerPawn.Drawer?.renderer?.SetAllGraphicsDirty();
		}

		private void ReimplantGenes(ThrallDef thrallDef, Pawn innerPawn)
		{
			List<GeneDef> oldXenotypeGenes = XaG_GeneUtility.ConvertGenesInGeneDefs(innerPawn.genes.GenesListForReading);
			ThrallMaker(innerPawn, thrallDef);
			if (thrallDef.addGenesFromAbility)
			{
				foreach (GeneDef item in Props.geneDefs)
				{
					innerPawn.genes?.AddGene(item, false);
				}
			}
			if (thrallDef.addGenesFromMaster && WVC_Biotech.settings.thrallMaker_ThrallsInheritMasterGenes)
			{
				foreach (GeneDef item in Props.inheritableGenes)
				{
					if (!XaG_GeneUtility.HasGene(item, parent.pawn))
					{
						continue;
					}
					if (item.prerequisite != null && !XaG_GeneUtility.HasGene(item.prerequisite, innerPawn))
					{
						continue;
					}
					if (!XaG_GeneUtility.HasGene(item, innerPawn))
					{
						innerPawn.genes?.AddGene(item, false);
					}
				}
			}
			List<GeneDef> currentPawnGenes = XaG_GeneUtility.ConvertGenesInGeneDefs(innerPawn.genes.GenesListForReading);
			if (!oldXenotypeGenes.NullOrEmpty())
			{
				int count = (int)(oldXenotypeGenes.Count * 0.2f);
				oldXenotypeGenes.Shuffle();
				for (int i = 0; i < oldXenotypeGenes.Count; i++)
                {
                    GeneDef geneDef = oldXenotypeGenes[i];
					if (CanImplant(innerPawn, currentPawnGenes, geneDef))
                    {
                        innerPawn.genes?.AddGene(geneDef, false);
						currentPawnGenes.Add(geneDef);
					}
                    else
                    {
                        count++;
                    }
                    if (i > count)
                    {
                        break;
                    }
                }
			}
			currentPawnGenes = XaG_GeneUtility.ConvertGenesInGeneDefs(innerPawn.genes.GenesListForReading);
			List<GeneDef> allMasterGenes = XaG_GeneUtility.ConvertGenesInGeneDefs(parent.pawn.genes.GenesListForReading);
			if (!allMasterGenes.NullOrEmpty())
			{
				int count = (int)(allMasterGenes.Count * 0.2f);
				allMasterGenes.Shuffle();
				for (int i = 0; i < allMasterGenes.Count; i++)
				{
					GeneDef geneDef = allMasterGenes[i];
					if (CanImplant(innerPawn, currentPawnGenes, geneDef))
					{
						innerPawn.genes?.AddGene(geneDef, false);
						currentPawnGenes.Add(geneDef);
					}
					else
					{
						count++;
					}
					if (i > count)
					{
						break;
					}
				}
			}
			GeneUtility.UpdateXenogermReplication(innerPawn);
			ReimplanterUtility.TrySetSkinAndHairGenes(innerPawn);
			ReimplanterUtility.PostImplantDebug(innerPawn);
		}

        public static bool CanImplant(Pawn innerPawn, List<GeneDef> currentPawnGenes, GeneDef geneDef)
        {
            return geneDef.passOnDirectly && geneDef.prerequisite == null && !XaG_GeneUtility.HasGene(geneDef, innerPawn) && !XaG_GeneUtility.ConflictWith(geneDef, currentPawnGenes) && CanAcceptMetabolismAfterImplanting(currentPawnGenes, geneDef);
		}

		public static bool CanAcceptMetabolismAfterImplanting(List<GeneDef> geneSet, GeneDef gene)
		{
			int result = geneSet.Sum((GeneDef x) => x.biostatMet);
			if (result > 5 && gene.biostatMet < 0 || result < -5 && gene.biostatMet > 0)
            {
				return true;
            }
			result += gene.biostatMet;
			return result < 6 && result > -6;
		}

		// =================

		public static void ThrallMaker(Pawn pawn, ThrallDef thrallDef)
		{
			Pawn_GeneTracker genes = pawn.genes;
			genes.Xenogenes.RemoveAllGenes();
			genes.Endogenes.RemoveAllGenes();
			if (thrallDef.xenotypeDef != null)
			{
				ReimplanterUtility.SetXenotypeDirect(null, pawn, thrallDef.xenotypeDef);
			}
			else
			{
				ReimplanterUtility.UnknownXenotype(pawn, thrallDef.label, thrallDef.xenotypeIconDef);
			}
			List<GeneDef> xenotypeGenes = thrallDef.genes;
			for (int i = 0; i < xenotypeGenes.Count; i++)
			{
				pawn.genes?.AddGene(xenotypeGenes[i], false);
			}
		}

		// =================

		public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
		{
			if (target.HasThing && target.Thing is Corpse corpse)
			{
				ThrallDef thrallDef = ReimplanterGene?.thrallDef;
				// if (ModsConfig.AnomalyActive && thrallDef.resurrectAsShambler)
				// {
					// if (!MutantUtility.CanResurrectAsShambler(corpse))
					// {
						// if (throwMessages)
						// {
							// Messages.Message("MessageCannotResurrectDessicatedCorpse".Translate(), corpse, MessageTypeDefOf.RejectInput, historical: false);
						// }
						// return false;
					// }
					// return true;
				// }
				Pawn innerPawn = corpse.InnerPawn;
				MutantDef mutantDef = thrallDef?.mutantDef;
				if (!innerPawn.IsHuman() || thrallDef == null || innerPawn.IsMutant && !innerPawn.IsMutantOfDef(mutantDef))
				{
					if (throwMessages)
					{
						Messages.Message("WVC_PawnIsAndroidCheck".Translate(), innerPawn, MessageTypeDefOf.RejectInput, historical: false);
					}
					return false;
				}
				if (mutantDef != null && mutantDef.allowedDevelopmentalStages != innerPawn.DevelopmentalStage)
				{
					if (throwMessages)
					{
						Messages.Message("WVC_XaG_WrongDevelopmentalStage".Translate(), innerPawn, MessageTypeDefOf.RejectInput, historical: false);
					}
					return false;
				}
				if (!thrallDef.acceptableRotStages.Contains(corpse.GetRotStage()))
				{
					if (throwMessages)
					{
						Messages.Message("WVC_XaG_MessageWrongRottingStage".Translate(), corpse, MessageTypeDefOf.RejectInput, historical: false);
					}
					return false;
				}
			}
			return base.Valid(target, throwMessages);
		}

		public override IEnumerable<Mote> CustomWarmupMotes(LocalTargetInfo target)
		{
			yield return MoteMaker.MakeAttachedOverlay((Corpse)target.Thing, ThingDefOf.Mote_XenogermImplantation, new Vector3(0f, 0f, 0.3f));
		}

		// public override string CompInspectStringExtra()
		// {
			// return null;
		// }

	}
}
