using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{
	public class CompProperties_BiosculpterPod_XenogermCycle : CompProperties_BiosculpterPod_BaseCycle
	{

		public List<HediffDef> hediffsToRemove;

	}

	public class CompBiosculpterPod_XenogermCycle : CompBiosculpterPod_Cycle
	{

		public new CompProperties_BiosculpterPod_XenogermCycle Props => (CompProperties_BiosculpterPod_XenogermCycle)props;

		public override void CycleCompleted(Pawn pawn)
		{
			HediffUtility.RemoveHediffsFromList(pawn, Props.hediffsToRemove);
			Messages.Message("WVC_XaG_XenogermCycleCompleted".Translate(pawn.Named("PAWN")), pawn, MessageTypeDefOf.PositiveEvent);
		}

	}

	public class CompBiosculpterPod_XenotypeNullifierCycle : CompBiosculpterPod_Cycle
	{

		public override void CycleCompleted(Pawn pawn)
		{
			ReimplanterUtility.SetXenotype(pawn, XenotypeDefOf.Baseliner);
			Messages.Message("WVC_XaG_XenogermCycleCompleted".Translate(pawn.Named("PAWN")), pawn, MessageTypeDefOf.PositiveEvent);
		}

	}

}
