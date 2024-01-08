using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

    public static class ReimplanterUtility
	{

		public static void Reimplanter(Pawn caster, Pawn recipient, bool endogenes = true, bool xenogenes = true)
		{
			if (!ModLister.CheckBiotech("xenogerm reimplantation"))
			{
				return;
			}
			QuestUtility.SendQuestTargetSignals(caster.questTags, "XenogermReimplanted", caster.Named("SUBJECT"));
			// ReimplanterUtility.ReimplantGenesBase(caster, recipient);
			ReimplanterUtility.ReimplantGenesHybrid(caster, recipient, endogenes, xenogenes);
			// GeneUtility.ExtractXenogerm(caster);
			ExtractXenogerm(caster);
		}

		public static void ExtractXenogerm(Pawn pawn, int overrideDurationTicks = -1)
		{
			if (ModLister.CheckBiotech("xenogerm extraction"))
			{
				pawn.health.AddHediff(HediffDefOf.XenogermLossShock);
				if (GeneUtility.PawnWouldDieFromReimplanting(pawn))
				{
					SetXenotype(pawn, XenotypeDefOf.Baseliner);
				}
				Hediff hediff = HediffMaker.MakeHediff(HediffDefOf.XenogermReplicating, pawn);
				if (overrideDurationTicks > 0)
				{
					hediff.TryGetComp<HediffComp_Disappears>().ticksToDisappear = overrideDurationTicks;
				}
				pawn.health.AddHediff(hediff);
			}
		}

		public static void SetXenotypeDirect(Pawn caster, Pawn recipient, XenotypeDef xenotypeDef = null, bool changeXenotype = true)
		{
			if (changeXenotype)
			{
				recipient.genes?.SetXenotypeDirect(caster != null ? caster.genes.Xenotype : xenotypeDef);
				recipient.genes.xenotypeName = caster?.genes.xenotypeName;
				recipient.genes.iconDef = caster?.genes.iconDef;
			}
		}

		public static void UnknownXenotype(Pawn pawn)
		{
			// pawn.genes.xenotypeName = "WVC_XaG_SubXenotypeUnknownXenotypeError".Translate();
			// pawn.genes.iconDef = WVC_GenesDefOf.WVC_XenoRandomKindc;
			pawn.genes.xenotypeName = GeneUtility.GenerateXenotypeNameFromGenes(XaG_GeneUtility.ConvertGenesInGeneDefs(pawn.genes.GenesListForReading));
			pawn.genes.iconDef = DefDatabase<XenotypeIconDef>.AllDefsListForReading.RandomElement();
		}

		public static void GiveReimplantJob(Pawn pawn, Pawn targPawn, JobDef absorbJob)
		{
			pawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(absorbJob, targPawn), JobTag.Misc);
			if (targPawn.HomeFaction != null && !targPawn.HomeFaction.Hidden && targPawn.HomeFaction != pawn.Faction && !targPawn.HomeFaction.HostileTo(Faction.OfPlayer))
			{
				Messages.Message("MessageAbsorbingXenogermWillAngerFaction".Translate(targPawn.HomeFaction, targPawn.Named("PAWN")), pawn, MessageTypeDefOf.CautionInput, historical: false);
			}
		}

		public static bool CanAbsorbGenogerm(Pawn pawn)
		{
			if (pawn?.genes == null)
			{
				return false;
			}
			// if (!pawn.genes.HasGene(GeneDefOf.XenogermReimplanter))
			// {
				// return false;
			// }
			// if (!MiscUtility.HasReimplanterAbility(pawn))
			// {
				// return false;
			// }
			// if (!XaG_GeneUtility.HasActiveGene(gene.def, pawn))
			// {
				// return false;
			// }
			if (pawn.IsPrisonerOfColony && pawn.guest.PrisonerIsSecure)
			{
				return true;
			}
			if (!pawn.Downed)
			{
				return false;
			}
			if (!pawn.genes.GenesListForReading.Any())
			{
				return false;
			}
			return true;
		}

		// ===============================================================

		public static void ReimplantXenogenesFromXenotype(Pawn pawn, XenotypeDef xenotypeDef)
		{
			// pawn.genes.SetXenotypeDirect(xenotypeDef);
			// pawn.genes.xenotypeName = null;
			// pawn.genes.iconDef = null;
			// pawn.genes.cachedHasCustomXenotype = null;
			// pawn.genes.cachedCustomXenotype = null;
			SetXenotypeDirect(null, pawn, xenotypeDef);
			Pawn_GeneTracker pawnGenes = pawn.genes;
			if (pawnGenes != null && pawnGenes.Xenogenes.Count > 0)
			{
				foreach (Gene item in pawn.genes?.Xenogenes)
				{
					pawn.genes?.RemoveGene(item);
				}
			}
			foreach (GeneDef xenogene in xenotypeDef.genes)
			{
				pawn.genes.AddGene(xenogene, xenogene: true);
			}
			if (!xenotypeDef.soundDefOnImplant.NullOrUndefined())
			{
				xenotypeDef.soundDefOnImplant.PlayOneShot(SoundInfo.InMap(pawn));
			}
			pawn.health.AddHediff(HediffDefOf.XenogerminationComa);
			GeneUtility.UpdateXenogermReplication(pawn);
		}

		public static void SaveReimplantXenogenesFromXenotype(Pawn pawn, XenotypeDef xenotypeDef)
		{
			if (pawn.genes != null && pawn.genes.Xenogenes.Count <= 0)
			{
				ReimplantXenogenesFromXenotype(pawn, xenotypeDef);
			}
		}

		public static void ReimplantGenesHybrid(Pawn caster, Pawn recipient, bool endogenes = true, bool xenogenes = true)
		{
			Pawn_GeneTracker recipientGenes = recipient.genes;
			if (recipientGenes.Xenogenes.NullOrEmpty() || xenogenes)
			{
				SetXenotypeDirect(caster, recipient);
			}
			if (xenogenes && !recipientGenes.Xenogenes.NullOrEmpty())
			{
				foreach (Gene item in recipientGenes.Xenogenes.ToList())
				{
					recipient.genes?.RemoveGene(item);
				}
			}
			if (endogenes && !recipientGenes.Endogenes.NullOrEmpty())
			{
				foreach (Gene item in recipientGenes.Endogenes.ToList())
				{
					recipient.genes?.RemoveGene(item);
				}
			}
			if (endogenes)
			{
				foreach (Gene endogene in caster.genes.Endogenes)
				{
					recipient.genes.AddGene(endogene.def, xenogene: false);
				}
			}
			if (xenogenes)
			{
				foreach (Gene xenogene in caster.genes.Xenogenes)
				{
					recipient.genes.AddGene(xenogene.def, xenogene: true);
				}
			}
			if (!caster.genes.Xenotype.soundDefOnImplant.NullOrUndefined())
			{
				caster.genes.Xenotype.soundDefOnImplant.PlayOneShot(SoundInfo.InMap(recipient));
			}
			recipient.health.AddHediff(HediffDefOf.XenogerminationComa);
			GeneUtility.UpdateXenogermReplication(recipient);
		}

		[Obsolete]
		public static void ReimplantGenesBase(Pawn caster, Pawn recipient)
		{
			// recipient.genes.SetXenotype(caster.genes.Xenotype);
			SetXenotypeDirect(caster, recipient);
			// recipient.genes.SetXenotypeDirect(caster.genes.Xenotype);
			// recipient.genes.xenotypeName = caster.genes.xenotypeName;
			// recipient.genes.iconDef = caster.genes.iconDef;
			Pawn_GeneTracker recipientGenes = recipient.genes;
			if (recipientGenes != null && recipientGenes.GenesListForReading.Count > 0)
			{
				foreach (Gene item in recipient.genes?.GenesListForReading)
				{
					recipient.genes?.RemoveGene(item);
				}
			}
			foreach (Gene endogene in caster.genes.Endogenes)
			{
				recipient.genes.AddGene(endogene.def, xenogene: false);
			}
			foreach (Gene xenogene in caster.genes.Xenogenes)
			{
				recipient.genes.AddGene(xenogene.def, xenogene: true);
			}
			if (!caster.genes.Xenotype.soundDefOnImplant.NullOrUndefined())
			{
				caster.genes.Xenotype.soundDefOnImplant.PlayOneShot(SoundInfo.InMap(recipient));
			}
			recipient.health.AddHediff(HediffDefOf.XenogerminationComa);
			GeneUtility.UpdateXenogermReplication(recipient);
		}

		public static List<GeneDef> GenesNonCandidatesForSerums()
		{
			List<GeneDef> list = new();
			foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
			{
				list.AddRange(item.nonCandidatesForSerums);
			}
			return list;
		}

		public static List<GeneDef> GenesPerfectCandidatesForSerums()
		{
			List<GeneDef> list = new();
			foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
			{
				list.AddRange(item.perfectCandidatesForSerums);
			}
			return list;
		}

		public static bool DelayedReimplanterIsActive(Pawn pawn)
		{
			if (pawn.health != null && pawn.health.hediffSet != null)
			{
				List<HediffDef> hediffDefs = new();
				foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
				{
					hediffDefs.AddRange(item.blackListedHediffDefForReimplanter);
				}
				for (int i = 0; i < hediffDefs.Count; i++)
				{
					if (pawn.health.hediffSet.HasHediff(hediffDefs[i]))
					{
						return true;
					}
				}
			}
			return false;
		}

		// =============================== Setter ===============================

		public static void SetXenotype_DoubleXenotype(Pawn pawn, XenotypeDef xenotypeDef, List<GeneDef> dontRemoveGeneDefs = null, bool changeXenotype = true)
		{
			if (!xenotypeDef.doubleXenotypeChances.NullOrEmpty() && Rand.Value < xenotypeDef.doubleXenotypeChances.Sum((XenotypeChance x) => x.chance) && xenotypeDef.doubleXenotypeChances.TryRandomElementByWeight((XenotypeChance x) => x.chance, out var result))
			{
				SetXenotype(pawn, result.xenotype, dontRemoveGeneDefs, changeXenotype);
			}
			SetXenotype(pawn, xenotypeDef, dontRemoveGeneDefs, changeXenotype);
		}

		public static void SetXenotype(Pawn pawn, XenotypeDef xenotypeDef, List<GeneDef> dontRemoveGeneDefs = null, bool changeXenotype = true)
		{
			// remove all genes
			List<GeneDef> dontRemoveGenes = new();
			if (!dontRemoveGeneDefs.NullOrEmpty())
			{
				dontRemoveGenes = dontRemoveGeneDefs.ToList();
			}
			Pawn_GeneTracker genes = pawn.genes;
			foreach (Gene item in genes.Xenogenes.ToList())
			{
				if (dontRemoveGenes.Contains(item.def))
				{
					continue;
				}
				pawn.genes?.RemoveGene(item);
			}
			if (xenotypeDef.inheritable || xenotypeDef == XenotypeDefOf.Baseliner)
			{
				// for (int numEndogenes = genes.Endogenes.Count - 1; numEndogenes >= 0; numEndogenes--)
				// {
					// if (dontRemoveGeneDefs.NullOrEmpty() || !dontRemoveGeneDefs.Contains(genes.Xenogenes[numEndogenes].def))
					// {
						// pawn.genes?.RemoveGene(genes.Endogenes[numEndogenes]);
					// }
				// }
				foreach (Gene item in genes.Endogenes.ToList())
				{
					if (dontRemoveGenes.Contains(item.def))
					{
						continue;
					}
					pawn.genes?.RemoveGene(item);
				}
			}
			// else
			// {
				// for (int numXenogenes = genes.Xenogenes.Count - 1; numXenogenes >= 0; numXenogenes--)
				// {
					// if (dontRemoveGeneDefs.NullOrEmpty() || !dontRemoveGeneDefs.Contains(genes.Xenogenes[numXenogenes].def))
					// {
						// pawn.genes?.RemoveGene(genes.Xenogenes[numXenogenes]);
					// }
				// }
			// }
			// Add genes
			SetXenotypeDirect(null, pawn, xenotypeDef, changeXenotype);
			// if (changeXenotype)
			// {
				// pawn.genes.xenotypeName = null;
				// pawn.genes.iconDef = null;
				// pawn.genes.cachedHasCustomXenotype = null;
				// pawn.genes.cachedCustomXenotype = null;
				// pawn.genes?.SetXenotypeDirect(xenotypeDef);
			// }
			bool xenotypeHasSkinColor = false;
			bool xenotypeHasHairColor = false;
			List<GeneDef> xenotypeGenes = xenotypeDef.genes;
			for (int i = 0; i < xenotypeGenes.Count; i++)
			{
				if (dontRemoveGenes.Contains(xenotypeGenes[i]))
				{
					continue;
				}
				pawn.genes?.AddGene(xenotypeGenes[i], !xenotypeDef.inheritable);
				if (xenotypeGenes[i].skinColorBase != null || xenotypeGenes[i].skinColorOverride != null)
				{
					xenotypeHasSkinColor = true;
				}
				if (xenotypeGenes[i].hairColorOverride != null)
				{
					xenotypeHasHairColor = true;
				}
			}
			if ((xenotypeDef.inheritable || xenotypeDef == XenotypeDefOf.Baseliner) && !xenotypeHasSkinColor)
			{
				pawn.genes?.AddGene(WVC_GenesDefOf.Skin_SheerWhite, false);
			}
			if ((xenotypeDef.inheritable || xenotypeDef == XenotypeDefOf.Baseliner) && !xenotypeHasHairColor)
			{
				pawn.genes?.AddGene(WVC_GenesDefOf.Hair_SnowWhite, false);
			}
		}

	}
}
