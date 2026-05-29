using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class ChimeraDevourer
	{

		public ChimeraDevourerDef def;

		public virtual bool CanFire(Pawn victim, Pawn caster)
		{
			List<PawnKindDef> targetPawnKindDefs = def.targetPawnKindDefs;
			if (targetPawnKindDefs != null)
			{
				return targetPawnKindDefs.Contains(victim.kindDef);
			}
			return false;
		}

		public virtual void Action(Pawn victim, Pawn caster)
		{

		}

	}

	public class ChimeraDevourer_RewardXenotype : ChimeraDevourer
	{

		public override bool CanFire(Pawn victim, Pawn caster)
		{
			if (def.XenotypeDefs.NullOrEmpty())
			{
				return false;
			}
			return base.CanFire(victim, caster);
		}

		public override void Action(Pawn victim, Pawn caster)
		{
			Dialog_MessageBox window = Dialog_MessageBox.CreateConfirmation("WVC_XaG_ChimeraDevourWarning".Translate(), 
				delegate
				{
					XaG_GeneUtility.ImplantChimeraEvolveGeneSet(caster, def.XenotypeDefs.RandomElement());
				}, 
				delegate
				{
					caster.genes.GetFirstGeneOfType<Gene_Chimera>()?.TryAddGenesFromList(def.XenotypeDefs.RandomElement().genes);
				}
			);
			window.buttonBText = "Genes".Translate().CapitalizeFirst();
			window.buttonAText = "Xenotype".Translate().CapitalizeFirst();
			Find.WindowStack.Add(window);
		}

	}

}
