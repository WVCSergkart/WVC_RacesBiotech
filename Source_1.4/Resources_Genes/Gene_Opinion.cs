using System.Collections.Generic;
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

	public class Gene_AngelBeauty : Gene_DustDrain
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

}
