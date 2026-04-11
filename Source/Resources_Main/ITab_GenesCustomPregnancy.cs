using RimWorld;
using System.Linq;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{
	//public class Alert_CanLayEgg : Alert
	//{

	//	public static bool disabled = true;

	//	public Alert_CanLayEgg()
	//	{
	//		defaultLabel = "WVC_AlertCanLayEgg".Translate();
	//		defaultExplanation = "WVC_AlertCanLayEggDesc".Translate();
	//		defaultPriority = AlertPriority.High;
	//	}

	//	public static bool Disabled
	//	{
	//		get
	//		{
	//			return disabled;
	//		}
	//		set
	//		{
	//			cachedOvipPawns = null;
	//			if (value == true && (OvipositorPawns.NullOrEmpty() || OvipositorPawns.All((p) => p.genes?.GetFirstGeneOfType<Gene_Ovipositor>()?.shouldLayEgg == false)))
	//			{
	//				disabled = value;
	//			}
	//		}
	//	}

	//	public static List<Pawn> cachedOvipPawns;
	//	public static List<Pawn> OvipositorPawns
	//	{
	//		get
	//		{
	//			if (cachedOvipPawns == null)
	//			{
	//				cachedOvipPawns = PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive.Where((p) => p.genes?.GetFirstGeneOfType<Gene_Ovipositor>() != null).ToList();
	//			}
	//			return cachedOvipPawns;
	//		}
	//	}

	//	public override AlertReport GetReport()
	//	{
	//		if (Disabled)
	//		{
	//			return false;
	//		}
	//		return AlertReport.CulpritsAre(OvipositorPawns.Where((p) => p.Map != null).Select((p) => new GlobalTargetInfo(p.Position, p.Map)).ToList());
	//	}
	//}
	//public class MayRequireSauridsForkAttribute : MayRequireAttribute
	//{
	//	public MayRequireSauridsForkAttribute()
	//		: base("vanillaracesexpanded.saurid.forked")
	//	{
	//	}
	//}
	public class ITab_GenesCustomPregnancy : RimWorld.ITab_GenesPregnancy
	{
		//public override bool IsVisible => true;

		protected override bool StillValid
		{
			get
			{
				if (Find.MainTabsRoot.OpenTab != MainButtonDefOf.Inspect)
				{
					return false;
				}
				MainTabWindow_Inspect mainTabWindow_Inspect = (MainTabWindow_Inspect)Find.MainTabsRoot.OpenTab.TabWindow;
				if (mainTabWindow_Inspect.CurTabs != null)
				{
					return mainTabWindow_Inspect.CurTabs.Contains(this);
				}
				return true;
			}
		}

		protected override Pawn SelPawn
		{
			get
			{
				//if (base.SelThing is Pawn pawn)
				//{
				//	return pawn;
				//}
				return base.SelPawnForGenes ?? base.SelPawn;
			}
		}

		private GeneSet UnbornBabyHediffGeneset()
		{
			Pawn selPawnForGenes = SelPawn;
			if (selPawnForGenes != null)
			{
				foreach (Hediff hediff in selPawnForGenes.health.hediffSet.hediffs)
				{
					if (hediff is IHediffCustomPregnancy hediffWithParents)
					{
						return hediffWithParents.GeneSet;
					}
					if (hediff is HediffWithComps hediffWithComps)
					{
						foreach (HediffComp hediffComp in hediffWithComps.comps)
						{
							if (hediffComp is IHediffCustomPregnancy hediffCompCustomPregnancy)
							{
								return hediffCompCustomPregnancy.GeneSet;
							}
						}
					}
				}
			}
			CompHumanEgg compHumanEgg = base.SelThing.TryGetComp<CompHumanEgg>();
			if (compHumanEgg != null)
			{
				return compHumanEgg.geneSet;
			}
			return null;
		}

		protected override void FillTab()
		{
			GeneUIUtility.DrawGenesInfo(new Rect(0f, 20f, size.x, size.y - 20f), null, 550f, ref size, ref scrollPosition, UnbornBabyHediffGeneset());
		}
	}

}
