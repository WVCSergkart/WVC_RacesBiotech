using System;
using System.Linq;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class CompAbilityEffect_Devourer : CompAbilityEffect
	{
		public new CompProperties_AbilityChimera Props => (CompProperties_AbilityChimera)props;

		//[Unsaved(false)]
		//private Gene_Chimera cachedChimeraGene;
		public Gene_Chimera ChimeraGene => parent?.pawn?.genes?.GetFirstGeneOfType<Gene_Chimera>();

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			base.Apply(target, dest);
			Pawn victim = target.Pawn;
			if (victim != null)
			{
				DevourTarget(victim);
			}
		}

		private void DevourTarget(Pawn victim)
		{
			string phase = "start";
			try
			{
				Pawn caster = parent.pawn;
				//if (victim.IsHuman())
				//{
				//}
				IfVictimIsHumanlike(victim, caster, ref phase);
				//else
				//{

				//}
				if (victim.RaceProps.IsFlesh)
				{
					phase = "get food";
					if (victim.TryGetNeedFood(out Need_Food victimFood) && caster.TryGetNeedFood(out Need_Food casterFood))
					{
						casterFood.CurLevel += victimFood.CurLevel;
						//for (int i = 0; i < victimFood.CurLevel; i++)
						//{
						//	if (Rand.Chance(0.25f))
						//	{
						//		gene_Chimera?.GetRandomGene();
						//	}
						//}
					}
					phase = "add hediff";
					if (Props.hediffDef != null)
					{
						Hediff hediff = caster.health.GetOrAddHediff(Props.hediffDef);
						HediffComp_Disappears hediffComp_Disappears = hediff.TryGetComp<HediffComp_Disappears>();
						if (hediffComp_Disappears != null)
						{
							hediffComp_Disappears.SetDuration(hediffComp_Disappears.ticksToDisappear += (int)(60000 * victim.BodySize));
						}
					}
					phase = "meat boom";
					if (ModsConfig.AnomalyActive)
					{
						MiscUtility.MeatSplatter(victim, FleshbeastUtility.MeatExplosionSize.Large, 7);
					}
					phase = "Notify_DevouredFlesh";
					Notify_DevouredFlesh(caster, victim);
					//phase = "try fleshmass overgrow";
					//caster.genes?.GetFirstGeneOfType<Gene_FleshmassNucleus>()?.TryGiveMutation();
				}
				else
				{
					phase = "EMP BOOM";
					Notify_DevouredMech(caster, victim);
					GenExplosion.DoExplosion(victim.PositionHeld, victim.MapHeld, 5f, DamageDefOf.EMP, victim);
				}
				phase = "kill and destroy";
				victim.Kill(new(DamageDefOf.ExecutionCut, 99999, 9999, instigator: caster));
				victim.Corpse?.Kill(new(DamageDefOf.ExecutionCut, 99999, 9999, instigator: caster));
				phase = "message";
				Messages.Message("WVC_XaG_GeneDevourer_Message".Translate(victim.NameShortColored), victim, MessageTypeDefOf.NeutralEvent, historical: false);
			}
			catch (Exception arg)
			{
				Log.Error("Failed devour target: " + victim.Name + ". On phase: " + phase + ". Reason: " + arg);
			}
		}

		private void IfVictimIsHumanlike(Pawn victim, Pawn caster, ref string phase)
		{
			phase = "change goodwill";
			if (victim.HomeFaction != null && !victim.HomeFaction.IsPlayer && !victim.HostileTo(caster.Faction) || victim.IsQuestLodger())
			{
				int goodwillChange = (victim.RaceProps.Humanlike ? (-29) : (-21)) * (victim.guilt.IsGuilty ? 1 : 2);
				if (victim.kindDef.factionHostileOnDeath || victim.kindDef.factionHostileOnKill && !victim.guilt.IsGuilty)
				{
					goodwillChange = caster.Faction.GoodwillToMakeHostile(victim.HomeFaction);
				}
				victim.HomeFaction.TryAffectGoodwillWith(caster.Faction, goodwillChange, canSendMessage: true, true, reason: RimWorld.HistoryEventDefOf.MemberKilled);
			}
			//float genesFactor = 0.01f;
			if (victim.IsHuman())
			{
				phase = "Notify_DevourerHuman";
				Notify_DevouredHuman(caster, victim);
				phase = "reset xenotype";
				float genesFactor = victim.genes.GenesListForReading.Count * 0.01f;
				ReimplanterUtility.SetXenotype(victim, XenotypeDefOf.Baseliner);
				phase = "copy skills";
				CopySkillsExp(caster, victim, genesFactor);
				phase = "inhumanize";
				if (Rand.Chance(caster.relations.OpinionOf(victim) * 0.01f))
				{
					Gene_Inhumanized.Inhumanize(caster);
				}
			}
			phase = "drop apparel";
			victim.apparel?.DropAll(victim.Position, true, false);
		}

		private void Notify_DevouredMech(Pawn caster, Pawn victim)
		{
			foreach (Gene gene in caster.genes.GenesListForReading)
			{
				try
				{
					if (gene is IGeneDevourer geneDevourer && gene.Active)
					{
						geneDevourer.Notify_DevouredMech(victim);
					}
				}
				catch (Exception arg)
				{
					Log.Error("Failed Notify_DevouredMech. Gene: " + gene.def.defName + ". Reason: " + arg.Message);
				}
			}
		}

		private void Notify_DevouredHuman(Pawn caster, Pawn victim)
		{
			foreach (Gene gene in caster.genes.GenesListForReading)
			{
				try
				{
					if (gene is IGeneDevourer geneDevourer && gene.Active)
					{
						geneDevourer.Notify_DevouredHuman(victim);
					}
				}
				catch (Exception arg)
				{
					Log.Error("Failed Notify_DevouredHuman. Gene: " + gene.def.defName + ". Reason: " + arg.Message);
				}
			}
		}

		private void Notify_DevouredFlesh(Pawn caster, Pawn victim)
		{
			foreach (Gene gene in caster.genes.GenesListForReading)
			{
				try
				{
					if (gene is IGeneDevourer geneDevourer && gene.Active)
					{
						geneDevourer.Notify_DevouredFlesh(victim);
					}
				}
				catch (Exception arg)
				{
					Log.Error("Failed Notify_DevouredFlesh. Gene: " + gene.def.defName + ". Reason: " + arg.Message);
				}
			}
		}

		private void CopySkillsExp(Pawn casterPawn, Pawn victimPawn, float factor)
		{
			if (victimPawn.skills == null)
			{
				return;
			}
			foreach (SkillRecord victimSkillRecord in victimPawn.skills.skills.ToList())
			{
				if (victimSkillRecord.TotallyDisabled)
				{
					continue;
				}
				foreach (SkillRecord casterSkillRecord in casterPawn.skills.skills.ToList())
				{
					if (casterSkillRecord.TotallyDisabled)
					{
						continue;
					}
					casterSkillRecord.Learn(victimSkillRecord.XpTotalEarned * factor);
				}
			}
		}

		public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
		{
			Pawn pawn = target.Pawn;
			if (pawn == null)
			{
				return false;
			}
			if (parent.pawn.IsQuestLodger())
			{
				if (throwMessages)
				{
					Messages.Message("WVC_XaG_PawnIsQuestLodgerMessage".Translate(parent.pawn), parent.pawn, MessageTypeDefOf.RejectInput, historical: false);
				}
				return false;
			}
			//if (!pawn.IsHuman())
			//{
			//	if (throwMessages)
			//	{
			//		Messages.Message("WVC_PawnIsAndroidCheck".Translate(), parent.pawn, MessageTypeDefOf.RejectInput, historical: false);
			//	}
			//	return false;
			//}
			return base.Valid(target, throwMessages);
		}

		public override Window ConfirmationDialog(LocalTargetInfo target, Action confirmAction)
		{
			if (target.Pawn.HostileTo(parent.pawn))
			{
				return null;
			}
			return Dialog_MessageBox.CreateConfirmation("WVC_XaG_WarningPawnWillDieFromDevourer".Translate(target.Pawn.Named("PAWN")), confirmAction, destructive: true);
		}

	}

}
