using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_MachineWoundHealing : Gene_OverOverridable
	{

		public GeneExtension_Undead Undead => def.GetModExtension<GeneExtension_Undead>();

		public override void Tick()
		{
			// base.Tick();
			if (!pawn.IsHashIntervalTick(676))
			{
				return;
			}
			HealingUtility.Regeneration(pawn, Undead.regeneration, WVC_Biotech.settings.totalHealingIgnoreScarification, 676);
		}

	}

	[Obsolete]
	public class Gene_Regeneration : Gene_MachineWoundHealing
	{


	}

}
