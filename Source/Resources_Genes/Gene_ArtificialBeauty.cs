using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_ArtificialBeauty : XaG_Gene, IGeneOverriddenBy
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

		private static List<Pawn> cachedAngelicPawns;
		public static List<Pawn> AngelPawns
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
					cachedAngelicPawns = list;
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

		public static void ResetCollection()
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

}
