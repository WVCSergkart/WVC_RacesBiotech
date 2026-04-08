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
		public override bool IsVisible => true;

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

		private GeneSet UnbornBabyHediffGeneset()
		{
			//Log.Error("0");
			Pawn selPawnForGenes = base.SelPawnForGenes;
			if (selPawnForGenes != null)
			{
				//Log.Error("1");
				foreach (Hediff hediff in selPawnForGenes.health.hediffSet.hediffs)
				{
					//Log.Error("2");
					if (hediff is Hediff_BudPregnancy hediffWithParents)
					{
						//Log.Error("3");
						return hediffWithParents.geneSet;
					}
				}
			}
			//Log.Error("4");
			CompHumanEgg compHumanEgg = base.SelThing.TryGetComp<CompHumanEgg>();
			if (compHumanEgg != null)
			{
				//Log.Error("5");
				//if (compHumanEgg.geneSet == null)
				//{
				//	Log.Error("0");
				//}
				//else
				//{
				//	Log.Error("1");
				//}
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
