using RimWorld;
using System.Collections.Generic;
using Verse;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_GeneBlesslinkInfo : CompProperties
	{

		public bool onlyPlayerFaction = true;

		public CompProperties_GeneBlesslinkInfo()
		{
			compClass = typeof(CompGeneBlesslinkInfo);
		}
	}

	public class CompGeneBlesslinkInfo : ThingComp
	{

		private CompProperties_GeneBlesslinkInfo Props => (CompProperties_GeneBlesslinkInfo)props;

		public Gene_DustMechlink HaveBlesslink(Pawn pawn)
		{
			Gene_DustMechlink blesslink = pawn.genes?.GetFirstGeneOfType<Gene_DustMechlink>();
			if (blesslink != null)
			{
				return blesslink;
			}
			return null;
		}

		public override string CompInspectStringExtra()
		{
			if (WVC_Biotech.settings.enableGeneBlesslinkInfo == true)
			{
                if (parent is Pawn pawn)
                {
					Gene_DustMechlink blesslink = HaveBlesslink(pawn);
					if (blesslink != null)
					{
						if (Props.onlyPlayerFaction)
						{
							if (pawn.Faction != null && pawn.Faction != Faction.OfPlayer)
							{
								return null;
							}
						}
						return Info(blesslink);
					}
                }
            }
			return null;
		}

		public string Info(Gene_DustMechlink blesslink)
		{
			string info = null;
			if (blesslink.summonMechanoids)
			{
				info = "WVC_XaG_Gene_Blesslin_On_Info".Translate().Resolve() + ": " + blesslink.timeForNextSummon.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor);
			}
			return info;
		}
	}

}
