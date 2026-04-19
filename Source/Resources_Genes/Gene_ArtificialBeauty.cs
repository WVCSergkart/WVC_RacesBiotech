using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_ArtificialBeauty : XaG_Gene, IGeneOverriddenBy, IGeneRecacheable
	{

		public static bool thoughtDisabled = false;
		//public static bool ThoughtDisabled
		//{
		//	get
		//	{
		//		if (thoughtDisabled == null)
		//		{
		//			_ = AngelPawns;
		//		}
		//		return thoughtDisabled.Value;
		//	}
		//}

		private static HashSet<Pawn> cachedAngelicPawns;
		public static HashSet<Pawn> AngelPawns
		{
			get
			{
				if (cachedAngelicPawns == null)
				{
					List<Pawn> list = new();
					foreach (Pawn pawn in PawnsFinder.All_AliveOrDead)
					{
						if (pawn?.genes?.GetFirstGeneOfType<Gene_ArtificialBeauty>() != null)
						{
							list.Add(pawn);
						}
					}
					thoughtDisabled = list.NullOrEmpty();
					cachedAngelicPawns = [..list];
				}
				return cachedAngelicPawns;
			}
		}

		public override void PostAdd()
		{
			base.PostAdd();
			ResetCollection();
		}

		public override void PostRemove()
		{
			base.PostRemove();
			ResetCollection();
		}

		public virtual void ResetCollection()
		{
			thoughtDisabled = false;
			cachedAngelicPawns = null;
		}

		public void Notify_OverriddenBy(Gene overriddenBy)
		{
			ResetCollection();
		}

		public void Notify_Override()
		{
			ResetCollection();
		}

		public override void ExposeData()
		{
			base.ExposeData();
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				ResetCollection();
			}
		}

		public void Notify_GenesRecache(Gene changedGene)
		{
			ResetCollection();
		}

	}

	[Obsolete]
	public class Gene_AngelBeauty : Gene_ArtificialBeauty
	{

		//private GeneExtension_Opinion cachedExtension;
		//public GeneExtension_Opinion Props
		//{
		//	get
		//	{
		//		if (cachedExtension == null)
		//		{
		//			cachedExtension = def?.GetModExtension<GeneExtension_Opinion>();
		//		}
		//		return cachedExtension;
		//	}
		//}

		//public override void TickInterval(int delta)
		//{
		//	//base.Tick();
		//	if (!pawn.IsHashIntervalTick(57250, delta))
		//	{
		//		return;
		//	}
		//	Gene_Opinion.SetOpinion(pawn, this, Props);
		//}

		//public override IEnumerable<Gizmo> GetGizmos()
		//{
		//	if (DebugSettings.ShowDevGizmos)
		//	{
		//		yield return new Command_Action
		//		{
		//			defaultLabel = "DEV: SetOpinion",
		//			action = delegate
		//			{
		//				Gene_Opinion.SetOpinion(pawn, this, Props);
		//			}
		//		};
		//	}
		//}

	}

	public class Gene_AbominationallyUgly : Gene_ArtificialBeauty, IGeneOverriddenBy
	{

		private static HashSet<Pawn> cachedLeperPawns;
		public static HashSet<Pawn> LeperPawns
		{
			get
			{
				if (cachedLeperPawns == null)
				{
					List<Pawn> list = new();
					foreach (Pawn pawn in PawnsFinder.All_AliveOrDead)
					{
						if (pawn?.genes?.GetFirstGeneOfType<Gene_AbominationallyUgly>() != null)
						{
							list.Add(pawn);
						}
					}
					thoughtDisabled = list.NullOrEmpty();
					cachedLeperPawns = [..list];
				}
				return cachedLeperPawns;
			}
		}

		//private static HashSet<Pawn> cachedVoidFascinationPawns;
		//public static HashSet<Pawn> VoidFascinationPawns
		//{
		//	get
		//	{
		//		if (cachedVoidFascinationPawns == null)
		//		{
		//			List<Pawn> list = new();
		//			foreach (Pawn pawn in PawnsFinder.All_AliveOrDead)
		//			{
		//				if (IsVoidFacinationPawn(pawn))
		//				{
		//					list.Add(pawn);
		//				}
		//			}
		//			cachedVoidFascinationPawns = [.. list];
		//		}
		//		return cachedVoidFascinationPawns;
		//	}
		//}

		///// <summary>
		///// Integration Hook
		///// </summary>
		///// <param name="pawn"></param>
		///// <returns></returns>
		//private static bool IsVoidFacinationPawn(Pawn pawn)
		//{
		//	return ModsConfig.AnomalyActive && pawn.story?.traits?.HasTrait(TraitDefOf.VoidFascination) == true;
		//}

		public override void ResetCollection()
		{
			//cachedVoidFascinationPawns = null;
			cachedLeperPawns = null;
			base.ResetCollection();
		}

	}

}
