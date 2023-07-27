using RimWorld;
using System.Collections.Generic;
using Verse;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_GeneWingInfo : CompProperties
	{

		public bool onlyPlayerFaction = false;

		public CompProperties_GeneWingInfo()
		{
			compClass = typeof(CompGeneWingInfo);
		}
	}

	public class CompGeneWingInfo : ThingComp
	{

		private CompProperties_GeneWingInfo Props => (CompProperties_GeneWingInfo)props;

		public Gene_Wings HaveWings(Pawn pawn)
		{
			Gene_Wings wings = pawn.genes?.GetFirstGeneOfType<Gene_Wings>();
			if (wings != null)
			{
				return wings;
			}
			return null;
		}

		public override string CompInspectStringExtra()
		{
			if (WVC_Biotech.settings.enableGeneWingInfo == true)
			{
                if (parent is Pawn pawn)
                {
					Gene_Wings wings = HaveWings(pawn);
					if (wings != null)
					{
						if (Props.onlyPlayerFaction)
						{
							if (pawn.Faction != null && pawn.Faction != Faction.OfPlayer)
							{
								return null;
							}
						}
						return Info(wings, pawn);
					}
                }
            }
			return null;
		}

		public string Info(Gene_Wings wings, Pawn pawn)
		{
			string info = null;
			if (pawn.health.hediffSet.HasHediff(wings.HediffDefName))
			{
				info = "WVC_XaG_Gene_Wings_On_Info".Translate();
			}
			return info;
		}
	}

}
