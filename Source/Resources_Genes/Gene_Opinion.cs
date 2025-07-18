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

		public override void TickInterval(int delta)
		{
			//base.Tick();
			if (!pawn.IsHashIntervalTick(57250, delta))
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

		public override void TickInterval(int delta)
		{
			//base.Tick();
			if (!pawn.IsHashIntervalTick(57250, delta))
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

		public override void TickInterval(int delta)
		{
			//base.Tick();
			if (!pawn.IsHashIntervalTick(57250, delta))
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

	public class Gene_Speaker : Gene
	{

		private int nextTick = 7200;

		public override void TickInterval(int delta)
		{
			nextTick -= delta;
			if (nextTick > 0)
			{
				return;
			}
			// Debugger DO NOT TOUCH
			if (nextTick < 0 - delta)
			{
				ResetInterval(new(7200, 7200));
				return;
			}
			TryInteract();
		}

		public virtual void TryInteract()
		{

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
						TryInteract();
					}
				};
			}
		}

		public virtual void ResetInterval(IntRange range)
		{
			//IntRange range = new(16000, 32000);
			nextTick = range.RandomInRange;
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref nextTick, "hashIntervalTick", 0);
		}

	}

	public class Gene_SweetVoice : Gene_Speaker
	{

		public GeneExtension_Opinion Props => def?.GetModExtension<GeneExtension_Opinion>();

		public override void PostAdd()
		{
			base.PostAdd();
			ResetInterval(new(7200, 22000));
		}

		public override void TryInteract()
		{
			if (GeneInteractionsUtility.TryInteractRandomly(pawn, true, false, true, out Pawn target))
            {
				target.needs?.mood?.thoughts?.memories.TryGainMemory(Props.AboutMeThoughtDef, pawn);
			}
			ResetInterval(new(7200, 22000));
		}

	}

	public class Gene_BinaryVoice : Gene_Speaker
	{

		//public string RemoteActionName
		//{
		//	get
		//	{
		//		if (isActive)
		//		{
		//			return "WVC_XaG_Gene_DustMechlink_On".Translate();
		//		}
		//		return "WVC_XaG_Gene_DustMechlink_Off".Translate();
		//	}
		//}

  //      public string RemoteActionDesc => "WVC_XaG_RemoteControlEnergyDesc".Translate();

  //      public override bool Active
		//{
		//	get
		//	{
		//		if (!isActive)
		//		{
		//			return false;
		//		}
		//		return base.Active;
		//	}
		//}

		//private bool isActive = true;

		//public void Remote—ontrol()
		//{
		//	isActive = !isActive;
		//	XaG_GeneUtility.Notify_GenesChanged(pawn);
		//}

		//public override void ExposeData()
		//{
		//	base.ExposeData();
		//	Scribe_Values.Look(ref isActive, "isActive", defaultValue: true);
		//}

		public override void PostAdd()
		{
			base.PostAdd();
			ResetInterval(new(7200, 22000));
		}

		public override void TryInteract()
		{
			GeneInteractionsUtility.TryInteractRandomly(pawn, false, true, true, out _, this);
			ResetInterval(new(7200, 22000));
		}

	}

	public class Gene_VoidVoice : Gene_Speaker
	{

		public override void PostAdd()
		{
			base.PostAdd();
			ResetInterval(new(57362, 262950));
		}

		public override void TryInteract()
		{
			if (GeneInteractionsUtility.TryInteractRandomly(pawn, true, false, true, out Pawn target, null, InteractionDefOf.DisturbingChat))
			{
				VoidInteraction(target);
			}
			ResetInterval(new(57362, 262950));
		}

		public void VoidInteraction(Pawn target)
		{
			if (ModsConfig.AnomalyActive && !target.Inhumanized() && Find.Anomaly.LevelDef != MonolithLevelDefOf.Disrupted)
			{
				target.mindState?.mentalBreaker?.TryDoMentalBreak("WVC_XaG_MentalBreakReason_VoidVoice".Translate(pawn.Named("PAWN"), target.Named("TARGET")), MentalBreakDefOf.HumanityBreak);
                if (pawn.Map != null)
				{
					FleckMaker.AttachedOverlay(pawn, DefDatabase<FleckDef>.GetNamed("PsycastPsychicEffect"), Vector3.zero);
				}
			}
		}

	}

	public class Gene_Recluse : Gene
	{

		public GeneExtension_Opinion Props => def?.GetModExtension<GeneExtension_Opinion>();

		private int nextTick = 841;

        public override void PostAdd()
        {
            base.PostAdd();
			if (MiscUtility.GameStarted())
			{
				MiscUtility.CountAllPlayerControlledPawns_StaticCollection();
			}
        }

        public override void TickInterval(int delta)
		{
			if (!GeneResourceUtility.CanTick(ref nextTick, 47618, delta))
			{
				return;
			}
			TryInteract();
		}

		public void TryInteract()
		{
			pawn.needs?.mood?.thoughts?.memories.RemoveMemoriesOfDef(Props.thoughtDef);
			if (StaticCollectionsClass.cachedColonistsCount > Props.colonistsLimit)
			{
				pawn.needs?.mood?.thoughts?.memories.TryGainMemory(Props.thoughtDef, null);
			}
		}

	}

	public class Gene_HumanCentric : Gene
	{

		public GeneExtension_Opinion Props => def?.GetModExtension<GeneExtension_Opinion>();

		private int nextTick = 842;

		public override void PostAdd()
		{
			base.PostAdd();
			if (MiscUtility.GameStarted())
			{
				MiscUtility.CountAllPlayerControlledPawns_StaticCollection();
			}
		}

		public override void TickInterval(int delta)
		{
			if (!GeneResourceUtility.CanTick(ref nextTick, 47619, delta))
			{
				return;
			}
			TryInteract();
		}

		public void TryInteract()
		{
			pawn.needs?.mood?.thoughts?.memories.RemoveMemoriesOfDef(Props.thoughtDef);
			if (StaticCollectionsClass.cachedNonHumansCount > 0)
			{
				pawn.needs?.mood?.thoughts?.memories.TryGainMemory(Props.thoughtDef, null);
			}
		}

	}

}
