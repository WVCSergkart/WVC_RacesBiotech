using RimWorld;
using System.Collections.Generic;
using Verse;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_GeneUndeadInfo : CompProperties
	{

		public bool onlyPlayerFaction = true;

		public CompProperties_GeneUndeadInfo()
		{
			compClass = typeof(CompGeneUndeadInfo);
		}
	}

	public class CompGeneUndeadInfo : ThingComp
	{

		private CompProperties_GeneUndeadInfo Props => (CompProperties_GeneUndeadInfo)props;

		public Gene_Undead Undead(Pawn pawn)
		{
			Gene_Undead canResurrect = pawn.genes?.GetFirstGeneOfType<Gene_Undead>();
			if (canResurrect != null)
			{
				return canResurrect;
			}
			return null;
		}

		public override string CompInspectStringExtra()
		{
			if (WVC_Biotech.settings.enableGeneUndeadInfo == true)
			{
                if (parent is Pawn pawn)
                {
					Gene_Undead canResurrect = Undead(pawn);
					if (canResurrect != null)
					{
						if (Props.onlyPlayerFaction)
						{
							if (pawn.Faction != null && pawn.Faction != Faction.OfPlayer)
							{
								return null;
							}
						}
						return Info(canResurrect);
					}
                }
            }
			return null;
		}

		public string Info(Gene_Undead canResurrect)
		{
			string info = null;
			if (canResurrect.PawnCanResurrect)
			{
				info = "WVC_XaG_Gene_Undead_On_Info".Translate().Resolve();
			}
			return info;
		}
	}

}
