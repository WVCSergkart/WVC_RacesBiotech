using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class XaG_MainButtonDef : MainButtonDef
	{

		public PawnTableDef pawnTableDef;

	}

	public class MainTabWindow_EntitiesWork : MainTabWindow_Work
	{

		protected override PawnTableDef PawnTableDef
		{
			get
			{
				if (def is XaG_MainButtonDef xaG_MainButtonDef)
				{
					return xaG_MainButtonDef.pawnTableDef;
				}
				return base.PawnTableDef;
			}
		}

		protected override IEnumerable<Pawn> Pawns => GetMutants();

		public static IEnumerable<Pawn> GetMutants()
		{
			return Find.CurrentMap.mapPawns.AllPawnsSpawned.Where((Pawn pawn) => IsWorkerMutant(pawn));
		}

		public static bool IsWorkerMutant(Pawn pawn)
		{
			return pawn.workSettings != null && pawn.IsMutant && pawn.mutant.Def is XaG_MutantDef && pawn.Faction == Faction.OfPlayer;
		}

	}
	//public class MainButtonWorker_HiddenTab : MainButtonWorker_ToggleTab
	//{

	//	public override bool Visible => false;

	//}

	public class MainButtonWorker_AutoInitTab : MainButtonWorker_ToggleTab
	{

		public static bool? visible = false;
		public override bool Visible
		{
			get
			{
				if (visible == null)
				{
					visible = Find.Maps.Any(map => map.mapPawns.AllPawns.Any(pawn => MainTabWindow_EntitiesWork.IsWorkerMutant(pawn)));
				}
				return visible.Value;
			}
		}

		public override void InterfaceTryActivate()
		{
			InitWorkPriority();
			base.InterfaceTryActivate();
		}

		private static bool initialized = false;
		private void InitWorkPriority()
		{
			if (initialized)
			{
				return;
			}
			try
			{
				if (def is XaG_MainButtonDef xaG_MainButtonDef)
				{
					PawnTableDef workTable = xaG_MainButtonDef.pawnTableDef;
					foreach (PawnColumnDef item in DefDatabase<PawnColumnDef>.AllDefsListForReading)
					{
						if (item.Worker is PawnColumnWorker_WorkPriority)
						{
							workTable.columns.Insert(workTable.columns.FindIndex((PawnColumnDef x) => x.Worker is PawnColumnWorker_CopyPasteWorkPriorities) + 1, item);
						}
					}
				}
			}
			catch
			{
				Log.Error("Failed init work priorities.");
			}
			initialized = true;
		}

	}

}
