using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class Gene_Subhuman : Gene, IGeneOverridden
	{

		public GeneExtension_Giver Giver => def?.GetModExtension<GeneExtension_Giver>();

		public override void PostAdd()
		{
			base.PostAdd();
			ClearOrSetPawnAsMutantInstantly(pawn, Giver?.mutantDef);
		}

		public void Notify_OverriddenBy(Gene overriddenBy)
		{
			if (overriddenBy != null)
			{
				ClearOrSetPawnAsMutantInstantly(pawn, null);
			}
		}

		public void Notify_Override()
		{
			ClearOrSetPawnAsMutantInstantly(pawn, Giver?.mutantDef);
		}

		public override void PostRemove()
		{
			base.PostRemove();
			ClearOrSetPawnAsMutantInstantly(pawn, null);
		}

		public static void ClearOrSetPawnAsMutantInstantly(Pawn pawn, MutantDef mutant)
		{
			if (mutant == null)
			{
				Revert(pawn);
				pawn.mutant = null;
				return;
			}
			if (pawn.mutant?.Def == mutant)
			{
				return;
			}
			Revert(pawn);
			pawn.mutant = new Pawn_MutantTracker(pawn, mutant, RotStage.Fresh);
			pawn.mutant.Turn(clearLord: true);
			static void Revert(Pawn pawn)
			{
				if (pawn.IsMutant)
				{
					pawn.mutant.Revert(false);
				}
			}
		}

	}

}
