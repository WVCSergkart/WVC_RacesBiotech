using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public static class DustUtility
	{

		public static void OffsetNeedFood(Pawn pawn, float offset)
		{
			// if (!ModsConfig.BiotechActive)
			// {
				// return;
			// }
			Need_Food need_Food = pawn.needs?.food;
			if (need_Food != null)
			{
				need_Food.CurLevel += offset;
			}
		}

		// public static void OffsetDust(Pawn pawn, float offset)
		// {
			// if (!ModsConfig.BiotechActive)
			// {
				// return;
			// }
			// Gene_Dust gene_Hemogen = pawn.genes?.GetFirstGeneOfType<Gene_Dust>();
			// if (gene_Hemogen != null)
			// {
				// gene_Hemogen.Value += offset;
			// }
		// }

		public static bool PawnInPronePosition(Pawn pawn)
		{
			if (pawn.Downed || pawn.Deathresting || RestUtility.InBed(pawn))
			{
				return true;
			}
			return false;
		}

		public static void ReimplantGenes(Pawn caster, Pawn recipient)
		{
			if (!ModLister.CheckBiotech("xenogerm reimplantation"))
			{
				return;
			}
			ReimplanterUtility.ReimplantGenesBase(caster, recipient);
			if (PawnUtility.ShouldSendNotificationAbout(recipient))
			{
				int max = HediffDefOf.XenogerminationComa.CompProps<HediffCompProperties_Disappears>().disappearsAfterTicks.max;
				Find.LetterStack.ReceiveLetter("LetterLabelGenesImplanted".Translate(), "WVC_LetterTextGenesImplanted".Translate(recipient.Named("TARGET"), max.ToStringTicksToPeriod().Named("COMADURATION")), LetterDefOf.NeutralEvent, new LookTargets(recipient));
			}
		}

		// Mecha summon
		public static bool MechanoidIsPlayerMechanoid(PawnKindDef mech)
		{
			if (mech.race.race.IsMechanoid 
			&& mech.defName.Contains("Mech_") 
			&& MechDefNameShouldNotContain(mech.defName)
			&& MechDefNameShouldNotContain(mech.race.defName)
			&& mech.race.race.thinkTreeMain == WVC_GenesDefOf.Mechanoid 
			&& mech.race.race.thinkTreeConstant == WVC_GenesDefOf.MechConstant 
			// && mech.race.race.maxMechEnergy == 100
			&& mech.race.race.lifeStageAges.Count > 1
			&& EverControllable(mech.race)
			&& EverRepairable(mech.race))
			{
				return true;
			}
			return false;
		}

		public static bool MechDefNameShouldNotContain(string defName)
		{
			List<string> list = new();
			foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
			{
				list.AddRange(item.mechDefNameShouldNotContain);
			}
			if (list.Contains(defName))
			{
				return false;
			}
			return true;
		}

		public static bool EverControllable(ThingDef def)
		{
			List<CompProperties> comps = def.comps;
			for (int i = 0; i < comps.Count; i++)
			{
				if (comps[i].compClass == typeof(CompOverseerSubject))
				{
					return true;
				}
			}
			return false;
		}

		public static bool EverRepairable(ThingDef def)
		{
			List<CompProperties> comps = def.comps;
			for (int i = 0; i < comps.Count; i++)
			{
				if (comps[i].compClass == typeof(CompMechRepairable))
				{
					return true;
				}
			}
			return false;
		}

	}
}
