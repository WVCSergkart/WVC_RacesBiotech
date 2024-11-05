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

		public string uniqueTag = "XaG_Cycle";

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

	public class CompBiosculpterPod_XenotypeHolderCycle : CompBiosculpterPod_Cycle
	{

		public int additionalCycleDays = 0;

		private SaveableXenotypeHolder xenotypeHolder;

		public bool ShouldInterrupt => xenotypeHolder == null;

		public new CompProperties_BiosculpterPod_XenogermCycle Props => (CompProperties_BiosculpterPod_XenogermCycle)props;

		public void StartCycle()
		{
			Find.WindowStack.Add(new Dialog_BiosculpterPod(this));
		}

		public void ResetCycle()
		{
			xenotypeHolder = null;
			additionalCycleDays = 0;
		}

		public void SetupHolder(XenotypeHolder holder, int additionalCycleDays)
		{
			this.additionalCycleDays = additionalCycleDays;
			SaveableXenotypeHolder newHolder = new();
			newHolder.xenotypeDef = holder.xenotypeDef;
			newHolder.name = holder.name;
			newHolder.iconDef = holder.iconDef;
			newHolder.genes = holder.genes;
			newHolder.inheritable = holder.inheritable;
			xenotypeHolder = newHolder;
		}

		public override void CycleCompleted(Pawn pawn)
		{
			if (xenotypeHolder != null)
			{
				if (!xenotypeHolder.CustomXenotype)
				{
					ReimplanterUtility.SetXenotype(pawn, xenotypeHolder.xenotypeDef);
				}
				else
				{
					ReimplanterUtility.SetCustomXenotype(pawn, xenotypeHolder);
				}
			}
			Messages.Message("WVC_XaG_XenogermCycleCompleted".Translate(pawn.Named("PAWN")), pawn, MessageTypeDefOf.PositiveEvent);
			ResetCycle();
		}

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look(ref additionalCycleDays, Props.uniqueTag + "_additionalCycleDays", 0);
			Scribe_Deep.Look(ref xenotypeHolder, Props.uniqueTag + "_xenotypeHolder");
		}

		//public override IEnumerable<Gizmo> CompGetGizmosExtra()
		//{
		//	yield return new Command_Action
		//	{
		//		defaultLabel = "WVC_XaG_XenoTreeXenotypeChooseLabel".Translate(),
		//		defaultDesc = "WVC_XaG_XenoTreeXenotypeChooseDesc".Translate(),
		//		Disabled = parent.TryGetComp<CompBiosculpterPod>().State != BiosculpterPodState.SelectingCycle,
		//		icon = GetIcon(),
		//		action = delegate
		//		{
		//			Find.WindowStack.Add(new Dialog_BiosculpterPod(this));
		//		}
		//	};
		//}

		//private Texture2D GetIcon()
		//{
		//	if (xenotypeHolder != null)
		//	{
		//		if (xenotypeHolder.iconDef != null)
		//		{
		//			return xenotypeHolder.iconDef.Icon;
		//		}
		//		else
		//		{
		//			return ContentFinder<Texture2D>.Get(xenotypeHolder.xenotypeDef.iconPath);
		//		}
		//	}
		//	return Props.Icon;
		//}

	}

}
