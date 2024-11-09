using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Opinion : Gene
	{

		public GeneExtension_Opinion Props => def?.GetModExtension<GeneExtension_Opinion>();

		// public int nextTick = 1500;

		public override void Tick()
		{
			base.Tick();
			if (!pawn.IsHashIntervalTick(57250))
			{
				return;
			}
			if (!Active)
			{
				return;
			}
			SetOpinion(pawn, this, Props);
			// ResetCounter();
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: SetOpinion",
					action = delegate
					{
						SetOpinion(pawn, this, Props);
					}
				};
			}
		}

		public static void SetOpinion(Pawn pawn, Gene gene, GeneExtension_Opinion props)
		{
			if (props == null)
			{
				return;
			}
			if (props.AboutMeThoughtDef != null)
			{
				ThoughtUtility.PawnMapOpinionAboutMe(pawn, gene, props.AboutMeThoughtDef, props.targetShouldBePsySensitive, props.targetShouldBeFamily, props.ignoreIfHasGene, props.onlySameXenotype);
			}
			if (props.MeAboutThoughtDef != null)
			{
				ThoughtUtility.MyOpinionAboutPawnMap(pawn, gene, props.MeAboutThoughtDef, props.targetShouldBePsySensitive, props.targetShouldBeFamily, props.ignoreIfHasGene, props.onlySameXenotype);
			}
		}

		// public void ResetCounter()
		// {
			// IntRange range = new(42000, 72000);
			// nextTick = range.RandomInRange();
		// }

	}

	public class Gene_AngelBeauty : Gene_FoodEfficiency
	{

		public GeneExtension_Opinion Props => def?.GetModExtension<GeneExtension_Opinion>();

		public override void Tick()
		{
			base.Tick();
			if (!pawn.IsHashIntervalTick(57250))
			{
				return;
			}
			if (!Active)
			{
				return;
			}
			Gene_Opinion.SetOpinion(pawn, this, Props);
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: SetOpinion",
					action = delegate
					{
						Gene_Opinion.SetOpinion(pawn, this, Props);
					}
				};
			}
		}

	}

	// WIP
	public class Gene_IncestLover : Gene
	{


	}

	public class Gene_DemonBeauty : Gene
	{

		public GeneExtension_Opinion Props => def?.GetModExtension<GeneExtension_Opinion>();

		public override void Tick()
		{
			base.Tick();
			if (!pawn.IsHashIntervalTick(57250))
			{
				return;
			}
			if (!Active)
			{
				return;
			}
			SetOpinion(pawn, this, Props);
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: SetOpinion",
					action = delegate
					{
						SetOpinion(pawn, this, Props);
					}
				};
			}
		}

		public void SetOpinion(Pawn pawn, Gene gene, GeneExtension_Opinion props)
		{
			if (props == null)
			{
				return;
			}
			if (props.AboutMeThoughtDef != null)
			{
				ThoughtUtility.PawnMapOpinionAboutMe(pawn, gene, props.AboutMeThoughtDef, true, false, true, false);
			}
			if (props.sameAsMe_AboutMeThoughtDef != null)
			{
				ThoughtUtility.PawnMapOpinionAboutMe(pawn, gene, props.sameAsMe_AboutMeThoughtDef, true, false, false, true);
			}
		}

	}

	public class Gene_BloodfeederBeauty : Gene, IGeneBloodfeeder
	{

		public GeneExtension_Opinion Props => def?.GetModExtension<GeneExtension_Opinion>();

		public void Notify_Bloodfeed(Pawn victim)
		{
			if (Props == null)
			{
				return;
			}
			if (Props.AboutMeThoughtDef != null)
			{
				victim.needs?.mood?.thoughts?.memories.TryGainMemory(Props.AboutMeThoughtDef, pawn);
			}
		}

	}

	public class Gene_SweetVoice : Gene
	{

		public GeneExtension_Opinion Props => def?.GetModExtension<GeneExtension_Opinion>();

		private int hashIntervalTick = 7200;

		public override void PostAdd()
		{
			base.PostAdd();
			ResetInterval();
		}

		public override void Tick()
		{
			if (!pawn.IsHashIntervalTick(hashIntervalTick))
			{
				return;
			}
			if (!Active)
			{
				return;
			}
			//TryInteractRandomly_CloseTarget(pawn);
			if (ThoughtUtility.TryInteractRandomly(pawn, true, false, true, out Pawn target))
            {
				target.needs?.mood?.thoughts?.memories.TryGainMemory(Props.AboutMeThoughtDef, pawn);
			}
			ResetInterval();
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: TryInteract",
					action = delegate
					{
						//TryInteractRandomly_CloseTarget(pawn);
						ThoughtUtility.TryInteractRandomly(pawn, true, false, true, out _);
					}
				};
			}
		}

		//public bool TryInteractRandomly_CloseTarget(Pawn pawn)
		//{
		//	if (!ThoughtUtility.Telepath_CanInitiateRandomInteraction(pawn, false))
		//	{
		//		return false;
		//	}
		//	List<Pawn> workingList = pawn.Map.mapPawns.SpawnedPawnsInFaction(pawn.Faction);
		//	workingList.Shuffle();
		//	List<InteractionDef> allDefsListForReading = DefDatabase<InteractionDef>.AllDefsListForReading;
		//	for (int i = 0; i < workingList.Count; i++)
		//	{
		//		Pawn p = workingList[i];
		//		if (!p.RaceProps.Humanlike)
		//		{
		//			continue;
		//		}
		//		if (!p.IsPsychicSensitive())
		//		{
		//			continue;
		//		}
		//		if (!InteractionUtility.IsGoodPositionForInteraction(pawn, p))
		//		{
		//			continue;
		//		}
		//		if (p != pawn && ThoughtUtility.Telepath_CanInteractNowWith(pawn, p, ignoreTalking: false) && InteractionUtility.CanReceiveRandomInteraction(p) && !pawn.HostileTo(p) && allDefsListForReading.TryRandomElementByWeight((InteractionDef x) => (!ThoughtUtility.Telepath_CanInteractNowWith(pawn, p, ignoreTalking: false, x)) ? 0f : x.Worker.RandomSelectionWeight(pawn, p), out var result))
		//		{
		//			if (ThoughtUtility.TryInteractWith(pawn, p, result, true))
		//			{
		//				p.needs?.mood?.thoughts?.memories.TryGainMemory(Props.AboutMeThoughtDef, pawn);
		//				return true;
		//			}
		//			Log.Error(string.Concat(pawn, " failed to interact with ", p));
		//		}
		//	}
		//	return false;
		//}

		private void ResetInterval()
		{
			IntRange range = new(16000, 32000);
			hashIntervalTick = range.RandomInRange;
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref hashIntervalTick, "hashIntervalTick", 0);
		}

	}

	public class Gene_BinaryVoice : Gene
	{

		public GeneExtension_Opinion Props => def?.GetModExtension<GeneExtension_Opinion>();

		private int hashIntervalTick = 7200;

		public override void PostAdd()
		{
			base.PostAdd();
			ResetInterval();
		}

		public override void Tick()
		{
			if (!pawn.IsHashIntervalTick(hashIntervalTick))
			{
				return;
			}
			if (!Active)
			{
				return;
			}
			//TryInteractRandomly_CloseTarget(pawn);
			ThoughtUtility.TryInteractRandomly(pawn, false, true, true, out _, this);
			ResetInterval();
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: TryInteract",
					action = delegate
					{
						//TryInteractRandomly_CloseTarget(pawn);
						ThoughtUtility.TryInteractRandomly(pawn, false, true, true, out _, this);
					}
				};
			}
		}

		//public bool TryInteractRandomly_CloseTarget(Pawn pawn)
		//{
		//	if (!ThoughtUtility.Telepath_CanInitiateRandomInteraction(pawn, true))
		//	{
		//		return false;
		//	}
		//	List<Pawn> workingList = pawn.Map.mapPawns.SpawnedPawnsInFaction(pawn.Faction);
		//	workingList.Shuffle();
		//	List<InteractionDef> allDefsListForReading = DefDatabase<InteractionDef>.AllDefsListForReading;
		//	for (int i = 0; i < workingList.Count; i++)
		//	{
		//		Pawn p = workingList[i];
		//		if (!p.RaceProps.Humanlike)
		//		{
		//			continue;
		//		}
		//		if (!InteractionUtility.IsGoodPositionForInteraction(pawn, p))
		//		{
		//			continue;
		//		}
		//		if (!XaG_GeneUtility.HasGeneOfType(this, p))
		//		{
		//			continue;
		//		}
		//		if (p != pawn && ThoughtUtility.Telepath_CanInteractNowWith(pawn, p, true) && InteractionUtility.CanReceiveRandomInteraction(p) && !pawn.HostileTo(p) && allDefsListForReading.TryRandomElementByWeight((InteractionDef x) => (!ThoughtUtility.Telepath_CanInteractNowWith(pawn, p, true, x)) ? 0f : x.Worker.RandomSelectionWeight(pawn, p), out var result))
		//		{
		//			if (ThoughtUtility.TryInteractWith(pawn, p, result, false))
		//			{
		//				return true;
		//			}
		//			Log.Error(string.Concat(pawn, " failed to interact with ", p));
		//		}
		//		continue;
		//	}
		//	return false;
		//}

		private void ResetInterval()
		{
			IntRange range = new(7200, 22000);
			hashIntervalTick = range.RandomInRange;
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref hashIntervalTick, "hashIntervalTick", 0);
		}

	}

	public class Gene_VoidVoice : Gene
	{

        public void VoidInteraction(Pawn target)
		{
			if (ModsConfig.AnomalyActive && !target.Inhumanized())
			{
				target.mindState?.mentalBreaker?.TryDoMentalBreak("WVC_XaG_MentalBreakReason_VoidVoice".Translate(pawn.Named("PAWN"), target.Named("TARGET")), MentalBreakDefOf.HumanityBreak);
                if (pawn.Map != null)
				{
					FleckMaker.AttachedOverlay(pawn, DefDatabase<FleckDef>.GetNamed("PsycastPsychicEffect"), Vector3.zero);
				}
			}
		}

        public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: TryInteract",
					action = delegate
					{
						if (ThoughtUtility.TryInteractRandomly(pawn, true, false, true, out Pawn target, null))
						{
							VoidInteraction(target);
						}
					}
				};
			}
		}

	}

}
