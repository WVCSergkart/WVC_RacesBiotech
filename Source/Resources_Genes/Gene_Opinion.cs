using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Opinion : Gene
	{

		public GeneExtension_Opinion Props => def?.GetModExtension<GeneExtension_Opinion>();

		public override void Tick()
		{
			base.Tick();
			if (!pawn.IsHashIntervalTick(60000))
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

		public static void SetOpinion(Pawn pawn, Gene gene, GeneExtension_Opinion props)
		{
			if (props == null)
			{
				return;
			}
			if (props.AboutMeThoughtDef != null)
			{
				GeneFeaturesUtility.PawnMapOpinionAboutMe(pawn, gene, props.AboutMeThoughtDef, props.targetShouldBePsySensitive, props.targetShouldBeFamily, props.ignoreIfHasGene, props.onlySameXenotype);
			}
			if (props.MeAboutThoughtDef != null)
			{
				GeneFeaturesUtility.MyOpinionAboutPawnMap(pawn, gene, props.MeAboutThoughtDef, props.targetShouldBePsySensitive, props.targetShouldBeFamily, props.ignoreIfHasGene, props.onlySameXenotype);
			}
		}

	}

	public class Gene_AngelBeauty : Gene_DustDrain
	{

		public GeneExtension_Opinion Props => def?.GetModExtension<GeneExtension_Opinion>();

		public override void Tick()
		{
			base.Tick();
			if (!pawn.IsHashIntervalTick(60000))
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
					defaultLabel = "DEV: AngelBeauty Opinion",
					action = delegate
					{
						Gene_Opinion.SetOpinion(pawn, this, Props);
					}
				};
			}
		}

	}

	// public class Gene_Family : Gene_DustDrain
	// {

		// public override void Tick()
		// {
			// base.Tick();
			// if (!pawn.IsHashIntervalTick(60000))
			// {
				// return;
			// }
			// GeneFeaturesUtility.PawnMapOpinionAboutMe(pawn, this, WVC_GenesDefOf.WVC_XenotypesAndGenes_AngelBeauty, true);
		// }

		// public override IEnumerable<Gizmo> GetGizmos()
		// {
			// if (DebugSettings.ShowDevGizmos)
			// {
				// yield return new Command_Action
				// {
					// defaultLabel = "DEV: AngelBeauty Opinion",
					// action = delegate
					// {
						// GeneFeaturesUtility.PawnMapOpinionAboutMe(pawn, this, WVC_GenesDefOf.WVC_XenotypesAndGenes_AngelBeauty, true);
					// }
				// };
			// }
		// }

	// }

}
